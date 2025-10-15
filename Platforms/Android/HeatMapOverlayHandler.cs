#if ANDROID
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using LocationTrackingApp.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Maps;
using AndroidX.Core.Content;
using Android.Graphics;

namespace LocationTrackingApp.Platforms.Android
{
    public class HeatMapOverlayHandler : Microsoft.Maui.Maps.Handlers.MapElementHandler
    {
        public new static void MapLocationPoints(IMapElementHandler handler, IMapElement mapElement)
        {
            if (handler is HeatMapOverlayHandler heatMapHandler)
                heatMapHandler.UpdateHeatMap();
        }

        public new static void MapRadius(IMapElementHandler handler, IMapElement mapElement)
        {
            if (handler is HeatMapOverlayHandler heatMapHandler)
                heatMapHandler.UpdateHeatMap();
        }

        public new static void MapIntensity(IMapElementHandler handler, IMapElement mapElement)
        {
            if (handler is HeatMapOverlayHandler heatMapHandler)
                heatMapHandler.UpdateHeatMap();
        }

        private void UpdateHeatMap()
        {
            var heatMapOverlay = VirtualView as HeatMapOverlay;
            if (heatMapOverlay?.LocationPoints == null || !heatMapOverlay.LocationPoints.Any())
                return;

            var googleMap = PlatformView as GoogleMap;
            if (googleMap == null) return;

            // Clear existing overlays
            googleMap.Clear();

            // Create circles for heat map visualization
            foreach (var point in heatMapOverlay.LocationPoints)
            {
                var circleOptions = new CircleOptions()
                    .InvokeCenter(new LatLng(point.Latitude, point.Longitude))
                    .InvokeRadius(heatMapOverlay.Radius)
                    .InvokeFillColor(global::Android.Graphics.Color.Argb(50, 255, 0, 0)) // Semi-transparent red
                    .InvokeStrokeColor(global::Android.Graphics.Color.Argb(100, 255, 0, 0))
                    .InvokeStrokeWidth(2);

                googleMap.AddCircle(circleOptions);
            }
        }
    }
}
#endif
