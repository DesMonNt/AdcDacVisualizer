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
    private ConversionService ConversionService { get; set; } = new(4, 5);
    private Timer Timer { get; } = new(TimeSpan.FromMilliseconds(100));
    
    private readonly Graphic _voltageGraphic = new(GraphicsType.Line, SKColors.Orange, 50);
    private readonly Graphic _analogVoltageGraphic = new(GraphicsType.StepLine, SKColors.CornflowerBlue, 50);
    private int _bitDepth = 4;
    
    public MainWindow()
    {
        InitializeComponent();
        InitializeLedPanels();
        
        VoltageSlider.PropertyChanged += VoltageSlider_ValueChanged;
        Timer.TimerElapsed += OnTimerElapsed;
        Functions.IsVisible = false;
        
        Timer.Start();
    }
    
    private void InitializeLedPanels()
    {
        for (var i = 0; i < ConversionService.Model.Comparators.Count; i++)
            ComparatorLedPanel.Children.Add(UiUtilities.CreateLed());

        for (var i = 0; i < _bitDepth; i++)
            ResistorLedPanel.Children.Add(UiUtilities.CreateLed());
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

    private void OnBitDepthChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DacResolutionComboBox.SelectedItem is not ComboBoxItem item) 
            return;

        var content = item.Content?.ToString();
        
        if (content is null)
            return;
        
        _bitDepth = int.Parse(content.Split()[0]);
        ConversionService = new ConversionService(_bitDepth, 5);
        
        ComparatorLedPanel.Children.Clear();
        ResistorLedPanel.Children.Clear();
        InitializeLedPanels();
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

        for (var i = 0; i < _bitDepth; i++)
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