namespace backend.Models.Neo4jModels
{
    public class Neo4jAirplane
    {
        public long Id { get; set; } 
        public string Name { get; set; } = null!;
        public long AirplanesAirlineId { get; set; }

        public long EconomyClassSeats { get; set; }

        public long BusinessClassSeats { get; set; }

        public long FirstClassSeats { get; set; }

        public virtual Neo4jAirline AirplanesAirline { get; set; } = null!;

        public virtual ICollection<Neo4jFlight> Flights { get; set; } = new List<Neo4jFlight>();
    }
}