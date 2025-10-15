using Microsoft.Extensions.Logging;
using LocationTrackingApp.Services;
using LocationTrackingApp.ViewModels;
using LocationTrackingApp.Controls;

#if ANDROID
using LocationTrackingApp.Platforms.Android;
#elif IOS
using LocationTrackingApp.Platforms.iOS;
#endif

namespace LocationTrackingApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiMaps()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.ConfigureMauiHandlers(handlers =>
			{
#if ANDROID
				handlers.AddHandler<HeatMapOverlay, HeatMapOverlayHandler>();
#elif IOS
				handlers.AddHandler<HeatMapOverlay, HeatMapOverlayHandler>();
#endif
			});

		// Register services
		builder.Services.AddSingleton<DatabaseService>();
		builder.Services.AddSingleton<LocationService>();
		builder.Services.AddSingleton<MainViewModel>();
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<App>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
