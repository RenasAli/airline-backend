using Newtonsoft.Json;

namespace backend.Models.Neo4jModels
{
    public class Neo4jFlight
    {
        [JsonProperty("id")]
        public long Id { get; set; } 

        [JsonProperty("flight_code")]
        public string FlightCode { get; set; } = null!;

        [JsonProperty("departure_port")]
        public long DeparturePort { get; set; }

        [JsonProperty("arrival_port")]
        public long ArrivalPort { get; set; }

        [JsonProperty("departure_time")]
        public DateTime DepartureTime { get; set; }

        [JsonProperty("completion_time")]
        public DateTime CompletionTime { get; set; }

        [JsonProperty("travel_time")]
        public int TravelTime { get; set; }

        [JsonProperty("kilometers")]
        public int? Kilometers { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("economy_class_seats_available")]
        public int EconomyClassSeatsAvailable { get; set; }

        [JsonProperty("first_class_seats_available")]
        public int FirstClassSeatsAvailable { get; set; }

        [JsonProperty("business_class_seats_available")]
        public int BusinessClassSeatsAvailable { get; set; }

        [JsonProperty("flights_airline_id")]
        public long FlightsAirlineId { get; set; }

        [JsonProperty("flights_airplane_id")]
        public long FlightsAirplaneId { get; set; }

        [JsonProperty("idempotency_key")]
        public string? IdempotencyKey { get; set; }

        [JsonProperty("created_by")]
        public string? CreatedBy { get; set; }


        [JsonProperty("arrival_port_navigation")]
        public virtual Neo4jAirport ArrivalPortNavigation { get; set; } = null!;

        [JsonProperty("departure_port_navigation")]
        public virtual Neo4jAirport DeparturePortNavigation { get; set; } = null!;

        [JsonProperty("flights_airline")]
        public virtual Neo4jAirline FlightsAirline { get; set; } = null!;

        [JsonProperty("flights_airplane")]
        public virtual Neo4jAirplane FlightsAirplane { get; set; } = null!;

        [JsonProperty("tickets")]
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
/*await _graphClient.Cypher
        .Match("(departureAirport:Airport)", "(arrivalAirport:Airport)", "(airline:Airline)", "(airplane:Airplane)")
        .Where((Airport departureAirport) => departureAirport.Id == flight.DeparturePort)
        .AndWhere((Airport arrivalAirport) => arrivalAirport.Id == flight.ArrivalPort)
        .AndWhere((Airline airline) => airline.Id == flight.FlightsAirlineId)
        .AndWhere((Airplane airplane) => airplane.Id == flight.FlightsAirplaneId)
        .Create(@"
            (f:Flight {
                id: $id, 
                flight_code: $flightCode, 
                departure_time: $departureTime, 
                completion_time: $completionTime, 
                travel_time: $travelTime, 
                price: $price, 
                kilometers: $kilometers, 
                economy_class_seats_available: $economySeats, 
                business_class_seats_available: $businessSeats, 
                first_class_seats_available: $firstClassSeats, 
                idempotency_key: $idempotencyKey, 
                created_by: $createdBy,
                arrival_port_navigation: $arrivalAirport,
                departure_port_navigation: $departureAirport,
                flights_airline: $airline,
                flights_airplane: $airplane


            })
            -[:DEPARTS_FROM]->(departureAirport)
            -[:ARRIVES_AT]->(arrivalAirport)
            <-[:OPERATES]-(airline)
            <-[:USED_BY]-(airplane)
            ")
        .WithParams(new
        {
            id = flight.Id,
            flightCode = flight.FlightCode,
            departureTime = flight.DepartureTime,
            completionTime = flight.CompletionTime,
            travelTime = flight.TravelTime,
            price = flight.Price,
            kilometers = flight.Kilometers,
            economySeats = flight.EconomyClassSeatsAvailable,
            businessSeats = flight.BusinessClassSeatsAvailable,
            firstClassSeats = flight.FirstClassSeatsAvailable,
            idempotencyKey = flight.IdempotencyKey,
            createdBy = flight.CreatedBy,
            arrivalAirport = flight.ArrivalPortNavigation,
            departureAirport = flight.DeparturePortNavigation,
            airline = flight.FlightsAirline,
            airplane = flight.FlightsAirplane
        })
        .ExecuteWithoutResultsAsync();*/
