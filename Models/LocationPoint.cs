using SQLite;

namespace LocationTrackingApp.Models
{
    [Table("LocationPoints")]
    public class LocationPoint
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public double Accuracy { get; set; }
        public double Speed { get; set; }
    }
}
