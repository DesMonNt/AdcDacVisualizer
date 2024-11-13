using AdcDacConversion.Infrastructure.VoltageFunctions;

namespace AdcDacConversion.Infrastructure;

public static class StringConstants
{
    public const string ConstantCurrent = "Постоянный";
    public const string AlternatingCurrent = "Переменный";
    
    public static string CurrentVoltageString(double voltage) => $"Текущее напряжение: {voltage:F2} В";
    
    public static string CurrentDigitalValueString(string binaryDigitalValue) 
        => $"Цифровое значение: {binaryDigitalValue}";
}