namespace backend.Models.Neo4jModels
{
    public class Neo4jTicket
    {
        public long Id { get; set; } 

        public decimal Price { get; set; }

        public string TicketNumber { get; set; } = null!;

        public virtual Neo4jFlight Flight { get; set; } = null!;

        public virtual Neo4jPassenger Passenger { get; set; } = null!;

        public virtual Neo4jBooking TicketsBooking { get; set; } = null!;

        public virtual Neo4jFlightClass FlightClass { get; set; } = null!;
    }
}