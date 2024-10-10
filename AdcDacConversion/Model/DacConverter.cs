namespace AdcDacConversion.Model;

internal class DacConverter(int bitDepth, double referenceVoltage) : IConverter<int, double>
{
    public double Convert(int digitalValue)
    {
        var maxSteps = (1 << bitDepth) - 1;
        var analogVoltage = digitalValue / (double)maxSteps * referenceVoltage;
        
        return analogVoltage;
    }
}