using backend.Models.MongoDB;

namespace backend.Database.Data.MongoDB
{
    public class MongoDBSeeder(MongoDBContext context)
    {

        private readonly MongoDBContext _context = context;

        public void Seed()
        {
            if (!_context.Airplanes.Any()) {
                var airplanes = new List<AirplaneMongo>()
                {
                  new()
                  {
                      Id = 1,
                      Name = "Boeing 2",
                      AirplanesAirlineId = 1,
                      EconomyClassSeats = 200,
                      BusinessClassSeats = 40,
                      FirstClassSeats = 20
                  },
                  new()
                  {
                      Id = 2,
                      Name = "Boeing 223",
                      AirplanesAirlineId = 1,
                      EconomyClassSeats = 222,
                      BusinessClassSeats = 43,
                      FirstClassSeats = 32
                  },
                  new()
                  {
                      Id = 3,
                      Name = "Boeing 111",
                      AirplanesAirlineId = 1,
                      EconomyClassSeats = 200,
                      BusinessClassSeats = 21,
                      FirstClassSeats = 12
                  }
                };

                _context.Airplanes.AddRangeAsync(airplanes);
                _context.SaveChangesAsync();
                    } }

    } }
