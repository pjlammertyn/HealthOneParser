using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace HealthOneParser
{
    internal static class Utils
    {
        //internal static V Maybe<T, V>(this T t, Func<T, V> selector)
        //{
        //    return t != null ? selector(t) : default(V);
        //}

        internal static void AddItem<TKey, TItem>(this IDictionary<TKey, IList<TItem>> dict, TKey key, TItem item)
        {
            if (!dict.ContainsKey(key))
                dict.Add(key, new List<TItem>());

            dict[key].Add(item);
        }

        internal static DateTime? ToNullableDatetime(this string value, params string[] formats)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            DateTime ret;
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out ret))
                    return ret;
            }

            return null;
        }

        internal static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        internal static long ToInt64(this string value)
        {
            Int64 result = 0;

            if (!value.IsNullOrEmpty())
                Int64.TryParse(value, out result);

            return result;
        }

        internal static bool IsValidSocialSecurityNumber(this string socialSecurityNumber)
        {
            if (socialSecurityNumber.IsNullOrEmpty())
                return false;

            long ssn = socialSecurityNumber.ToInt64();

            var valid = IsValidSocialSecurityNumberBefore2000(ssn);
            if (!valid)
                valid = IsValidSocialSecurityNumberAfter2000(ssn);
            return valid;
        }

        static bool IsValidSocialSecurityNumberBefore2000(long ssn)
        {
            int checkDigit = (int)(ssn % 100);
            int numberWithoutCheckDigit = (int)(ssn / 100);

            return ((97 - (numberWithoutCheckDigit % 97)) == checkDigit);
        }

        static bool IsValidSocialSecurityNumberAfter2000(long ssn)
        {
            int checkDigit = (int)(ssn % 100);
            int numberWithoutCheckDigit = (int)(ssn / 100);

            return (97 - ((numberWithoutCheckDigit + (long)2000000000) % 97) == checkDigit);
        }
    }
}
