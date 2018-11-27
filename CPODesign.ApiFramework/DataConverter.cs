using CPODesign.ApiFramework;
using Newtonsoft.Json;
using System;

namespace ConsoleCPODesign.ApiFrameworkApp1
{
    /// <summary>
    /// Custom data converter
    /// </summary>
    public class DataConverter : IDataConverter
    {
        /// <summary>
        /// Converts the specified result string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resultStr">The result string.</param>
        /// <param name="conversionType">Type of the conversion.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">resultStr</exception>
        /// <exception cref="ArgumentException">The string does not contain opening json bracket - resultStr</exception>
        /// <exception cref="NotImplementedException"></exception>
        public T Convert<T>(string resultStr, ConversionType conversionType = ConversionType.JSON )
        {
            if (resultStr == null) throw new ArgumentNullException(nameof(resultStr));
            if (string.IsNullOrWhiteSpace(resultStr)) return (T)Activator.CreateInstance(typeof(T));

            switch (conversionType)
            {
                case ConversionType.JSON:
                    {
                        if (!resultStr.Contains("{")) throw new ArgumentException("The string does not contain opening json bracket", nameof(resultStr));
                        return JsonConvert.DeserializeObject<T>(resultStr);
                    }
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public interface IDataConverter
    {
        T Convert<T>(string resultStr, ConversionType conversionType = ConversionType.JSON);
    }
}