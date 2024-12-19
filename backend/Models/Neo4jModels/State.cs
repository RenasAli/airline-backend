namespace backend.Models.Neo4jModels
{
    public class Neo4jState
    {
        public long Id { get; set; } 
        
        public string Code { get; set; } = null!;

        public virtual ICollection<Neo4jCity> Cities { get; set; } = new List<Neo4jCity>();
    }
}