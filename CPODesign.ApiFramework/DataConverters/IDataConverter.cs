using CPODesign.ApiFramework.Enums;

namespace CPODesign.ApiFramework.DataConverters
{
    public interface IDataConverter
    {
        T Convert<T>(string resultStr, ConversionType conversionType = ConversionType.JSON);
    }
}