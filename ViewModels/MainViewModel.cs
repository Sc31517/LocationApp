using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using LocationTrackingApp.Models;
using LocationTrackingApp.Services;
using LocationTrackingApp.Controls;
using LocationTrackingApp.Helpers;

namespace LocationTrackingApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly LocationService _locationService;
        private readonly DatabaseService _databaseService;
        private bool _isTracking;
        private string _trackingButtonText = "Start Tracking";
        private int _locationPointCount;
        private ObservableCollection<LocationPoint> _locationPoints;
        private HeatMapOverlay _heatMapOverlay;

        public MainViewModel(LocationService locationService, DatabaseService databaseService)
        {
            _locationService = locationService;
            _databaseService = databaseService;
            _locationPoints = new ObservableCollection<LocationPoint>();
            _heatMapOverlay = new HeatMapOverlay();

            StartStopTrackingCommand = new Command(async () => await OnStartStopTrackingAsync());
            ClearDataCommand = new Command(async () => await OnClearDataAsync());
            RefreshMapCommand = new Command(async () => await OnRefreshMapAsync());

            LoadLocationPointsAsync();
        }

        public bool IsTracking
        {
            get => _isTracking;
            set
            {
                _isTracking = value;
                OnPropertyChanged();
                TrackingButtonText = _isTracking ? "Stop Tracking" : "Start Tracking";
            }
        }

        public string TrackingButtonText
        {
            get => _trackingButtonText;
            set
            {
                _trackingButtonText = value;
                OnPropertyChanged();
            }
        }

        public int LocationPointCount
        {
            get => _locationPointCount;
            set
            {
                _locationPointCount = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LocationPoint> LocationPoints
        {
            get => _locationPoints;
            set
            {
                _locationPoints = value;
                OnPropertyChanged();
            }
        }

        public HeatMapOverlay HeatMapOverlay
        {
            get => _heatMapOverlay;
            set
            {
                _heatMapOverlay = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartStopTrackingCommand { get; }
        public ICommand ClearDataCommand { get; }
        public ICommand RefreshMapCommand { get; }

        private async Task OnStartStopTrackingAsync()
        {
            if (IsTracking)
            {
                await _locationService.StopTrackingAsync();
                IsTracking = false;
            }
            else
            {
                var success = await _locationService.StartTrackingAsync();
                IsTracking = success;
            }
        }

        private async Task OnClearDataAsync()
        {
            var result = await Application.Current.MainPage.DisplayAlert(
                "Clear Data", 
                "Are you sure you want to clear all location data?", 
                "Yes", "No");

            if (result)
            {
                await _databaseService.ClearAllLocationPointsAsync();
                await LoadLocationPointsAsync();
                await OnRefreshMapAsync();
            }
        }

        private async Task OnRefreshMapAsync()
        {
            await LoadLocationPointsAsync();
            HeatMapOverlay.LocationPoints = LocationPoints.ToList();
        }

        private async Task LoadLocationPointsAsync()
        {
            try
            {
                var points = await _databaseService.GetLocationPointsAsync();
                LocationPoints.Clear();
                
                foreach (var point in points)
                {
                    LocationPoints.Add(point);
                }

                LocationPointCount = points.Count;
                HeatMapOverlay.LocationPoints = points;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", 
                    $"Failed to load location points: {ex.Message}", "OK");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
