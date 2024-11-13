using System.Collections.Generic;
using AdcDacConversion.Infrastructure.Graphics;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using SkiaSharp;

namespace AdcDacConversion.Domain.Entities;

public class Graphic(GraphicsType type, SKColor color, int capacity)
{
    private readonly List<ObservableValue> _values = [];

    public void Add(double value)
    {
        if (_values.Count > capacity)
            _values.RemoveAt(0);
        
        _values.Add(new ObservableValue(value));
    }

    public void Clear() => _values.Clear();

    public ISeries[] ToSeries() => LiveChartExtension.GetSeries(_values, type, color);
}