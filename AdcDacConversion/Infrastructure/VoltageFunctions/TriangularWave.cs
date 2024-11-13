using System;

namespace AdcDacConversion.Infrastructure.VoltageFunctions;

public class TriangularWave(double maxVoltage, double period, double normalizationFactor) : VoltageFunction(maxVoltage)
{
    public override double CalculateNewVoltage(double currentVoltage, double currentTime)
    {
        var phase = Math.Abs(currentTime % period - period / 2) / normalizationFactor - 1;

        return MaxVoltage / 2 * (1 + phase);
    }
}