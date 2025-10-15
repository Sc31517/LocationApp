#if IOS
using CoreLocation;
using LocationTrackingApp.Controls;
using MapKit;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Maps;
using UIKit;

namespace LocationTrackingApp.Platforms.iOS
{
    public class HeatMapOverlayHandler : Microsoft.Maui.Maps.Handlers.MapElementHandler<HeatMapOverlay>
    {
        protected override void ConnectHandler(object? platformView)
        {
            base.ConnectHandler(platformView);
            UpdateHeatMap();
        }

        public static void MapLocationPoints(IMapElementHandler handler, IMapElement mapElement)
        {
            if (handler is HeatMapOverlayHandler heatMapHandler)
                heatMapHandler.UpdateHeatMap();
        }

        public static void MapRadius(IMapElementHandler handler, IMapElement mapElement)
        {
            if (handler is HeatMapOverlayHandler heatMapHandler)
                heatMapHandler.UpdateHeatMap();
        }

        public static void MapIntensity(IMapElementHandler handler, IMapElement mapElement)
        {
            if (handler is HeatMapOverlayHandler heatMapHandler)
                heatMapHandler.UpdateHeatMap();
        }

        private void UpdateHeatMap()
        {
            if (VirtualView?.LocationPoints == null || !VirtualView.LocationPoints.Any())
                return;

            var mapView = Handler?.PlatformView as MKMapView;
            if (mapView == null) return;

            // Remove existing overlays
            mapView.RemoveOverlays(mapView.Overlays);

            // Create circles for heat map visualization
            foreach (var point in VirtualView.LocationPoints)
            {
                var coordinate = new CLLocationCoordinate2D(point.Latitude, point.Longitude);
                var circle = MKCircle.Circle(coordinate, VirtualView.Radius);
                mapView.AddOverlay(circle);
            }
        }
    }

    public class HeatMapCircleRenderer : MKOverlayRenderer
    {
        public override void DrawMapRect(MKMapRect mapRect, nfloat zoomScale, CoreGraphics.CGContext context)
        {
            var circle = Overlay as MKCircle;
            if (circle == null) return;

            var rect = RectForMapRect(mapRect);
            context.SetFillColor(UIColor.Red.ColorWithAlpha(0.2f).CGColor);
            context.SetStrokeColor(UIColor.Red.ColorWithAlpha(0.4f).CGColor);
            context.SetLineWidth(2.0f / zoomScale);

            context.FillEllipseInRect(rect);
            context.StrokeEllipseInRect(rect);
        }
    }
}
#endif
