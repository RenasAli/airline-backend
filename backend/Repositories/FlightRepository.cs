﻿using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;

namespace backend.Repositories
{
    public class FlightRepository: IFlightRepository
    {
        private readonly DatabaseContext _context;
        public FlightRepository(DatabaseContext context) {
            _context = context;
        }

        public async Task<List<Flight>> GetAll()
        {
            var flights = await _context.Flights
                // AsNoTracking gives better performance but should only be used in "read-only" scenarios
                .AsNoTracking()
                .Include(flight => flight.FlightsAirline)
                .Include(flight => flight.FlightsAirplane)
                .Include(flight => flight.DeparturePortNavigation)
                .Include(flight => flight.ArrivalPortNavigation)
                .ToListAsync();
            return flights;
        }

        public async Task<Flight?> GetFlightById(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            return flight;
        }

        public async Task<Flight?> GetFlightWithRelationshipsById(int id)
        {
            var flight = await _context.Flights
                .Include(flight => flight.FlightsAirline)
                .Include(flight => flight.FlightsAirplane)
                .Include(flight => flight.DeparturePortNavigation)
                .Include(flight => flight.ArrivalPortNavigation)
                .FirstOrDefaultAsync(flight => flight.Id == id);
            return flight;
        }

        public async Task<List<Flight>> GetFlightsByAirplaneIdAndTimeInterval(Flight newFlight)
        {
            var flights = await _context.Flights
                .Where(flight => flight.FlightsAirplaneId == newFlight.FlightsAirplaneId
                        && flight.DepartureTime < newFlight.CompletionTime
                        && flight.CompletionTime > newFlight.DepartureTime)
                .ToListAsync();
            return flights;
        }

        public async Task<Flight?> GetFlightByIdempotencyKey(string idempotencyKey)
        {
            Flight? flight = await _context.Flights.FirstOrDefaultAsync((flight) => flight.IdempotencyKey == idempotencyKey);
            return flight;
        }

        public async Task<Flight> Create(Flight flight)
        {
            /*
               Would prefer not to handle connections manually like this, and instead let EF Core handle it all,
               but this is needed to retrieve the last inserted ID from the stored procedure.
            */

            /*
               the 'using' statement makes sure that the database connection is closed/released 
               to the connection pool after the clode block ends, regardless if an exception happens or not.
               Its the same as using try, catch and finally, where you then close the connection in the 'finally' block.
             */
            using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "CheckAndInsertFlight";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new MySqlParameter("@airplaneId", flight.FlightsAirplaneId));
            command.Parameters.Add(new MySqlParameter("@departureTime", flight.DepartureTime));
            command.Parameters.Add(new MySqlParameter("@completionTime", flight.CompletionTime));
            command.Parameters.Add(new MySqlParameter("@flightCode", flight.FlightCode));
            command.Parameters.Add(new MySqlParameter("@departurePort", flight.DeparturePort));
            command.Parameters.Add(new MySqlParameter("@arrivalPort", flight.ArrivalPort));
            command.Parameters.Add(new MySqlParameter("@travelTime", flight.TravelTime));
            command.Parameters.Add(new MySqlParameter("@price", flight.Price));
            command.Parameters.Add(new MySqlParameter("@kilometers", flight.Kilometers));
            command.Parameters.Add(new MySqlParameter("@economySeats", flight.EconomyClassSeatsAvailable));
            command.Parameters.Add(new MySqlParameter("@businessSeats", flight.BusinessClassSeatsAvailable));
            command.Parameters.Add(new MySqlParameter("@firstClassSeats", flight.FirstClassSeatsAvailable));
            command.Parameters.Add(new MySqlParameter("@airlineId", flight.FlightsAirlineId));
            command.Parameters.Add(new MySqlParameter("@idempotencyKey", flight.IdempotencyKey));

            // Output parameter
            var newFlightIdParam = new MySqlParameter("@newFlightId", MySqlDbType.Int32)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newFlightIdParam);

            // Execute the command that calls the "CheckAndInsertFlight" stored procedure.
            await command.ExecuteNonQueryAsync();

            flight.Id = (int)newFlightIdParam.Value;

            return flight;
        }



        public async Task<List<Flight>> GetFlightsByDepartureDestinationAndDepartureDate(int departureAirportId, int destinationAirportId, DateOnly departureDate)
        {
            var flights = await _context.Flights
                .Where(flight =>
                       flight.DeparturePort == departureAirportId &&
                       flight.ArrivalPort == destinationAirportId &&
                       DateOnly.FromDateTime(flight.DepartureTime) == departureDate
                    )
                .Include(flight => flight.FlightsAirline)
                .Include(flight => flight.DeparturePortNavigation)
                .Include(flight => flight.ArrivalPortNavigation)
                .ToListAsync();
            return flights;
        }

        public async Task<List<Flight>> GetFlightsByAirplaneId(int airplaneId)
        {
            var flights = await _context.Flights
                .Where(flight => flight.FlightsAirplaneId == airplaneId)
                .ToListAsync();

            return flights;
        }

        public async Task<FlightClass?> GetFlightClassById(int id)
        {
            var flightClass = await _context.FlightClasses.FindAsync(id);
            return flightClass;
        }
    }
}
