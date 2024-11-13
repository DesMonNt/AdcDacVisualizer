using System;
using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace AdcDacConversion.Infrastructure.Graphics;

public static class LiveChartExtension
{
    public static ISeries[] GetSeries(List<ObservableValue> values, GraphicsType type, SKColor color) => type switch
    {
        GraphicsType.Line =>
        [
            new LineSeries<ObservableValue>
            {
                Values = values,
                Stroke = new SolidColorPaint(color) { StrokeThickness = 5 },
                GeometryStroke = new SolidColorPaint(color),
                GeometrySize = 0,
                Fill = new SolidColorPaint(SKColors.Empty),
                AnimationsSpeed = TimeSpan.Zero,
                LineSmoothness = 0
            }
        ],
        GraphicsType.StepLine =>
        [
            new StepLineSeries<ObservableValue>
            {
                Values = values,
                Stroke = new SolidColorPaint(color) { StrokeThickness = 5 },
                GeometryStroke = new SolidColorPaint(color),
                GeometrySize = 0,
                Fill = new SolidColorPaint(SKColors.Empty),
                AnimationsSpeed = TimeSpan.Zero,
            }
        ],
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };
}