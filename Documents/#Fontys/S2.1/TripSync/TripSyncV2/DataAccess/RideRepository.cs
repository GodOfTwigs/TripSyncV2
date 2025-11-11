using System.Collections.Generic;
using System.Data.SqlClient;
using Domain;

namespace DataAccess.Repositories
{
    public class RideRepository
    {
        private readonly string _connectionString;

        public RideRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Get ALL Rides
        public List<Ride> GetAllRides()
        {
            var rides = new List<Ride>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"
                SELECT 
                    ride_id,
                    driver_id,
                    start_point,
                    destination,
                    departure_time,
                    available_seats,
                    estimated_cost
                FROM rides";

            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                rides.Add(new Ride
                {
                    ride_id = Convert.ToInt32(reader["ride_id"]),
                    driver_id = Convert.ToInt32(reader["driver_id"]),
                    start_point = reader.GetString(reader.GetOrdinal("start_point")),
                    destination = reader.GetString(reader.GetOrdinal("destination")),
                    departure_time = reader.GetDateTime(reader.GetOrdinal("departure_time")),
                    available_seats = Convert.ToInt32(reader["available_seats"]),
                    estimated_cost = reader.GetDecimal(reader.GetOrdinal("estimated_cost"))
                });
            }

            return rides;
        }

        // Add A Ride
        public void InsertRide(Ride ride)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"
                INSERT INTO Rides (driver_id, start_point, destination, departure_time, available_seats, estimated_cost)
                VALUES (@driver_id, @start_point, @destination, @departure_time, @available_seats, @estimated_cost)";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@driver_id", ride.driver_id);
            cmd.Parameters.AddWithValue("@start_point", ride.start_point);
            cmd.Parameters.AddWithValue("@destination", ride.destination);
            cmd.Parameters.AddWithValue("@departure_time", ride.departure_time);
            cmd.Parameters.AddWithValue("@available_seats", ride.available_seats);
            cmd.Parameters.AddWithValue("@estimated_cost", ride.estimated_cost);

            cmd.ExecuteNonQuery();
        }

        public bool JoinRide(int rideId, int passengerId)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string checkQuery = @"
            SELECT COUNT(*) FROM ride_passengers
            WHERE ride_id = @rideId AND passenger_id = @passengerId";

            using var checkCmd = new SqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@rideId", rideId);
            checkCmd.Parameters.AddWithValue("@passengerId", passengerId);

            int alreadyJoined = (int)checkCmd.ExecuteScalar();
            if (alreadyJoined > 0)
                return false; // already joined

            string insertQuery = @"
            INSERT INTO ride_passengers (ride_id, passenger_id)
            VALUES (@rideId, @passengerId)";

            using var insertCmd = new SqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@rideId", rideId);
            insertCmd.Parameters.AddWithValue("@passengerId", passengerId);

            insertCmd.ExecuteNonQuery();
            return true;
        }

        public List<Ride> GetRidesByDriver(int driverId)
        {
            var rides = new List<Ride>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"SELECT * FROM Rides WHERE driver_id = @driverId";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@driverId", driverId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rides.Add(MapRide(reader));
            }

            return rides;
        }

        public List<Ride> GetRidesJoinedByUser(int userId)
        {
            var rides = new List<Ride>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = @"
        SELECT r.*
        FROM ride_passengers rp
        JOIN Rides r ON rp.ride_id = r.ride_id
        WHERE rp.passenger_id = @userId";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                rides.Add(MapRide(reader));
            }

            return rides;
        }

        private Ride MapRide(SqlDataReader reader)
        {
            return new Ride
            {
                ride_id = Convert.ToInt32(reader["ride_id"]),
                start_point = reader["start_point"].ToString(),
                destination = reader["destination"].ToString(),
                departure_time = Convert.ToDateTime(reader["departure_time"]),
                available_seats = Convert.ToInt32(reader["available_seats"]),
                estimated_cost = Convert.ToDecimal(reader["estimated_cost"]),
                driver_id = Convert.ToInt32(reader["driver_id"])
            };
        }
    }
}
