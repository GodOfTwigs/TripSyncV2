using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLogic;

namespace TripSyncV2.Pages
{
    public class OfferRideModel : PageModel
    {
        private readonly RideService _rideService;

        public OfferRideModel(RideService rideService)
        {
            _rideService = rideService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Start location is required.")]
        public string From { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Destination is required.")]
        public string To { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Date and time are required.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Departure Time")]
        public DateTime Date { get; set; }

        [BindProperty]
        [Range(1, 10, ErrorMessage = "Seats must be between 1 and 10.")]
        public int Seats { get; set; }

        [BindProperty]
        [Range(1, 1000, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        public string? Message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // re-render form with validation messages
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                Message = "You must be logged in to offer a ride.";
                return Page();
            }

            var success = _rideService.OfferRide(
                userId.Value, From, To, Date, Seats, Price, out string error
            );

            Message = success ? "Ride offered successfully!" : error;
            return Page();
        }
    }
}
