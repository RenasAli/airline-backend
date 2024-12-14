// Create airlines
MERGE (delta:Airline {name: "Delta Airlines"});
MERGE (united:Airline {name: "United Airlines"});
MERGE (american:Airline {name: "American Airlines"});

// Create airplanes and link to airlines
MERGE (b737:Airplane {name: "Boeing 737", economy_class_seats: 140, business_class_seats: 40, first_class_seats: 5})-[:OWNED_BY]->(delta);
MERGE (a320:Airplane {name: "Airbus A320", economy_class_seats: 120, business_class_seats: 22, first_class_seats: 20})-[:OWNED_BY]->(united);
MERGE (b777:Airplane {name: "Boeing 777", economy_class_seats: 120, business_class_seats: 12, first_class_seats: 20})-[:OWNED_BY]->(american);

// Create states
MERGE (ca:State {name: "California", code: "CA"});
MERGE (ny:State {name: "New York", code: "NY"});
MERGE (tx:State {name: "Texas", code: "TX"});

// Create cities and link to states
MERGE (la:City {name: "Los Angeles"})-[:IN_STATE]->(ca);
MERGE (nyc:City {name: "New York"})-[:IN_STATE]->(ny);
MERGE (hou:City {name: "Houston"})-[:IN_STATE]->(tx);

// Create airports and link to cities
MERGE (lax:Airport {name: "Los Angeles International Airport", code: "LAX"})-[:LOCATED_IN]->(la);
MERGE (jfk:Airport {name: "John F. Kennedy International Airport", code: "JFK"})-[:LOCATED_IN]->(nyc);
MERGE (iah:Airport {name: "George Bush Intercontinental Airport", code: "IAH"})-[:LOCATED_IN]->(hou);

// Create users
MERGE (admin:User {name: "Admin1", email: "admin@example.com", password: "hashed_admin_password", role: "Admin"});
MERGE (customer1:User {name: "Customer1", email: "customer@example.com", password: "hashed_customer_password", role: "Customer"});
MERGE (customer2:User {name: "Customer2", email: "customer2@example.com", password: "hashed_customer_password", role: "Customer"});

// Create bookings and link to users
MERGE (booking1:Booking {confirmation_number: "ABC123"})-[:BELONGS_TO]->(customer1);
MERGE (booking2:Booking {confirmation_number: "DEF456"})-[:BELONGS_TO]->(customer1);
MERGE (booking3:Booking {confirmation_number: "GHI789"})-[:BELONGS_TO]->(customer2);

// Create flight classes
MERGE (economy:FlightClass {name: "EconomyClass", price_multiplier: 1.00});
MERGE (business:FlightClass {name: "BusinessClass", price_multiplier: 1.50});
MERGE (first:FlightClass {name: "FirstClass", price_multiplier: 3.00});

// Create flights and link to airports, airlines, and airplanes
MERGE (flight1:Flight {flight_code: "DL100", departure_time: "2024-12-24T08:00:00", travel_time: 360, price: 199.99, kilometers: 450, economy_class_seats_available: 150, business_class_seats_available: 20, first_class_seats_available: 5})
-[:DEPARTS_FROM]->(lax)
-[:ARRIVES_AT]->(jfk)
-[:OPERATED_BY]->(delta)
-[:USES_AIRPLANE]->(b737);

MERGE (flight2:Flight {flight_code: "DL101", departure_time: "2024-12-24T09:30:00", travel_time: 360, price: 199.99, kilometers: 450, economy_class_seats_available: 150, business_class_seats_available: 20, first_class_seats_available: 5})
-[:DEPARTS_FROM]->(jfk)
-[:ARRIVES_AT]->(iah)
-[:OPERATED_BY]->(delta)
-[:USES_AIRPLANE]->(b777);

// Link flights to bookings
MERGE (booking1)-[:CONTAINS_FLIGHT]->(flight1);
MERGE (booking2)-[:CONTAINS_FLIGHT]->(flight2);

// Create passengers and link to tickets
MERGE (jane:Passenger {first_name: "Jane", last_name: "Smith", email: "jane.smith@example.com"});
MERGE (michael:Passenger {first_name: "Michael", last_name: "Jones", email: "michael.jones@example.com"});

// Create tickets and link to passengers, flights, and flight classes
MERGE (ticket1:Ticket {price: 300.00, ticket_number: "TCK1001"})-[:FOR_FLIGHT]->(flight1)-[:CLASS_TYPE]->(economy);
MERGE (ticket2:Ticket {price: 500.00, ticket_number: "TCK1002"})-[:FOR_FLIGHT]->(flight2)-[:CLASS_TYPE]->(business);

MERGE (jane)-[:HAS_TICKET]->(ticket1);
MERGE (michael)-[:HAS_TICKET]->(ticket2);