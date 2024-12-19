namespace backend.Models.Neo4jModels
{
    public class Neo4jAirport
    {
        public long Id { get; set; } 
        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;
        public int? CityId { get; set; }

        public virtual Neo4jCity? City { get; set; }

        public virtual ICollection<Neo4jFlight> FlightArrivalPortNavigations { get; set; } = new List<Neo4jFlight>();

        public virtual ICollection<Neo4jFlight> FlightDeparturePortNavigations { get; set; } = new List<Neo4jFlight>();

    }
}