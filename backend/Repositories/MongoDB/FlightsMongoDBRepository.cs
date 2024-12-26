using AutoMapper;
using backend.Database;
using backend.Models;
using backend.Models.MongoDB;
using backend.Utils;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

namespace backend.Repositories.MongoDB
{
    public class FlightsMongoDBRepository(MongoDBContext context, IMapper mapper) : IFlightRepository
    {
        private readonly MongoDBContext _context = context;
        private readonly IMapper _mapper = mapper;
        public async Task<Flight> Create(Flight flight)
        {
            // Check for overlapping flights
            var overLappingFlights = await GetFlightsByAirplaneIdAndTimeInterval(flight);
            if (overLappingFlights.Any())
            {
                throw new InvalidOperationException("There are 1 or more overlapping flights.");
            }

            // Generate a new flight ID
            var newFlightId = UniqueSequenceGenerator.GenerateLongIdUsingTicks();
            flight.Id = newFlightId;

            // Concurrently find and map related entities
            var airplaneTask = _context.Airplanes.FindAsync(flight.FlightsAirplaneId);
            var airlineTask = _context.Airlines.FindAsync(flight.FlightsAirlineId);
            var departurePortTask = _context.Airports.FindAsync(flight.DeparturePort);
            var arrivalPortTask = _context.Airports.FindAsync(flight.ArrivalPort);

            await Task.WhenAll(airplaneTask.AsTask(), airlineTask.AsTask(), departurePortTask.AsTask(), arrivalPortTask.AsTask());

            // Map and assign related entities
            var flightAirplane = await airplaneTask;
            if (flightAirplane == null)
            {
                throw new InvalidOperationException("Airplane not found.");
            }
            flight.FlightsAirplane = _mapper.Map<Airplane>(flightAirplane);

            var airline = await airlineTask;
            if (airline == null)
            {
                throw new InvalidOperationException("Airline not found.");
            }
            flight.FlightsAirline = _mapper.Map<Airline>(airline);

            var departurePort = await departurePortTask;
            if (departurePort == null)
            {
                throw new InvalidOperationException("Departure port not found.");
            }
            flight.DeparturePortNavigation = _mapper.Map<Airport>(departurePort);

            var arrivalPort = await arrivalPortTask;
            if (arrivalPort == null)
            {
                throw new InvalidOperationException("Arrival port not found.");
            }
            flight.ArrivalPortNavigation = _mapper.Map<Airport>(arrivalPort);

            // Map the flight to the MongoDB entity and save it
            var flightEntity = _mapper.Map<FlightMongo>(flight);
            await _context.Flights.AddAsync(flightEntity);
            await _context.SaveChangesAsync();

            return flight;
        }

        public async Task<Flight> Delete(long id, string deletedBy)
        {

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                throw new InvalidOperationException("Flight not found.");
            }

            var convertedFlight = _mapper.Map<Flight>(flight);

            _context.Flights.Remove(flight);

            // Find bookings with embedded flights matching flight id
            var bookings = await _context.Bookings
                .Where(b => b.Tickets.Any(t => t.Flight.Id == id))
                .Include(b => b.Tickets)
                .ToListAsync();
            
            // Remove tickets from bookings
            foreach (var booking in bookings)
            {
                booking.Tickets.RemoveAll(t => t.Flight.Id == id);
            }

            // Remove bookings without tickets
            _context.Bookings.RemoveRange(bookings.Where(b => b.Tickets.Count == 0));

            await _context.SaveChangesAsync();

            return convertedFlight;
        }

        public async Task<List<Flight>> GetAll()
        {
            var flights = await _context.Flights.ToListAsync();
            return _mapper.Map<List<Flight>>(flights);
        }

        public async Task<Flight?> GetFlightById(long id)
        {
            var flight = await _context.Flights.FindAsync(id);
            return _mapper.Map<Flight>(flight);
        }

        public async Task<Flight?> GetFlightByIdempotencyKey(string idempotencyKey)
        {
            var flight = await _context.Flights.FirstOrDefaultAsync(flight => flight.IdempotencyKey == idempotencyKey);
            return _mapper.Map<Flight>(flight);
        }

        public async Task<FlightClass?> GetFlightClassById(long id)
        {
            var flight = await _context.FlightClasses.FindAsync(id);
            return _mapper.Map<FlightClass>(flight);
        }

        public async Task<List<Flight>> GetFlightsByAirplaneId(long airplaneId)
        {
            var flights = await _context.Flights.Where(flight => flight.FlightsAirplane.Id == airplaneId).ToListAsync();
            return _mapper.Map<List<Flight>>(flights);
        }

        public async Task<List<Flight>> GetFlightsByAirplaneIdAndTimeInterval(Flight newFlight)
        {
            var flights = await _context.Flights
                .Where(flight => flight.FlightsAirplane.Id == newFlight.FlightsAirplaneId
                        && flight.DepartureTime < newFlight.CompletionTime
                        && flight.CompletionTime > newFlight.DepartureTime)
                .ToListAsync();
            return _mapper.Map<List<Flight>>(flights);
        }

        public async Task<List<Flight>> GetFlightsByDepartureDestinationAndDepartureDate(long departureAirportId, long destinationAirportId, DateOnly departureDate)
        {
            var flights = await _context.Flights
                .Where(flight =>
                       flight.DeparturePort.Id == departureAirportId &&
                       flight.ArrivalPort.Id == destinationAirportId &&
                       DateOnly.FromDateTime(flight.DepartureTime) == departureDate
                    )
                .ToListAsync();
            return _mapper.Map<List<Flight>>(flights);
        }

        public async Task<Flight?> GetFlightWithRelationshipsById(long id)
        {
            var flight = await _context.Flights.FindAsync(id);
            return _mapper.Map<Flight>(flight);
        }

        public async Task<List<Ticket>> GetTicketsByFlightId(long flightId)
        {
            var tickets = await _context.Bookings
                .Where(b => b.Tickets.Any(t => t.Flight.Id == flightId))
                .SelectMany(b => b.Tickets.Where(t => t.Flight.Id == flightId))
                .ToListAsync();

            return _mapper.Map<List<Ticket>>(tickets);
        }


        public Task<bool> UpdateFlight(Flight flight)
        {
            throw new NotImplementedException();
        }
    }
}
