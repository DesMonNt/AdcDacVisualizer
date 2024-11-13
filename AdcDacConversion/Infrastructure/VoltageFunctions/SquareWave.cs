namespace AdcDacConversion.Infrastructure.VoltageFunctions;

public class SquareWave(double maxVoltage, double period) : VoltageFunction(maxVoltage)
{
    public override double CalculateNewVoltage(double currentVoltage, double currentTime)
    {
        var phase = currentTime % period;
        return phase < period / 2 ? MaxVoltage : -MaxVoltage;
    }
}