using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace AdcDacConversion.Infrastructure.Graphics;

public static class UiUtilities
{
    public static Ellipse CreateLed() => new()
    {
        Width = 30,
        Height = 30,
        Fill = Brushes.Gray,
        Margin = new Thickness(5)
    };
}