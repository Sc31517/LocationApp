using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using LocationTrackingApp.Models;

namespace LocationTrackingApp.Controls
{
    public class HeatMapOverlay : MapElement
    {
        public static readonly BindableProperty LocationPointsProperty =
            BindableProperty.Create(nameof(LocationPoints), typeof(List<LocationPoint>), typeof(HeatMapOverlay), new List<LocationPoint>());

        public List<LocationPoint> LocationPoints
        {
            get => (List<LocationPoint>)GetValue(LocationPointsProperty);
            set => SetValue(LocationPointsProperty, value);
        }

        public static readonly BindableProperty RadiusProperty =
            BindableProperty.Create(nameof(Radius), typeof(double), typeof(HeatMapOverlay), 50.0);

        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        public static readonly BindableProperty IntensityProperty =
            BindableProperty.Create(nameof(Intensity), typeof(double), typeof(HeatMapOverlay), 1.0);

        public double Intensity
        {
            get => (double)GetValue(IntensityProperty);
            set => SetValue(IntensityProperty, value);
        }
    }
}
