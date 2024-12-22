using Newtonsoft.Json;

namespace backend.Models.Neo4jModels
{
    public class Neo4jAirport
    {
        [JsonProperty("id")]
        public long Id { get; set; } 

        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("code")]
        public string Code { get; set; } = null!;

        [JsonProperty("city_id")]
        public int? CityId { get; set; }

        [JsonProperty("city")]
        public virtual Neo4jCity? City { get; set; }

        [JsonProperty("flight_arrival_port_navigations")]
        public virtual ICollection<Neo4jFlight> FlightArrivalPortNavigations { get; set; } = new List<Neo4jFlight>();
        
        [JsonProperty("flight_departur_port_navigations")]
        public virtual ICollection<Neo4jFlight> FlightDeparturePortNavigations { get; set; } = new List<Neo4jFlight>();

    }
}