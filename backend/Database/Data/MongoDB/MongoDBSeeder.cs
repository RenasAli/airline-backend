using backend.Models;
using backend.Models.MongoDB;


namespace backend.Database.Data.MongoDB
{
    public class MongoDBSeeder(MongoDBContext context)
    {

        private readonly MongoDBContext _context = context;

        public void Seed()
        {
            if (!_context.Airplanes.Any())
            {
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

                var flights = new List<FlightMongo>()
                {
                    new()
                    {
                        Id = 1,
                        FlightCode = "TEST123",
                        DepartureTime = new DateTime(),
                        CompletionTime = new DateTime().AddHours(5),
                        TravelTime = 200,
                        Kilometers = 2500,
                        Price = 300,
                        EconomyClassSeatsAvailable = 100,
                        BusinessClassSeatsAvailable = 40,
                        FirstClassSeatsAvailable = 10,
                        FlightsAirline = new()
                        {
                            Id = 1,
                            Name = "Delta Airlines"
                        },
                        FlightsAirplane = new()
                        {
                            Id = 1,
                            Name = "Test plane",
                            EconomyClassSeats = 100,
                            BusinessClassSeats = 40,
                            FirstClassSeats = 10
                        },
                        ArrivalPort = new()
                        {
                            Id = 1,
                            Name = "Los angeles airport",
                            Code = "LAX",
                            City = new()
                            {
                                Id = 1,
                                Name = "Los Angeles",
                                State = new()
                                {
                                    Id = 1,
                                    Code = "CA"
                                }
                            }
                        },
                        DeparturePort = new()
                        {
                            Id = 2,
                            Name = "John F Kennedy Airport",
                            Code = "JFK",
                            City = new()
                            {
                                Id = 2,
                                Name = "????",
                                State = new()
                                {
                                    Id = 2,
                                    Code = "WA"
                                }
                            }
                        }
                    }
                };

                var airports = new List<AirportMongo>()
                {
                  new()
                  {
                      Id = 1,
                      Name = "Los angeles airport",
                      Code = "LAX",
                      City = new()
                      {
                         Id = 1,
                         Name = "Los Angeles",
                         State = new()
                         {
                             Id = 1,
                             Code = "CA"
                         }
                      }
                  }
                };

                var users = new List<UserMongo>()
                {
                    new()
                    {
                        Id = 1,
                        Email = "customer@example.com",
                        Password = "AQAAAAIAAYagAAAAEJvAdN3g69LF6cuKWK/xIHyUyz1qtNoVCMgKIlSd5oTPwk+7/A+qEAcxQJ2B+FvghQ==",
                        Role = UserRole.Admin,
                    }
                };

                _context.Users.AddRange(users);
                _context.Airplanes.AddRange(airplanes);
                _context.Flights.AddRange(flights);
                _context.Airports.AddRange(airports);
                _context.SaveChanges();
            }
        }
    }
}
