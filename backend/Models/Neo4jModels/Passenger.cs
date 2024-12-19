namespace backend.Models.Neo4jModels
{
    public class Neo4jPassenger
    {
        public long Id { get; set; } 
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public virtual ICollection<Neo4jTicket> Tickets { get; set; } = [];
    }
}