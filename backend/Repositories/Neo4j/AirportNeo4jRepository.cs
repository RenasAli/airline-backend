using Neo4jClient;
using backend.Models.Neo4jModels;
using AutoMapper;
using backend.Models;

namespace backend.Repositories.Neo4j;
public class AirportNeo4jRepository(IGraphClient graphClient, IMapper mapper): IAirportRepository
{
    private readonly IGraphClient _graphClient = graphClient;
    private readonly IMapper _mapper = mapper;


    public async Task<List<Airport>> GetAll()
    {
        var query = await _graphClient.Cypher
            .Match("(a:Airport)")  
            .Return(a => a.As<Neo4jAirport>())  
            .ResultsAsync;

        var airports = query.ToList();

        return _mapper.Map<List<Airport>>(airports);;  
    }

    public async Task<List<Airport>> FindByIds(params long[] ids)
        {
            var query = await _graphClient.Cypher
            .Match("(a:Airport)")
            .Where((Neo4jAirplane a) => ids.Contains(a.Id))
            .Return(a => a.As<Neo4jAirport>())  
            .ResultsAsync;

            var airports = query.ToList();

            return _mapper.Map<List<Airport>>(airports);
        }
}
