using AutoMapper;
using backend.Database;
using backend.Models;
using backend.Models.MongoDB;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace backend.Repositories.MongoDB
{
    public class FlightsMongoDBRepository(MongoDBContext context, IMapper mapper) : IFlightRepository
    {
        private readonly MongoDBContext _context = context;
        private readonly IMapper _mapper = mapper;
        public Task<Flight> Create(Flight flight)
        {
            throw new NotImplementedException();
            
        }

        public Task<Flight> Delete(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Flight>> GetAll()
        {
            var flights = await _context.Flights.ToListAsync();
            return _mapper.Map<List<Flight>>(flights);
        }

        public async Task<Flight?> GetFlightById(long id)
        {
            throw new NotImplementedException();
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

        public Task<List<Flight>> GetFlightsByAirplaneIdAndTimeInterval(Flight newFlight)
        {
            throw new NotImplementedException();
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
