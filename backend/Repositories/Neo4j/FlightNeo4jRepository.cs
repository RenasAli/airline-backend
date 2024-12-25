using Neo4jClient;
using backend.Models.Neo4jModels;
using AutoMapper;
using backend.Models;
using backend.Utils;
using MongoDB.Bson;

namespace backend.Repositories.Neo4j;
public class FlightNeo4jRepository(IGraphClient graphClient, IMapper mapper): IFlightRepository
{
    private readonly IGraphClient _graphClient = graphClient;
    private readonly IMapper _mapper = mapper;


    public async Task<List<Flight>> GetAll()
    {
        var query = await _graphClient.Cypher
            .Match("(f:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(f)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(f)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(f)-[:FLIES_ON]->(airplane:Airplane)") 
            .Return((f, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = f.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;


            var flights = query
            .Select(result => new Flight
            {
                Id = result.Flight.Id,
                FlightCode = result.Flight.FlightCode,
                DepartureTime = result.Flight.DepartureTime,
                CompletionTime = result.Flight.CompletionTime,
                DeparturePort = result.Flight.DeparturePort,
                ArrivalPort = result.Flight.ArrivalPort,
                TravelTime = result.Flight.TravelTime,
                Price = result.Flight.Price,
                Kilometers = result.Flight.Kilometers,
                EconomyClassSeatsAvailable = result.Flight.EconomyClassSeatsAvailable,
                BusinessClassSeatsAvailable = result.Flight.BusinessClassSeatsAvailable,
                FirstClassSeatsAvailable = result.Flight.FirstClassSeatsAvailable,
                FlightsAirplaneId = result.Flight.FlightsAirplaneId,
                FlightsAirlineId = result.Flight.FlightsAirlineId,
                IdempotencyKey = result.Flight.IdempotencyKey?? "",
                CreatedBy = result.Flight.CreatedBy,
                DeparturePortNavigation = _mapper.Map<Airport>(result.DepartureAirport),
                ArrivalPortNavigation = _mapper.Map<Airport>(result.ArrivalAirport),
                FlightsAirline = _mapper.Map<Airline>(result.Airline),
                FlightsAirplane = _mapper.Map<Airplane>(result.Airplane)
            })
            .ToList();

        return _mapper.Map<List<Flight>>(flights);;  
    }
    

    public async Task<Flight> Create(Flight flight)
    {
        if (flight == null)
        {
            throw new ArgumentNullException(nameof(flight));
        }
        // Check for overlapping flights
        var overlapCount = await _graphClient.Cypher
        .Match("(f:Flight)")
        .Where("f.AirplaneId = $airplaneId")
        .AndWhere("f.DepartureTime < $completionTime AND f.CompletionTime > $departureTime")
        .WithParams(new
        {
            airplaneId = flight.FlightsAirplaneId,
            departureTime = flight.DepartureTime,
            completionTime = flight.CompletionTime
        })
        .Return(f => f.Count())
        .ResultsAsync;

        var singleOverlapCount = overlapCount.SingleOrDefault();

        if (singleOverlapCount > 0)
        {
            throw new InvalidOperationException("Overlap detected with existing flight schedule.");
        }


        // Generate a unique ID for the flight
        flight.Id = UniqueSequenceGenerator.GenerateLongIdUsingTicks();

        await _graphClient.Cypher
            .Match("(departureAirport:Airport)", "(arrivalAirport:Airport)", "(airline:Airline)", "(airplane:Airplane)")
            .Where((Neo4jAirport departureAirport) => departureAirport.Id == flight.DeparturePort)
            .AndWhere((Neo4jAirport arrivalAirport) => arrivalAirport.Id == flight.ArrivalPort)
            .AndWhere((Neo4jAirline airline) => airline.Id == flight.FlightsAirlineId)
            .AndWhere((Neo4jAirline airplane) => airplane.Id == flight.FlightsAirplaneId)
            .Create("(f:Flight {id: $id, flight_code: $flightCode, flights_airplane_id: $airplaneId, departure_time: $departureTime, completion_time: $completionTime, departure_port: $departurePort, arrival_port: $arrivalPort, travel_time: $travelTime, price: $price, kilometers: $kilometers, economy_class_seats_available: $economySeats, business_class_seats_available: $businessSeats, first_class_seats_available: $firstClassSeats, flights_airline_id: $airlineId, idempotency_key: $idempotencyKey, created_by: $createdBy})")
            .Create("(f)-[:DEPARTS_FROM]->(departureAirport)")
            .Create("(f)-[:ARRIVES_AT]->(arrivalAirport)")
            .Create("(f)-[:OPERATED_BY]->(airline)")
            .Create("(f)-[:FLIES_ON]->(airplane)")
            .WithParams(new
            {
                id = flight.Id,
                flightCode = flight.FlightCode,
                airplaneId = flight.FlightsAirplaneId,
                departureTime = flight.DepartureTime,
                completionTime = flight.CompletionTime,
                departurePort = flight.DeparturePort,
                arrivalPort = flight.ArrivalPort,
                travelTime = flight.TravelTime,
                price = flight.Price,
                kilometers = flight.Kilometers,
                economySeats = flight.EconomyClassSeatsAvailable,
                businessSeats = flight.BusinessClassSeatsAvailable,
                firstClassSeats = flight.FirstClassSeatsAvailable,
                airlineId = flight.FlightsAirlineId,
                idempotencyKey = flight.IdempotencyKey,
                createdBy = flight.CreatedBy
            })
            .ExecuteWithoutResultsAsync();

        return flight;
        
    }


    public Task<Flight> Delete(long id, string deletedBy)
    {
        throw new NotImplementedException();
    }

    

    public async Task<Flight?> GetFlightById(long id)
    {
        var query = await _graphClient.Cypher
            .Match("(f:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(f)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(f)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(f)-[:FLIES_ON]->(airplane:Airplane)")
            .Where((Neo4jFlight f) => f.Id == id)
            .Return((f, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = f.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;

            var flight = query
            .Select(result => new Flight
            {
                Id = result.Flight.Id,
                FlightCode = result.Flight.FlightCode,
                DepartureTime = result.Flight.DepartureTime,
                CompletionTime = result.Flight.CompletionTime,
                DeparturePort = result.Flight.DeparturePort,
                ArrivalPort = result.Flight.ArrivalPort,
                TravelTime = result.Flight.TravelTime,
                Price = result.Flight.Price,
                Kilometers = result.Flight.Kilometers,
                EconomyClassSeatsAvailable = result.Flight.EconomyClassSeatsAvailable,
                BusinessClassSeatsAvailable = result.Flight.BusinessClassSeatsAvailable,
                FirstClassSeatsAvailable = result.Flight.FirstClassSeatsAvailable,
                FlightsAirplaneId = result.Flight.FlightsAirplaneId,
                FlightsAirlineId = result.Flight.FlightsAirlineId,
                IdempotencyKey = result.Flight.IdempotencyKey?? "",
                CreatedBy = result.Flight.CreatedBy,
                DeparturePortNavigation = _mapper.Map<Airport>(result.DepartureAirport),
                ArrivalPortNavigation = _mapper.Map<Airport>(result.ArrivalAirport),
                FlightsAirline = _mapper.Map<Airline>(result.Airline),
                FlightsAirplane = _mapper.Map<Airplane>(result.Airplane)
            })
            .SingleOrDefault();


        return flight == null? null : _mapper.Map<Flight>(flight);
    }

    public async Task<Flight?> GetFlightByIdempotencyKey(string idempotencyKey)
    {
        var query = await _graphClient.Cypher
            .Match("(f:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(f)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(f)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(f)-[:FLIES_ON]->(airplane:Airplane)")
            .Where((Neo4jFlight f) => f.IdempotencyKey == idempotencyKey)
            .Return((f, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = f.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;

            var flight = query
            .Select(result => new Flight
            {
                Id = result.Flight.Id,
                FlightCode = result.Flight.FlightCode,
                DepartureTime = result.Flight.DepartureTime,
                CompletionTime = result.Flight.CompletionTime,
                DeparturePort = result.Flight.DeparturePort,
                ArrivalPort = result.Flight.ArrivalPort,
                TravelTime = result.Flight.TravelTime,
                Price = result.Flight.Price,
                Kilometers = result.Flight.Kilometers,
                EconomyClassSeatsAvailable = result.Flight.EconomyClassSeatsAvailable,
                BusinessClassSeatsAvailable = result.Flight.BusinessClassSeatsAvailable,
                FirstClassSeatsAvailable = result.Flight.FirstClassSeatsAvailable,
                FlightsAirplaneId = result.Flight.FlightsAirplaneId,
                FlightsAirlineId = result.Flight.FlightsAirlineId,
                IdempotencyKey = result.Flight.IdempotencyKey?? "",
                CreatedBy = result.Flight.CreatedBy,
                DeparturePortNavigation = _mapper.Map<Airport>(result.DepartureAirport),
                ArrivalPortNavigation = _mapper.Map<Airport>(result.ArrivalAirport),
                FlightsAirline = _mapper.Map<Airline>(result.Airline),
                FlightsAirplane = _mapper.Map<Airplane>(result.Airplane)
            })
            .SingleOrDefault();

        return flight == null? null : _mapper.Map<Flight>(flight);
    }

    public async Task<FlightClass?> GetFlightClassById(long id)
    {
        var query = await _graphClient.Cypher
            .Match("(a:FlightClass)")  
            .Where((Neo4jFlightClass a) => a.Id == id)
            .Return(a => a.As<Neo4jFlightClass>())  
            .ResultsAsync;

            var flight = query.SingleOrDefault();
        return flight == null? null : _mapper.Map<FlightClass>(flight);
    }

    public async Task<List<Flight>> GetFlightsByAirplaneId(long airplaneId)
    {
        var query = await _graphClient.Cypher
            .Match("(f:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(f)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(f)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(f)-[:FLIES_ON]->(airplane:Airplane)")
            .Where((Neo4jFlight f) => f.FlightsAirplaneId == airplaneId)
            .Return((f, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = f.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;

            var flights = query
            .Select(result => new Flight
            {
                Id = result.Flight.Id,
                FlightCode = result.Flight.FlightCode,
                DepartureTime = result.Flight.DepartureTime,
                CompletionTime = result.Flight.CompletionTime,
                DeparturePort = result.Flight.DeparturePort,
                ArrivalPort = result.Flight.ArrivalPort,
                TravelTime = result.Flight.TravelTime,
                Price = result.Flight.Price,
                Kilometers = result.Flight.Kilometers,
                EconomyClassSeatsAvailable = result.Flight.EconomyClassSeatsAvailable,
                BusinessClassSeatsAvailable = result.Flight.BusinessClassSeatsAvailable,
                FirstClassSeatsAvailable = result.Flight.FirstClassSeatsAvailable,
                FlightsAirplaneId = result.Flight.FlightsAirplaneId,
                FlightsAirlineId = result.Flight.FlightsAirlineId,
                IdempotencyKey = result.Flight.IdempotencyKey?? "",
                CreatedBy = result.Flight.CreatedBy,
                DeparturePortNavigation = _mapper.Map<Airport>(result.DepartureAirport),
                ArrivalPortNavigation = _mapper.Map<Airport>(result.ArrivalAirport),
                FlightsAirline = _mapper.Map<Airline>(result.Airline),
                FlightsAirplane = _mapper.Map<Airplane>(result.Airplane)
            })
            .ToList();



        
        return _mapper.Map<List<Flight>>(flights);
    }

    public async Task<List<Flight>> GetFlightsByAirplaneIdAndTimeInterval(Flight flight)
    {
        var query = await _graphClient.Cypher
            .Match("(f:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(f)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(f)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(f)-[:FLIES_ON]->(airplane:Airplane)")
            .Where((Neo4jFlight a) => a.FlightsAirplaneId == flight.FlightsAirplaneId
                    && a.DepartureTime < flight.CompletionTime
                    && a.CompletionTime > flight.DepartureTime)
            .Return((f, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = f.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;

            var flights = query
            .Select(result => new Flight
            {
                Id = result.Flight.Id,
                FlightCode = result.Flight.FlightCode,
                DepartureTime = result.Flight.DepartureTime,
                CompletionTime = result.Flight.CompletionTime,
                DeparturePort = result.Flight.DeparturePort,
                ArrivalPort = result.Flight.ArrivalPort,
                TravelTime = result.Flight.TravelTime,
                Price = result.Flight.Price,
                Kilometers = result.Flight.Kilometers,
                EconomyClassSeatsAvailable = result.Flight.EconomyClassSeatsAvailable,
                BusinessClassSeatsAvailable = result.Flight.BusinessClassSeatsAvailable,
                FirstClassSeatsAvailable = result.Flight.FirstClassSeatsAvailable,
                FlightsAirplaneId = result.Flight.FlightsAirplaneId,
                FlightsAirlineId = result.Flight.FlightsAirlineId,
                IdempotencyKey = result.Flight.IdempotencyKey?? "",
                CreatedBy = result.Flight.CreatedBy,
                DeparturePortNavigation = _mapper.Map<Airport>(result.DepartureAirport),
                ArrivalPortNavigation = _mapper.Map<Airport>(result.ArrivalAirport),
                FlightsAirline = _mapper.Map<Airline>(result.Airline),
                FlightsAirplane = _mapper.Map<Airplane>(result.Airplane)
            })
            .ToList();

        return _mapper.Map<List<Flight>>(flights);
    }

    public async Task<List<Flight>> GetFlightsByDepartureDestinationAndDepartureDate(long departureAirportId, long destinationAirportId, DateOnly departureDate)
    {
        DateTime startOfDay = departureDate.ToDateTime(TimeOnly.MinValue);
        DateTime endOfDay = departureDate.ToDateTime(TimeOnly.MaxValue);

        var query = await _graphClient.Cypher
            .Match("(f:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(f)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(f)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(f)-[:FLIES_ON]->(airplane:Airplane)")
            .Where((Neo4jFlight f) =>
                    f.DeparturePort == departureAirportId &&
                    f.ArrivalPort == destinationAirportId &&
                       f.DepartureTime >= startOfDay &&
                       f.DepartureTime <= endOfDay      
                )
            .Return((f, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = f.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;


            var flights = query
            .Select(result => new Flight
            {
                Id = result.Flight.Id,
                FlightCode = result.Flight.FlightCode,
                DepartureTime = result.Flight.DepartureTime,
                CompletionTime = result.Flight.CompletionTime,
                DeparturePort = result.Flight.DeparturePort,
                ArrivalPort = result.Flight.ArrivalPort,
                TravelTime = result.Flight.TravelTime,
                Price = result.Flight.Price,
                Kilometers = result.Flight.Kilometers,
                EconomyClassSeatsAvailable = result.Flight.EconomyClassSeatsAvailable,
                BusinessClassSeatsAvailable = result.Flight.BusinessClassSeatsAvailable,
                FirstClassSeatsAvailable = result.Flight.FirstClassSeatsAvailable,
                FlightsAirplaneId = result.Flight.FlightsAirplaneId,
                FlightsAirlineId = result.Flight.FlightsAirlineId,
                IdempotencyKey = result.Flight.IdempotencyKey?? "",
                CreatedBy = result.Flight.CreatedBy,
                DeparturePortNavigation = _mapper.Map<Airport>(result.DepartureAirport),
                ArrivalPortNavigation = _mapper.Map<Airport>(result.ArrivalAirport),
                FlightsAirline = _mapper.Map<Airline>(result.Airline),
                FlightsAirplane = _mapper.Map<Airplane>(result.Airplane)
            })
            .ToList();
        
        return _mapper.Map<List<Flight>>(flights);
    }

    public async Task<Flight?> GetFlightWithRelationshipsById(long id)
    {
        var query = await _graphClient.Cypher
            .Match("(f:Flight)-[:DEPARTS_FROM]->(departureAirport:Airport)")
            .Match("(f)-[:ARRIVES_AT]->(arrivalAirport:Airport)")
            .Match("(f)-[:OPERATED_BY]->(airline:Airline)")
            .Match("(f)-[:FLIES_ON]->(airplane:Airplane)")
            .Where((Neo4jFlight f) => f.Id == id)
            .Return((f, departureAirport, arrivalAirport, airline, airplane) => new
            {
                Flight = f.As<Neo4jFlight>(),
                DepartureAirport = departureAirport.As<Neo4jAirport>(),
                ArrivalAirport = arrivalAirport.As<Neo4jAirport>(),
                Airline = airline.As<Neo4jAirline>(),
                Airplane = airplane.As<Neo4jAirplane>()
            })
            .ResultsAsync;

            var flight = query
            .Select(result => new Flight
            {
                Id = result.Flight.Id,
                FlightCode = result.Flight.FlightCode,
                DepartureTime = result.Flight.DepartureTime,
                CompletionTime = result.Flight.CompletionTime,
                DeparturePort = result.Flight.DeparturePort,
                ArrivalPort = result.Flight.ArrivalPort,
                TravelTime = result.Flight.TravelTime,
                Price = result.Flight.Price,
                Kilometers = result.Flight.Kilometers,
                EconomyClassSeatsAvailable = result.Flight.EconomyClassSeatsAvailable,
                BusinessClassSeatsAvailable = result.Flight.BusinessClassSeatsAvailable,
                FirstClassSeatsAvailable = result.Flight.FirstClassSeatsAvailable,
                FlightsAirplaneId = result.Flight.FlightsAirplaneId,
                FlightsAirlineId = result.Flight.FlightsAirlineId,
                IdempotencyKey = result.Flight.IdempotencyKey?? "",
                CreatedBy = result.Flight.CreatedBy,
                DeparturePortNavigation = _mapper.Map<Airport>(result.DepartureAirport),
                ArrivalPortNavigation = _mapper.Map<Airport>(result.ArrivalAirport),
                FlightsAirline = _mapper.Map<Airline>(result.Airline),
                FlightsAirplane = _mapper.Map<Airplane>(result.Airplane)
            })
            .SingleOrDefault();

        return _mapper.Map<Flight>(flight);
    }

    public async Task<List<Ticket>> GetTicketsByFlightId(long flightId)
    {
       var ticketsNeo4j = await _graphClient.Cypher
        .Match("(b:Booking)-[:HAS_TICKET]->(t:Ticket)-[:FOR_FLIGHT]->(f:Flight)") // Match the relationships
        .Where((Neo4jFlight f) => f.Id == flightId) // Filter by Flight ID
        .Return(t => t.As<Neo4jTicket>()) // Return the ticket nodes
        .ResultsAsync;

        var tickets = ticketsNeo4j.ToList(); // Convert to a list
        return _mapper.Map<List<Ticket>>(tickets); // Map to your domain model
    }


    public Task<bool> UpdateFlight(Flight flight)
    {
        throw new NotImplementedException();
    }
}