using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using LiveChartsCore;
using System.Collections.ObjectModel;
using System.Linq;
using AdcDacConversion.AdcDacModel;
using AdcDacConversion.Graphics;
using Avalonia.Media;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace AdcDacConversion;

public partial class MainWindow : Window
{
    private const int BitDepth = 4;
    private const double ReferenceVoltage = 5;
    
    private Model Model { get; }
    private string BinaryDigitalValue => Convert.ToString(Model.DigitalValue, 2).PadLeft(BitDepth, '0');
    private ObservableCollection<ISeries> AdcSeries { get; } = [];
    
    public MainWindow()
    {
        InitializeComponent();

        Model = new Model(BitDepth, ReferenceVoltage);
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
        AdcValueLabel.Text = $"Digital Value: {BinaryDigitalValue}";
        DacValueLabel.Text = $"Analog Voltage: {Model.AnalogVoltage:F2}V";

        UpdateCharts(Model.Voltage);
        UpdateComparatorsLamps();
        UpdateResistorsLamps();
    }

    private void UpdateCharts(double voltage)
    {
        AdcSeries.Clear();
        
        var converter = new AdcConverter(BitDepth, ReferenceVoltage);
        var perfectValues = new List<ChartPoint> { new(0, 0), new(Model.Voltage, Model.DigitalValue) };
        var realValues = Enumerable.Range(0, 101)
            .Select(i => i * voltage / 100.0)
            .Select(analogVoltage => new ChartPoint(analogVoltage, converter.Convert(analogVoltage)))
            .ToList();
        
        AdcSeries.Add(LiveChartExtension.GetLineSeries(perfectValues, new SolidColorPaint(SKColors.Orange) { StrokeThickness = 5 }));
        AdcSeries.Add(LiveChartExtension.GetLineSeries(realValues, new SolidColorPaint(SKColors.CornflowerBlue) { StrokeThickness = 5 }));
    }

    private void UpdateResistorsLamps()
    {
        for (var i = 0; i < BitDepth; i++)
        {
            var lamp = GetLamp(BinaryDigitalValue[i] == '1' ? Brushes.Yellow : Brushes.Gray);
            
            ResistorsGrid.Children.Add(lamp);
            Grid.SetRow(lamp, i);
        }
    }
    private void UpdateComparatorsLamps()
    {
        for (var i = 0; i < Model.Comparators.Count; i++)
        {
            var lamp = GetLamp(Model.Comparators[i] ? Brushes.Yellow : Brushes.Gray);
            
            ComparatorsGrid.Children.Add(lamp);
            Grid.SetRow(lamp, i);
        }
    }
    
    private static Border GetLamp(IBrush color) => new()
    {
        Width = 20,
        Height = 20,
        BorderThickness = new Thickness(1),
        BorderBrush = Brushes.Black,
        Background = color,
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
    };
}