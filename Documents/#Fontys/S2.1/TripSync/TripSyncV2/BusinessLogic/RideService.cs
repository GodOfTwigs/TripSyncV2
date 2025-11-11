using DataAccess;
using DataAccess.Repositories;
using Domain;

namespace BusinessLogic
{
    public class RideService
    {
        private readonly RideRepository _rideRepo;

        public RideService(RideRepository rideRepo)
        {
            _rideRepo = rideRepo;
        }

        public bool OfferRide(int driverId, string from, string to, DateTime departureTime, int seats, decimal price, out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                error = "Start and destination are required.";
                return false;
            }

            if (seats <= 0)
            {
                error = "You must offer at least one seat.";
                return false;
            }

            if (price < 0)
            {
                error = "Price must be positive.";
                return false;
            }

            try
            {
                var ride = new Ride
                {
                    driver_id = driverId,
                    start_point = from,
                    destination = to,
                    departure_time = departureTime,
                    available_seats = seats,
                    estimated_cost = price
                };

                _rideRepo.InsertRide(ride);
                return true;
            }
            catch (Exception ex)
            {
                error = $"Database error: {ex.Message}";
                return false;
            }
        }

        public bool JoinRide(int rideId, int userId)
        {
            // You could add business logic here, e.g.:
            // - Check if seats are available
            // - Prevent users from joining their own ride
            return _rideRepo.JoinRide(rideId, userId);
        }

        public List<Ride> GetRidesByDriver(int driverId)
        {
            return _rideRepo.GetRidesByDriver(driverId);
        }

        public List<Ride> GetRidesJoinedByUser(int userId)
        {
            return _rideRepo.GetRidesJoinedByUser(userId);
        }


        public List<Ride> GetAllRides()
        {
            return _rideRepo.GetAllRides();
        }
    }
}
