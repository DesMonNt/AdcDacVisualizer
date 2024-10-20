using System;
using System.Collections.Generic;
using System.Linq;
using AdcDacConversion.AdcDacModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace AdcDacConversion;

public partial class MainWindow : Window
{
    private const int BitDepth = 4;
    private const double ReferenceVoltage = 5;

    private Model Model { get; } = new(BitDepth, ReferenceVoltage);
    private string BinaryDigitalValue => Convert.ToString(Model.DigitalValue, 2).PadLeft(BitDepth, '0');
    private DispatcherTimer Timer { get; } = new() { Interval = TimeSpan.FromMilliseconds(100) };
    
    private double _currentTime;
    private readonly List<ObservableValue> _voltageValues = [];
    private readonly List<ObservableValue> _analogVoltageValues = [];
    
    
    public MainWindow()
    {
        InitializeComponent();
        Timer.Start();
    
        for (var i = 0; i < Model.Comparators.Count; i++)
            ComparatorLedPanel.Children.Add(CreateLed());

        for (var i = 0; i < BitDepth; i++)
            ResistorLedPanel.Children.Add(CreateLed());
        
        VoltageSlider.PropertyChanged += VoltageSlider_ValueChanged;
        Timer.Tick += OnTimerElapsed;
    }

    private void VoltageSlider_ValueChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name != "Value" || sender is not Slider slider) 
            return;

        Model.Voltage = slider.Value;
    }

    private void OnCurrentTypeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CurrentTypeComboBox.SelectedItem is not ComboBoxItem selectedItem) 
            return;

        _voltageValues.Clear();
        _currentTime = 0;
        VoltageSlider.IsVisible = selectedItem.Content.ToString() == "Постоянный";
    }

    private void UpdateComparatorLedPanel()
    {
        var ledPanel = ComparatorLedPanel.Children.OfType<Ellipse>().ToArray();

        for (var i = 0; i < Model.Comparators.Count; i++)
            ledPanel[i].Fill = Model.Comparators[i] ? Brushes.Gold : Brushes.SlateGray;
    }

    private void UpdateResistorLedPanel()
    {
        var ledPanel = ResistorLedPanel.Children.OfType<Ellipse>().ToArray();

        for (var i = 0; i < BitDepth; i++)
            ledPanel[i].Fill = BinaryDigitalValue[i] == '1' ? Brushes.Gold : Brushes.SlateGray;
    }

    private void UpdateVoltage()
    {
        if (CurrentTypeComboBox.SelectedItem is not ComboBoxItem selectedItem) 
            return;

        Model.Voltage = selectedItem.Content.ToString() == "Постоянный" 
            ? VoltageSlider.Value 
            : 2.5 + 2.5 * Math.Sin(_currentTime / 10);
    }

    private void UpdateChart()
    {
        AdcChart.Series = new ISeries[] { new LineSeries<ObservableValue> 
                { 
                    Values = _voltageValues, 
                    Stroke = new SolidColorPaint(SKColors.Orange) {StrokeThickness = 5},
                    GeometryStroke = new SolidColorPaint(SKColors.Orange),
                    GeometrySize = 0,
                    Fill = new SolidColorPaint(SKColors.Empty)
                }};
        DacChart.Series = new ISeries[] { new StepLineSeries<ObservableValue> 
            { 
                Values = _analogVoltageValues, 
                Stroke = new SolidColorPaint(SKColors.CornflowerBlue) {StrokeThickness = 5},
                GeometryStroke = new SolidColorPaint(SKColors.CornflowerBlue),
                GeometrySize = 0,
                Fill = new SolidColorPaint(SKColors.Empty)
            }};
    }

    private void OnTimerElapsed(object sender, EventArgs e)
    {
        CurrentVoltageTextBlock.Text = $"Текущее напряжение: {Model.Voltage:F2} В";
        DigitalValueTextBlock.Text = $"Цифровое значение: {BinaryDigitalValue}";
        
        _currentTime++;
        _voltageValues.Add(new ObservableValue(Model.Voltage));
        _analogVoltageValues.Add(new ObservableValue(Model.AnalogVoltage));
        
        if (_voltageValues.Count > 50)
            _voltageValues.RemoveAt(0);

        if (_analogVoltageValues.Count > 50)
            _analogVoltageValues.RemoveAt(0);
        
        UpdateComparatorLedPanel();
        UpdateResistorLedPanel();
        UpdateVoltage();
        UpdateChart();
    }

    private static Ellipse CreateLed() => new()
    {
        Width = 30,
        Height = 30,
        Fill = Brushes.Gray,
        Margin = new Thickness(5)
    };
}