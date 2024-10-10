using System;
using System.Collections.Generic;

namespace AdcDacConversion.Model;

internal sealed class Model(int bitDepth, double referenceVoltage)
{
    public double Voltage
    {
        get => _voltage;
        set
        {
            _voltage = Math.Clamp(value, 0, ReferenceVoltage);
            DigitalValue = Adc.Convert(_voltage);
            AnalogVoltage = Dac.Convert(DigitalValue);
        }
    }
    
    public int DigitalValue { get; private set; }
    public double AnalogVoltage { get; private set; }

    public IReadOnlyList<bool> Comparators => Adc.Comparators;
    
    private double _voltage;
    private double ReferenceVoltage { get; } = referenceVoltage;
    private AdcConverter Adc { get; } = new(bitDepth, referenceVoltage);
    private DacConverter Dac { get; } = new(bitDepth, referenceVoltage);
}