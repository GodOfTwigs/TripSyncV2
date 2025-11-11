namespace Domain
{
    public class Ride
    {
        public int ride_id { get; set; }
        public int driver_id { get; set; }
        public string start_point { get; set; }
        public string destination { get; set; }
        public DateTime departure_time { get; set; }
        public int available_seats { get; set; }
        public int total_seats { get; set; }
        public decimal estimated_cost { get; set; }
    }
}
