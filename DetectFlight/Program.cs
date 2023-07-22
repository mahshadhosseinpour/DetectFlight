using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        //if (args.Length != 3)
        //{
        //    Console.WriteLine("Usage: YourProgram.exe <start_date> <end_date> <agency_id>");
        //    return;
        //}

        DateTime startDate = DateTime.Parse("2018-01-01");//DateTime.Parse(args[0]);
        DateTime endDate = DateTime.Parse("2018-01-18"); //DateTime.Parse(args[1]);
        int agencyId = 1; //int.Parse(args[2]);

        // Initialize data context
        using (var context = new DataContext())
        {
            // Get the subscriptions for the given agencyId
            var subscriptions = context.Subscriptions
                .Where(sub => sub.agency_id == agencyId)
                .ToList();

            // Calculate the date ranges for filtering the flights
            DateTime startDateMinus7Days = startDate.AddDays(-7).AddMinutes(-30);
            DateTime startDatePlus30Minutes = startDate.AddMinutes(30);
            DateTime endDateMinus30Minutes = endDate.AddMinutes(-30);
            DateTime endDatePlus7DaysPlus30Minutes = endDate.AddDays(7).AddMinutes(30);

            // Retrieve all flights and routes from the database
            var flights = context.Flights.ToList();
            var routes = context.Routes.ToList();

            // Filter the newFlights based on the subscriptions and date ranges
            var newFlights = flights
                .Where(flight =>
                    subscriptions.Any(sub =>
                        flight.departure_time >= startDateMinus7Days &&
                        flight.departure_time <= startDatePlus30Minutes &&
                        routes.Any(route =>
                            route.route_id == flight.route_id &&
                            route.origin_city_id == sub.origin_city_id &&
                            route.destination_city_id == sub.destination_city_id)))
                .ToList();

            // Filter the discontinuedFlights based on the subscriptions and date ranges
            var discontinuedFlights = flights
                .Where(flight =>
                    subscriptions.Any(sub =>
                        flight.departure_time >= endDateMinus30Minutes &&
                        flight.departure_time <= endDatePlus7DaysPlus30Minutes &&
                        routes.Any(route =>
                            route.route_id == flight.route_id &&
                            route.origin_city_id == sub.origin_city_id &&
                            route.destination_city_id == sub.destination_city_id)))
                .ToList();

            // Output results to CSV file
            using (var writer = new StreamWriter("results.csv"))
            {
                writer.WriteLine("flight_id,origin_city_id,destination_city_id,departure_time,arrival_time,airline_id,status");
                foreach (var flight in newFlights)
                {
                    writer.WriteLine($"{flight.flight_id},{routes.First(r => r.route_id == flight.route_id).origin_city_id},{routes.First(r => r.route_id == flight.route_id).destination_city_id}," +
                                     $"{flight.departure_time},{flight.arrival_time},{flight.airline_id},New");
                }
                foreach (var flight in discontinuedFlights)
                {
                    writer.WriteLine($"{flight.flight_id},{routes.First(r => r.route_id == flight.route_id).origin_city_id},{routes.First(r => r.route_id == flight.route_id).destination_city_id}," +
                                     $"{flight.departure_time},{flight.arrival_time},{flight.airline_id},Discontinued");
                }
            }
        }

    }
}
