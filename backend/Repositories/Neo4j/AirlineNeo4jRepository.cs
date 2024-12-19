using Neo4jClient;
using backend.Models.Neo4jModels;
using System.Collections.Generic;
using System.Linq;
using backend.Database;

namespace backend.Repositories.Neo4j;
public class AirlineNeo4jRepository(Neo4jDbContext context)
{
   // private readonly IGraphClient _graphClient = graphClient;
    private readonly Neo4jDbContext _context = context;



   
}