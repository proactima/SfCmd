using System;
using Microsoft.Extensions.CommandLineUtils;

namespace ServiceFabricUploader
{
    public static class OptionsHelper
    {
        public static T GetEnumValueOrDefault<T>(CommandOption rawConfigOption, T defaultValue)
            where T : struct, IConvertible
        {
            if (!rawConfigOption.HasValue())
                return defaultValue;

            T value;
            return Enum.TryParse(rawConfigOption.Value(), out value)
                ? value
                : defaultValue;
        }

        public static string GetStringOrDefaultValue(CommandOption rawConfigOption, string defaultValue)
        {
            return rawConfigOption.HasValue()
                ? rawConfigOption.Value()
                : defaultValue;
        }

        public static int GetIntOrDefaultValue(CommandOption rawConfigOption, int defaultValue)
        {
            if (!rawConfigOption.HasValue())
                return defaultValue;

            var value = rawConfigOption.Value();
            int actualValue;
            if (Int32.TryParse(value, out actualValue))
                return actualValue;

            throw new Exception("Unable to parse Cluster Port");
        }
    }
}