using System.Collections.Generic;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace AdcDacConversion.Graphics;

internal static class LiveChartExtension
{
    public static LineSeries<ChartPoint> GetLineSeries(List<ChartPoint> points, SolidColorPaint color) => new() 
    {
        Values = points,
        Mapping = (point, _) => new Coordinate(point.Value, point.Voltage),
        Fill = null,
        GeometrySize = 1,
        Stroke = color,
        GeometryStroke = color
    };
}