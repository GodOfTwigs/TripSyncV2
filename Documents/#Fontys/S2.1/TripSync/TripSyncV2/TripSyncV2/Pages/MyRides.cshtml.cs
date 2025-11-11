using BusinessLogic;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TripSyncV2.Pages
{
    public class MyRidesModel : PageModel
    {
        private readonly RideService _rideService;

        public List<Ride> OfferedRides { get; set; } = new();
        public List<Ride> JoinedRides { get; set; } = new();

        public MyRidesModel(RideService rideService)
        {
            _rideService = rideService;
        }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            OfferedRides = _rideService.GetRidesByDriver(userId.Value);
            JoinedRides = _rideService.GetRidesJoinedByUser(userId.Value);

            return Page();
        }
    }
}
