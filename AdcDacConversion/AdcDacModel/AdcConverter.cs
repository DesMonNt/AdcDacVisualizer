using System;
using System.Collections.Generic;
using System.Linq;

namespace AdcDacConversion.AdcDacModel;

internal class AdcConverter(int bitDepth, double referenceVoltage) : IConverter<double, int>
{
    public IReadOnlyList<bool> Comparators => _comparators;

    private readonly bool[] _comparators = new bool[(int)Math.Pow(2, bitDepth) - 1];

    public int Convert(double analogVoltage)
    {
        UpdateComparators(Math.Clamp(analogVoltage, 0, referenceVoltage));

        return _comparators.Sum(System.Convert.ToInt32);
    }

    private void UpdateComparators(double analogVoltage)
    {
        var length = _comparators.Length;
        
        for(var i = 0; i < length; i++)
            _comparators[i] = analogVoltage >= referenceVoltage * (length - i) / length;
    }
}
