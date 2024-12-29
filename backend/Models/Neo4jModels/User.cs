namespace backend.Models.Neo4jModels
{
    public class Neo4jUser
    {
        public long Id { get; set; } 
        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public UserRole Role { get; set; }


        public virtual ICollection<Neo4jBooking> Bookings { get; set; } = new List<Neo4jBooking>();
    }

    
}