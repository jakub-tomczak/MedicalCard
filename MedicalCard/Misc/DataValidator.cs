using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MedicalCard.Misc
{
    public static class DataValidator
    {
        public static bool TryValidateDateTime(string input, out DateTime validated, DateTime latestDatePossible)
        {
            var toReturn = TryValidateDateTime(input, out validated);
            return toReturn && validated <= latestDatePossible;
        }
        public static bool TryValidateDateTime(string input, out DateTime validated)
        {
            return DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out validated);
        }
        public static bool TryValidateString(string input, out string validated, uint minLength = 0, uint maxLength = UInt32.MaxValue, string regexPattern = null)
        {
            validated = input;
            var returnValue = validated.Length >= minLength && validated.Length <= maxLength;
            if (string.IsNullOrEmpty(regexPattern))
            {
                return returnValue && !string.IsNullOrEmpty(validated);
            }
            else
            {
                var regexValidator = new Regex(regexPattern);
                return returnValue && !string.IsNullOrEmpty(validated) && regexValidator.Matches(validated).Count() > 0;
            }
        }
    }
}
