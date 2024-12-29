namespace backend.Models.Neo4jModels
{
    public class Neo4jFlightClass
    {
        public long Id { get; set; } 
        public FlightClassName Name { get; set; }

        public decimal PriceMultiplier { get; set; }

        public virtual ICollection<Neo4jTicket> Tickets { get; set; } = new List<Neo4jTicket>();

    }
}

