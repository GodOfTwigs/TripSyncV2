using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace TripSync.Pages.Rides
{
    public class SearchModel : PageModel
    {
        private readonly IConfiguration _config;

        public SearchModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty(SupportsGet = true)]
        public string From { get; set; }

        [BindProperty(SupportsGet = true)]
        public string To { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime Date { get; set; }

        public List<Ride> TopMatches { get; set; } = new();
        public List<Ride> OtherRides { get; set; } = new();

        public void OnGet()
        {
            var rides = GetAllRides();

            var scoredRides = rides
                .Select(ride =>
                {
                    int score = 0;

                    if (!string.IsNullOrWhiteSpace(From) &&
                        ride.start_point.Contains(From, StringComparison.OrdinalIgnoreCase))
                        score += 2;

                    if (!string.IsNullOrWhiteSpace(To) &&
                        ride.destination.Contains(To, StringComparison.OrdinalIgnoreCase))
                        score += 2;

                    if (Date != default && ride.departure_time.Date == Date.Date)
                        score += 1;

                    return new { Ride = ride, Score = score };
                })
                .OrderByDescending(x => x.Score)
                .ToList();

            TopMatches = scoredRides.Where(x => x.Score >= 4).Select(x => x.Ride).ToList();
            OtherRides = scoredRides.Where(x => x.Score < 4).Select(x => x.Ride).ToList();
        }

        private List<Ride> GetAllRides()
        {
            var rides = new List<Ride>();
            string connStr = _config.GetConnectionString("TripSyncDb");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = @"SELECT ride_id, driver_id, start_point, destination, departure_time, available_seats, estimated_cost 
                                 FROM Rides";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rides.Add(new Ride
                        {
                            ride_id = Convert.ToInt32(reader["ride_id"]),
                            driver_id = Convert.ToInt32(reader["driver_id"]),
                            start_point = reader.GetString(2),
                            destination = reader.GetString(3),
                            departure_time = reader.GetDateTime(4),
                            available_seats = reader.GetInt32(5),
                            estimated_cost = reader.GetDecimal(6)
                        });
                    }
                }
            }

            return rides;
        }

        public class Ride
        {
            public int ride_id { get; set; }
            public int driver_id { get; set; }
            public string start_point { get; set; }
            public string destination { get; set; }
            public DateTime departure_time { get; set; }
            public int available_seats { get; set; }
            public decimal estimated_cost { get; set; }
        }
    }
}
