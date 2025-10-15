using LocationTrackingApp.Models;

namespace LocationTrackingApp.Services
{
    public class LocationService
    {
        private readonly DatabaseService _databaseService;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isTracking = false;

        public LocationService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public bool IsTracking => _isTracking;

        public async Task<bool> StartTrackingAsync()
        {
            if (_isTracking) return true;

            try
            {
                var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    await Application.Current.MainPage.DisplayAlert("Permission Required", 
                        "Location permission is required for tracking.", "OK");
                    return false;
                }

                _cancellationTokenSource = new CancellationTokenSource();
                _isTracking = true;

                // Start background location tracking
                _ = Task.Run(async () => await TrackLocationAsync(_cancellationTokenSource.Token));

                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", 
                    $"Failed to start tracking: {ex.Message}", "OK");
                return false;
            }
        }

        public async Task StopTrackingAsync()
        {
            if (!_isTracking) return;

            _cancellationTokenSource?.Cancel();
            _isTracking = false;
        }

        private async Task TrackLocationAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var request = new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Best,
                        Timeout = TimeSpan.FromSeconds(10)
                    };

                    var location = await Geolocation.GetLocationAsync(request, cancellationToken);

                    if (location != null)
                    {
                        var locationPoint = new LocationPoint
                        {
                            Latitude = location.Latitude,
                            Longitude = location.Longitude,
                            Timestamp = DateTime.UtcNow,
                            Accuracy = location.Accuracy ?? 0,
                            Speed = location.Speed ?? 0
                        };

                        await _databaseService.SaveLocationPointAsync(locationPoint);
                    }

                    // Wait for 5 seconds before next location update
                    await Task.Delay(5000, cancellationToken);
                }
                catch (Exception ex)
                {
                    // Log error but continue tracking
                    System.Diagnostics.Debug.WriteLine($"Location tracking error: {ex.Message}");
                    await Task.Delay(5000, cancellationToken);
                }
            }
        }

        public async Task<Location> GetCurrentLocationAsync()
        {
            try
            {
                var request = new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(10)
                };

                return await Geolocation.GetLocationAsync(request);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", 
                    $"Failed to get current location: {ex.Message}", "OK");
                return null;
            }
        }
    }
}
