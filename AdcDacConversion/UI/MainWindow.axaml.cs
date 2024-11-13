using System;
using System.Linq;
using AdcDacConversion.Application;
using AdcDacConversion.Domain.Entities;
using AdcDacConversion.Infrastructure;
using AdcDacConversion.Infrastructure.Graphics;
using AdcDacConversion.Infrastructure.VoltageFunctions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using SkiaSharp;

namespace AdcDacConversion.UI;

public partial class MainWindow : Window
{
    private const int BitDepth = 4;
    private const double ReferenceVoltage = 5;

    private ConversionService ConversionService { get; } = new(BitDepth, ReferenceVoltage);
    private Timer Timer { get; } = new(TimeSpan.FromMilliseconds(100));
    
    private readonly Graphic _voltageGraphic = new(GraphicsType.Line, SKColors.Orange, 50);
    private readonly Graphic _analogVoltageGraphic = new(GraphicsType.StepLine, SKColors.CornflowerBlue, 50);
    
    public MainWindow()
    {
        InitializeComponent();
    
        for (var i = 0; i < ConversionService.Model.Comparators.Count; i++)
            ComparatorLedPanel.Children.Add(UiUtilities.CreateLed());

        for (var i = 0; i < BitDepth; i++)
            ResistorLedPanel.Children.Add(UiUtilities.CreateLed());
        
        VoltageSlider.PropertyChanged += VoltageSlider_ValueChanged;
        Timer.TimerElapsed += OnTimerElapsed;
        Functions.IsVisible = false;
        
        Timer.Start();
    }

    private void VoltageSlider_ValueChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name != "Value" || sender is not Slider slider) 
            return;

        ConversionService.Model.Voltage = slider.Value;
    }

    private void OnCurrentTypeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CurrentTypeComboBox.SelectedItem is not ComboBoxItem selectedItem) 
            return;
        
        VoltageSlider.IsVisible = selectedItem.Content.ToString() == StringConstants.ConstantCurrent;
        Functions.IsVisible = selectedItem.Content.ToString() == StringConstants.AlternatingCurrent;
        
        Timer.Restart();
    }

    private void UpdateComparatorLedPanel()
    {
        var ledPanel = ComparatorLedPanel.Children.OfType<Ellipse>().ToArray();

        for (var i = 0; i < ConversionService.Model.Comparators.Count; i++)
            ledPanel[i].Fill = ConversionService.Model.Comparators[i] ? Brushes.Gold : Brushes.SlateGray;
    }

    private void UpdateResistorLedPanel()
    {
        var ledPanel = ResistorLedPanel.Children.OfType<Ellipse>().ToArray();

        for (var i = 0; i < BitDepth; i++)
            ledPanel[i].Fill = ConversionService.BinaryDigitalValue[i] == '1' ? Brushes.Gold : Brushes.SlateGray;
    }

    private void UpdateVoltage()
    {
        if (CurrentTypeComboBox.SelectedItem is not ComboBoxItem item) 
            return;

        if (item.Content.ToString() == StringConstants.ConstantCurrent)
        {
            ConversionService.Model.Voltage = VoltageSlider.Value;
            return;
        }

        if (SineRadioButton.IsChecked ?? false)
            ConversionService.SwitchVoltageFunction(VoltageFunctions.Sin);
        else if (TriangleRadioButton.IsChecked ?? false)
            ConversionService.SwitchVoltageFunction(VoltageFunctions.TriangularWave);
        else if (SquareRadioButton.IsChecked ?? false)
            ConversionService.SwitchVoltageFunction(VoltageFunctions.SquareWave);

        ConversionService.Model.Voltage = ConversionService.CalculateNewVoltage(Timer.CurrentTime / 10);
    }

    private void OnTimerElapsed(object sender, EventArgs e)
    {
        CurrentVoltageTextBlock.Text = StringConstants.CurrentVoltageString(ConversionService.Model.Voltage);
        DigitalValueTextBlock.Text = StringConstants.CurrentDigitalValueString(ConversionService.BinaryDigitalValue);
        
        _voltageGraphic.Add(ConversionService.Model.Voltage);
        _analogVoltageGraphic.Add(ConversionService.Model.AnalogVoltage);
        
        AdcChart.Series = _voltageGraphic.ToSeries();
        DacChart.Series = _analogVoltageGraphic.ToSeries();
        
        UpdateComparatorLedPanel();
        UpdateResistorLedPanel();
        UpdateVoltage();
    }
}