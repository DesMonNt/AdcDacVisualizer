using System;
using Avalonia;
using Avalonia.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using System.Linq;
using AdcDacConversion.Model;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace AdcDacConversion;

using ChartPoint = Graphics.ChartPoint;

public partial class MainWindow : Window
{
    private const int BitDepth = 4;
    private const double ReferenceVoltage = 5d;
    
    private Model.Model Model { get; }
    private ObservableCollection<ISeries> AdcSeries { get; }
    
    public MainWindow()
    {
        InitializeComponent();

        Model = new Model.Model(BitDepth, ReferenceVoltage);
        AdcSeries = new ObservableCollection<ISeries>();
        AdcChart.Series = AdcSeries;
        ComparatorsGrid = this.FindControl<Grid>("ComparatorsGrid");
        ResistorsGrid = this.FindControl<Grid>("ResistorsGrid");
        VoltageSlider.PropertyChanged += VoltageSlider_ValueChanged;
        
        UpdateComparatorsLamps();
        UpdateResistorsLamps();
    }

    private void VoltageSlider_ValueChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name != "Value")
            return;

        Model.Voltage = VoltageSlider.Value;
        
        VoltageLabel.Text = $"Voltage: {Model.Voltage:F2}V";
        AdcValueLabel.Text = $"Digital Value: {Convert.ToString(Model.DigitalValue, 2)}";
        DacValueLabel.Text = $"Analog Voltage: {Model.AnalogVoltage:F2}V";

        UpdateCharts(Model.Voltage);
        UpdateComparatorsLamps();
        UpdateResistorsLamps();
    }

    private void UpdateCharts(double voltage)
    {
        AdcSeries.Clear();

        var color = new SolidColorPaint(SKColors.Yellow) { StrokeThickness = 5 };
        var converter = new AdcConverter(BitDepth, ReferenceVoltage);
        var voltageRange = Enumerable.Range(0, 101).Select(i => i * voltage / 100.0).ToList();
        var values = voltageRange.Select(v => new ChartPoint(v,converter.Convert(v))).ToList();
        
        var lineSeries = new LineSeries<ChartPoint>
        {
            Values = values,
            Mapping = (point, _) => new Coordinate(point.Value, point.Voltage),
            Fill = null,
            GeometrySize = 1,
            Stroke = color,
            GeometryStroke = color
        };

        AdcSeries.Add(lineSeries);
    }

    private void UpdateResistorsLamps()
    {
        var binaryValue = Convert.ToString(Model.DigitalValue, 2)
            .PadLeft(4, '0')
            .Select(x => x == '1')
            .ToArray();

        for (var i = 0; i < BitDepth; i++)
        {
            var lamp = new Border
            {
                Width = 20,
                Height = 20,
                BorderThickness = new Thickness(1),
                BorderBrush = Avalonia.Media.Brushes.Black,
                Background = binaryValue[i] ? Avalonia.Media.Brushes.Yellow : Avalonia.Media.Brushes.Gray,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            
            ResistorsGrid.Children.Add(lamp);
            Grid.SetRow(lamp, i);
        }
    }
    private void UpdateComparatorsLamps()
    {
        ComparatorsGrid.Children.Clear();

        var length = Model.Comparators.Count;
        
        for (var i = 0; i < length; i++)
        {
            var lamp = new Border
            {
                Width = 20,
                Height = 20,
                BorderThickness = new Thickness(1),
                BorderBrush = Avalonia.Media.Brushes.Black,
                Background = Model.Comparators[i] ? Avalonia.Media.Brushes.Yellow : Avalonia.Media.Brushes.Gray,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            
            ComparatorsGrid.Children.Add(lamp);
            Grid.SetRow(lamp, i);
        }
    }
}