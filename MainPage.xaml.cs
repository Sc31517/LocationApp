using LocationTrackingApp.ViewModels;
using LocationTrackingApp.Services;
using LocationTrackingApp.Helpers;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Maps;
using System.Collections.Specialized;

namespace LocationTrackingApp;

public partial class MainPage : ContentPage
{
	private readonly MainViewModel _viewModel;

	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
		
		Loaded += MainPage_Loaded;
		
		// Subscribe to location points changes
		_viewModel.LocationPoints.CollectionChanged += OnLocationPointsChanged;
		_viewModel.PropertyChanged += OnViewModelPropertyChanged;
	}

	private async void MainPage_Loaded(object? sender, EventArgs e)
	{
		await SetInitialMapLocation();
	}

	private async Task SetInitialMapLocation()
	{
		try
		{
			var location = await Geolocation.GetLocationAsync(new GeolocationRequest
			{
				DesiredAccuracy = GeolocationAccuracy.Best,
				Timeout = TimeSpan.FromSeconds(10)
			});

			if (location != null)
			{
				var mapSpan = MapSpan.FromCenterAndRadius(
					new Location(location.Latitude, location.Longitude),
					Distance.FromKilometers(1));
				
				LocationMap.MoveToRegion(mapSpan);
			}
			else
			{
				// Default to San Francisco if location is not available
				var mapSpan = MapSpan.FromCenterAndRadius(
					new Location(37.7749, -122.4194),
					Distance.FromKilometers(10));
				
				LocationMap.MoveToRegion(mapSpan);
			}
		}
		catch (Exception ex)
		{
			// Default to San Francisco if there's an error
			var mapSpan = MapSpan.FromCenterAndRadius(
				new Location(37.7749, -122.4194),
				Distance.FromKilometers(10));
			
			LocationMap.MoveToRegion(mapSpan);
		}
	}

	private void OnLocationPointsChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		UpdateHeatMap();
	}

	private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(_viewModel.LocationPointCount))
		{
			UpdateHeatMap();
		}
	}

	private void UpdateHeatMap()
	{
		// Clear existing map elements
		LocationMap.MapElements.Clear();

		if (_viewModel.LocationPoints?.Any() == true)
		{
			// Create heat map circles
			var circles = HeatMapHelper.CreateHeatMapCircles(_viewModel.LocationPoints.ToList());

			// Add circles to map
			foreach (var circle in circles)
			{
				LocationMap.MapElements.Add(circle);
			}
		}
	}
}
