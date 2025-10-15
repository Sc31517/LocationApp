using SQLite;
using LocationTrackingApp.Models;

namespace LocationTrackingApp.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "LocationTracking.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<LocationPoint>().Wait();
        }

        public async Task<List<LocationPoint>> GetLocationPointsAsync()
        {
            return await _database.Table<LocationPoint>().ToListAsync();
        }

        public Task<List<LocationPoint>> GetLocationPointsInAreaAsync(double minLat, double maxLat, double minLon, double maxLon)
        {
            return _database.Table<LocationPoint>()
                .Where(lp => lp.Latitude >= minLat && lp.Latitude <= maxLat &&
                            lp.Longitude >= minLon && lp.Longitude <= maxLon)
                .ToListAsync();
        }

        public Task<int> SaveLocationPointAsync(LocationPoint locationPoint)
        {
            return _database.InsertAsync(locationPoint);
        }

        public Task<int> DeleteLocationPointAsync(LocationPoint locationPoint)
        {
            return _database.DeleteAsync(locationPoint);
        }

        public async Task<int> ClearAllLocationPointsAsync()
        {
            return await _database.DeleteAllAsync<LocationPoint>();
        }

        public Task<int> GetLocationPointCountAsync()
        {
            return _database.Table<LocationPoint>().CountAsync();
        }
    }
}
