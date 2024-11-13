namespace AdcDacConversion.Infrastructure.VoltageFunctions;

public abstract class VoltageFunction(double maxVoltage)
{
    protected readonly double MaxVoltage = maxVoltage;

    public abstract double CalculateNewVoltage(double currentTime);
}