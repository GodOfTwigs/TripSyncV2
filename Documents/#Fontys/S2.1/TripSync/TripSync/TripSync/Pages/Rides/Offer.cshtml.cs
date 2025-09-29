using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace TripSync.Pages.Rides
{
    public class OfferModel : PageModel
    {
        private readonly IConfiguration _config;

        public OfferModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty] public string From { get; set; }
        [BindProperty] public string To { get; set; }
        [BindProperty] public DateTime Date { get; set; }
        [BindProperty] public int Seats { get; set; }
        [BindProperty] public decimal Price { get; set; }

        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            //Check if user is logged in
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            //Check if user is logged in
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            //Validate form fields
            if (string.IsNullOrWhiteSpace(From) ||
                string.IsNullOrWhiteSpace(To) ||
                Date == default ||
                Seats <= 0 ||
                Price <= 0)
            {
                ErrorMessage = "Please fill out all required fields.";
                return Page();
            }

            string connStr = _config.GetConnectionString("TripSyncDb");

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    string query = @"INSERT INTO Rides 
                                    (driver_id, start_point, destination, departure_time, available_seats, estimated_cost) 
                                    VALUES (@driver_id, @from, @to, @date, @seats, @price)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@driver_id", userId.Value); //From session
                        cmd.Parameters.AddWithValue("@from", From);
                        cmd.Parameters.AddWithValue("@to", To);
                        cmd.Parameters.AddWithValue("@date", Date);
                        cmd.Parameters.AddWithValue("@seats", Seats);
                        cmd.Parameters.AddWithValue("@price", Price);

                        cmd.ExecuteNonQuery();
                    }
                }

                SuccessMessage = $"Ride successfully posted from {From} to {To} on {Date:dd-MM-yyyy HH:mm}.";
                ModelState.Clear(); //Clear the form after success
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error posting ride: " + ex.Message;
            }

            return Page();
        }
    }
}
