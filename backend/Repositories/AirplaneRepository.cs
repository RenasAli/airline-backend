using backend.Database;
using backend.Models;
using backend.Models.MongoDB;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories
{
    public class AirplaneRepository(DatabaseContext context, MongoDBContext mongoContext): IAirplaneRepository
    {
        private readonly DatabaseContext _context = context;
        private readonly MongoDBContext _mongoContext = mongoContext;
        public async Task<List<Airplane>> GetAll()
        {
            var airplanes = await _context.Airplanes.ToListAsync();
            return airplanes;
        }

        public async Task<Airplane?> GetAirplaneById(int id)
        {
            var airplane = await _context.Airplanes.FindAsync(id);
            return airplane;
        }

        public async Task<List<AirplaneMongo>> GetAirplanesMongoDB()
        {
            var airplanes = await _mongoContext.Airplanes.ToListAsync();
            Console.WriteLine("GOING MONGO: " + airplanes.Count);
            return airplanes;
        }
    }
}
