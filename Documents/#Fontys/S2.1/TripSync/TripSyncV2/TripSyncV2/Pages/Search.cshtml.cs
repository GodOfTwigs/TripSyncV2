using BusinessLogic;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TripSyncV2.Pages
{
    public class SearchModel : PageModel
    {
        private readonly RideService _rideService;

        public List<Ride> Rides { get; set; }
        public string FeedbackMessage { get; set; }

        public SearchModel(RideService rideService)
        {
            _rideService = rideService;
        }

        public void OnGet()
        {
            Rides = _rideService.GetAllRides();
        }

        public IActionResult OnPostJoinRide(int rideId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                FeedbackMessage = "You must be logged in to join a ride.";
                Rides = _rideService.GetAllRides();
                return Page();
            }

            bool success = _rideService.JoinRide(rideId, userId.Value);

            FeedbackMessage = success
                ? "You have successfully joined the ride!"
                : "You already joined this ride or something went wrong.";

            Rides = _rideService.GetAllRides();
            return Page();
        }
    }
}
