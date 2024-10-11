namespace AdcDacConversion.AdcDacModel;

internal interface IConverter<in TSource, out TResult>
{
    TResult Convert(TSource value);
}