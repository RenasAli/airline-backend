using AutoMapper;
using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;

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

        public Task<Flight> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Flight>> GetAll()
        {
            var flights = await _context.Flights.ToListAsync();
            return _mapper.Map<List<Flight>>(flights);
        }

        public Task<Flight?> GetFlightById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Flight?> GetFlightByIdempotencyKey(string idempotencyKey)
        {
            throw new NotImplementedException();
        }

        public Task<FlightClass?> GetFlightClassById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Flight>> GetFlightsByAirplaneId(int airplaneId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Flight>> GetFlightsByAirplaneIdAndTimeInterval(Flight newFlight)
        {
            throw new NotImplementedException();
        }

        public Task<List<Flight>> GetFlightsByDepartureDestinationAndDepartureDate(int departureAirportId, int destinationAirportId, DateOnly departureDate)
        {
            throw new NotImplementedException();
        }

        public Task<Flight?> GetFlightWithRelationshipsById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetTicketsByFlightId(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateFlight(Flight flight)
        {
            throw new NotImplementedException();
        }
    }
}
