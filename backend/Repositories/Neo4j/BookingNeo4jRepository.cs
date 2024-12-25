using Neo4jClient;
using backend.Models.Neo4jModels;
using AutoMapper;
using backend.Models;
using backend.Dtos;

namespace backend.Repositories.Neo4j;
public class BookingNeo4jRepository(IGraphClient graphClient, IMapper mapper): IBookingRepository
{
    private readonly IGraphClient _graphClient = graphClient;
    private readonly IMapper _mapper = mapper;
    public Task<Booking> CreateBooking(BookingProcessedRequest bookingProcessedRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Booking>> GetBookingsByUserId(long id)
    {
        var query = await _graphClient.Cypher
            .Match("(b:Booking)")
            .Where((Neo4jBooking b) => b.UserId == id)
            .Return(b => b.As<Neo4jBooking>())  
            .ResultsAsync;
        
        var bookings = query.ToList();
        return _mapper.Map<List<Booking>>(bookings);
    }



}