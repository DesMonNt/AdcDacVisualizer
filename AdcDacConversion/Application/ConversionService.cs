using System;
using AdcDacConversion.Domain.Entities;
using AdcDacConversion.Infrastructure.VoltageFunctions;

namespace AdcDacConversion.Application;

public class ConversionService(int bitDepth, double referenceVoltage)
{
    public readonly Model Model = new(bitDepth, referenceVoltage);
    public string BinaryDigitalValue => Convert.ToString(Model.DigitalValue, 2).PadLeft(bitDepth, '0');
    
    private VoltageFunction _voltageFunction = new Sin(referenceVoltage);

    public double CalculateNewVoltage(double time) => _voltageFunction.CalculateNewVoltage(time);
    
    public void SwitchVoltageFunction(VoltageFunctions function)
    {
        _voltageFunction = function switch
        {
            VoltageFunctions.Constant => new Sin(referenceVoltage),
            VoltageFunctions.Sin => new Sin(referenceVoltage),
            VoltageFunctions.TriangularWave => new TriangularWave(referenceVoltage, referenceVoltage, referenceVoltage / 4),
            VoltageFunctions.SquareWave => new SquareWave(referenceVoltage, referenceVoltage),
            _ => throw new ArgumentOutOfRangeException(nameof(function), function, null)
        };
    }
}