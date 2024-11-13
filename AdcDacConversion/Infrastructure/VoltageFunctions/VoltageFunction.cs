namespace AdcDacConversion.Infrastructure.VoltageFunctions;

public abstract class VoltageFunction(double maxVoltage)
{
    protected double MaxVoltage = maxVoltage;

    public abstract double CalculateNewVoltage(double currentVoltage, double currentTime);
}