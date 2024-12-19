using Neo4jClient;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using backend.Models.Neo4jModels;
using backend.Models;

namespace backend.Database;

public partial class Neo4jDbContext
{
    public IGraphClient GraphClient { get; }

    // Default constructor
    public Neo4jDbContext()
    {
        var neo4jUrl = Environment.GetEnvironmentVariable("NEO4J_URL")?? "";
        var neo4jUsername = Environment.GetEnvironmentVariable("NEO4J_USERNAME");
        var neo4jPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");

        GraphClient = new BoltGraphClient(new Uri(neo4jUrl), neo4jUsername, neo4jPassword);
        GraphClient.ConnectAsync().Wait();
    }

    public Neo4jDbContext(DbContextOptions options)
    {
        var neo4jUrl = Environment.GetEnvironmentVariable("NEO4J_URL")?? "";
        var neo4jUsername = Environment.GetEnvironmentVariable("NEO4J_USERNAME");
        var neo4jPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");

        GraphClient = new BoltGraphClient(new Uri(neo4jUrl), neo4jUsername, neo4jPassword);
        GraphClient.ConnectAsync().Wait();

    }

     // DbSet-like properties (virtual for mocking/testing)
  public virtual IQueryable<Neo4jAirline> Airlines =>
    (IQueryable<Neo4jAirline>)GraphClient.Cypher
        .Match("(a:Airline)")
        .Return(a => a.As<Neo4jAirline>());
    public virtual IQueryable<Neo4jAirplane> Airplanes => (IQueryable<Neo4jAirplane>)GraphClient.Cypher
        .Match("(a:Airplane)")
        .Return(a => a.As<Neo4jAirplane>());

    public virtual IQueryable<Neo4jAirport> Airports => (IQueryable<Neo4jAirport>)GraphClient.Cypher
        .Match("(a:Airport)")
        .Return(a => a.As<Neo4jAirport>());

    public virtual IQueryable<Neo4jBooking> Bookings => (IQueryable<Neo4jBooking>)GraphClient.Cypher
        .Match("(b:Booking)")
        .Return(b => b.As<Neo4jBooking>());

    public virtual IQueryable<Neo4jCity> Cities => (IQueryable<Neo4jCity>)GraphClient.Cypher
        .Match("(c:City)")
        .Return(c => c.As<Neo4jCity>());

    public virtual IQueryable<Neo4jFlight> Flights => (IQueryable<Neo4jFlight>)GraphClient.Cypher
        .Match("(f:Flight)")
        .Return(f => f.As<Neo4jFlight>());

    public virtual IQueryable<Neo4jFlightClass> FlightClasses => (IQueryable<Neo4jFlightClass>)GraphClient.Cypher
        .Match("(fc:FlightClass)")
        .Return(fc => fc.As<Neo4jFlightClass>());

    public virtual IQueryable<Neo4jPassenger> Passengers => (IQueryable<Neo4jPassenger>)GraphClient.Cypher
        .Match("(p:Passenger)")
        .Return(p => p.As<Neo4jPassenger>());

    public virtual IQueryable<Neo4jState> States => (IQueryable<Neo4jState>)GraphClient.Cypher
        .Match("(s:State)")
        .Return(s => s.As<Neo4jState>());

    public virtual IQueryable<Neo4jTicket> Tickets => (IQueryable<Neo4jTicket>)GraphClient.Cypher
        .Match("(t:Ticket)")
        .Return(t => t.As<Neo4jTicket>());

    public virtual IQueryable<Neo4jUser> Users => (IQueryable<Neo4jUser>)GraphClient.Cypher
        .Match("(u:User)")
        .Return(u => u.As<Neo4jUser>());

    // Dispose method to close connection (optional)
    public void Dispose()
    {
        // Since Neo4jClient doesn't hold connections open like a DbContext, no action is needed.
    }
}