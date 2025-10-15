using LocationTrackingApp.Models;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace LocationTrackingApp.Helpers
{
    public static class HeatMapHelper
    {
        public static List<Circle> CreateHeatMapCircles(List<LocationPoint> locationPoints, double radius = 50)
        {
            var circles = new List<Circle>();

            if (locationPoints == null || !locationPoints.Any())
                return circles;

            // Group nearby points to create intensity-based visualization
            var groupedPoints = GroupNearbyPoints(locationPoints, radius);

            foreach (var group in groupedPoints)
            {
                var center = CalculateCenterPoint(group);
                var intensity = group.Count;
                
                // Create circle with color based on intensity
                var circle = new Circle
                {
                    Center = new Location(center.Latitude, center.Longitude),
                    Radius = Distance.FromMeters(radius),
                    StrokeColor = GetIntensityColor(intensity, false),
                    FillColor = GetIntensityColor(intensity, true),
                    StrokeWidth = 2
                };

                circles.Add(circle);
            }

            return circles;
        }

        private static List<List<LocationPoint>> GroupNearbyPoints(List<LocationPoint> points, double radiusMeters)
        {
            var groups = new List<List<LocationPoint>>();
            var processed = new HashSet<LocationPoint>();

            foreach (var point in points)
            {
                if (processed.Contains(point))
                    continue;

                var group = new List<LocationPoint> { point };
                processed.Add(point);

                // Find nearby points
                foreach (var otherPoint in points)
                {
                    if (processed.Contains(otherPoint))
                        continue;

                    var distance = CalculateDistance(point, otherPoint);
                    if (distance <= radiusMeters)
                    {
                        group.Add(otherPoint);
                        processed.Add(otherPoint);
                    }
                }

                groups.Add(group);
            }

            return groups;
        }

        private static LocationPoint CalculateCenterPoint(List<LocationPoint> points)
        {
            var avgLat = points.Average(p => p.Latitude);
            var avgLon = points.Average(p => p.Longitude);

            return new LocationPoint
            {
                Latitude = avgLat,
                Longitude = avgLon
            };
        }

        private static double CalculateDistance(LocationPoint point1, LocationPoint point2)
        {
            var location1 = new Location(point1.Latitude, point1.Longitude);
            var location2 = new Location(point2.Latitude, point2.Longitude);
            
            return Location.CalculateDistance(location1, location2, DistanceUnits.Kilometers) * 1000; // Convert to meters
        }

        private static Color GetIntensityColor(int intensity, bool isFill)
        {
            // Create color based on intensity (red scale)
            var alpha = isFill ? 0.3f : 0.7f;
            
            if (intensity == 1)
                return Color.FromRgba(0, 255, 0, alpha); // Green for single points
            else if (intensity <= 3)
                return Color.FromRgba(255, 255, 0, alpha); // Yellow for low intensity
            else if (intensity <= 6)
                return Color.FromRgba(255, 165, 0, alpha); // Orange for medium intensity
            else
                return Color.FromRgba(255, 0, 0, alpha); // Red for high intensity
        }
    }
}
