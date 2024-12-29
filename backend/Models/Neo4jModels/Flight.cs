namespace backend.Models.Neo4jModels
{
    public class Neo4jFlight
    {
        public long Id { get; set; } 
        public string FlightCode { get; set; } = null!;

        public int DeparturePort { get; set; }

        public int ArrivalPort { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime CompletionTime { get; set; }

        public int TravelTime { get; set; }

        public int? Kilometers { get; set; }

        public decimal Price { get; set; }

         public int EconomyClassSeatsAvailable { get; set; }
        public int FirstClassSeatsAvailable { get; set; }

        public int BusinessClassSeatsAvailable { get; set; }

        public string? IdempotencyKey { get; set; }
        
        public virtual Neo4jAirport ArrivalPortNavigation { get; set; } = null!;

        public virtual Neo4jAirport DeparturePortNavigation { get; set; } = null!;

        public virtual Neo4jAirline FlightsAirline { get; set; } = null!;
        public virtual Neo4jAirplane FlightsAirplane { get; set; } = null!;

        public virtual ICollection<Neo4jTicket> Tickets { get; set; } = new List<Neo4jTicket>();

        public void DecrementSeatAvailability(FlightClassName flightClassName)
    {
        switch (flightClassName)
        {
            case FlightClassName.EconomyClass:
                if (EconomyClassSeatsAvailable > 0)
                    EconomyClassSeatsAvailable--;
                else
                    throw new InvalidOperationException("No Economy class seats available.");
                break;

            case FlightClassName.BusinessClass:
                if (BusinessClassSeatsAvailable > 0)
                    BusinessClassSeatsAvailable--;
                else
                    throw new InvalidOperationException("No Business class seats available.");
                break;

            case FlightClassName.FirstClass:
                if (FirstClassSeatsAvailable > 0)
                    FirstClassSeatsAvailable--;
                else
                    throw new InvalidOperationException("No First Class seats available.");
                break;

            default:
                throw new ArgumentException($"Invalid flight class: {flightClassName}");
        }
    }



    }
}