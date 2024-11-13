using System;

namespace AdcDacConversion.Infrastructure.VoltageFunctions;

public class Sin(double maxVoltage) : VoltageFunction(maxVoltage)
{
    public override double CalculateNewVoltage(double currentTime) 
        => MaxVoltage / 2 + MaxVoltage / 2 * Math.Sin(currentTime);
}