namespace backend.Models.Neo4jModels
{
    public class Neo4jBooking
    {
        public long Id { get; set; } 
        public string ConfirmationNumber { get; set; } = null!;

        public string Code { get; set; } = null!;
        public virtual ICollection<Neo4jTicket> Tickets { get; set; } = new List<Neo4jTicket>();

        public virtual Neo4jUser User { get; set; } = null!;

    }
}