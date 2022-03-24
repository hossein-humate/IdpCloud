using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Humate.WASM.Common
{
    public static class Extension
    {

        //public static Guid LinkRequestIdOnTinkServer = Guid.Empty;

        #region Enum
        public static string GetName<TEnum>(this TEnum item)
        {
            return Enum.GetName(typeof(TEnum), item);
        }

        public static TEnum GetValue<TEnum>(this object item)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), item.ToString());
        }

        public static int GetEnumIndex<TEnum>(this TEnum item)
        {
            return (int)Enum.Parse(typeof(TEnum), item.GetName());
        }
        #endregion

        #region Convert Data Type
        public static byte ToByte(this object input)
        {
            return byte.Parse(input.ToString());
        }

        public static int ToInt(this object input)
        {
            return int.Parse(input.ToString());
        }

        public static Guid ToGuid(this object input)
        {
            try
            {
                return Guid.Parse(input.ToString());
            }
            catch
            {
                return Guid.Empty;
            }
        }
        #endregion

        #region Date & Time
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ConvertToTimestamp(this DateTime? value, DateTime alterValue)
        {
            if (!value.HasValue) value = alterValue;
            TimeSpan elapsedTime = value.Value - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }
        public static long ConvertToTimestamp(this DateTime value)
        {
            TimeSpan elapsedTime = value - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }

        public static DateTime UnixTimeStampToDateTime(this long? unixTimeStamp)
        {
            DateTime dtDateTime = Epoch;
            if (unixTimeStamp.HasValue)
            {
                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp.Value).ToLocalTime();
            }

            return dtDateTime;
        }

        public static DateTime JavaTimeStampToDateTime(this long? unixTimeStamp)
        {
            DateTime dtDateTime = Epoch;
            if (unixTimeStamp.HasValue)
            {
                dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp.Value).ToLocalTime();
            }

            return dtDateTime;
        }
        public static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
        {
            DateTime dtDateTime = Epoch;
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static DateTime JavaTimeStampToDateTime(this long unixTimeStamp)
        {
            DateTime dtDateTime = Epoch;
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static string GetMonthName(this int input, MonthFormat format = MonthFormat.Fullname)
        {
            return format switch
            {
                MonthFormat.Fullname => (input switch
                {
                    1 => "January",
                    2 => "February",
                    3 => "March",
                    4 => "April",
                    5 => "May",
                    6 => "June",
                    7 => "July",
                    8 => "August",
                    9 => "September",
                    10 => "October",
                    11 => "November",
                    12 => "December",
                    _ => default
                }),
                MonthFormat.ThreeChar => (input switch
                {
                    1 => "Jan",
                    2 => "Feb",
                    3 => "Mar",
                    4 => "Apr",
                    5 => "May",
                    6 => "Jun",
                    7 => "Jul",
                    8 => "Aug",
                    9 => "Sep",
                    10 => "Oct",
                    11 => "Nov",
                    12 => "Dec",
                    _ => default
                }),
                _ => default
            };
        }

        public enum MonthFormat
        {
            Fullname,
            ThreeChar
        }
        #endregion

        #region String
        public static string EmptyToNull(this string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        public static string ToCamelcase(this string value)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > 1)
            {
                return char.ToLowerInvariant(value[0]) + value.Substring(1);
            }

            return value;
        }
        #endregion

        public static bool IsEqual<T>(this T x, T y)
        {
            return EqualityComparer<T>.Default.Equals(x, y);
        }

        public static string ThousandSeparator(this object input)
        {
            var number = input.ToString();
            var result = "";
            var mod = number.Length % 3;
            if (mod != 0)
            {
                for (var i = 0; i < mod; i++)
                {
                    result += number[i];
                }
                result += ',';
            }
            for (var i = 0; i < (number.Length - mod) / 3; i++)
            {

                result += number.Substring((i * 3) + mod, 3) + ',';
            }
            result = result[..^1];
            return result;
        }

        public static T Find<T>(this ICollection<T> enumerable, Func<T, bool> predicate)
        {
            foreach (var current in enumerable)
            {
                if (predicate(current))
                {
                    return current;
                }
            }

            return default;
        }

        public static bool IsValidJson(this string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();
            if (value.StartsWith("{", StringComparison.Ordinal) &&
                value.EndsWith("}", StringComparison.Ordinal) || //For object
                value.StartsWith("[", StringComparison.Ordinal) &&
                value.EndsWith("]", StringComparison.Ordinal)) //For array
            {
                try
                {
                    var _ = JToken.Parse(value);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        #region Reflection Type
        public static bool GetValue(object currentObject, string propName, out object value)
        {
            return GetValue(currentObject, propName, out value, new HashSet<object>());
        }

        public static bool GetValue(object currentObject, string propName, out object value,
            HashSet<object> searchedObjects)
        {
            PropertyInfo propInfo = currentObject.GetType().GetProperty(propName);
            if (propInfo != null)
            {
                value = propInfo.GetValue(currentObject, null);
                return true;
            }
            // search child properties
            foreach (PropertyInfo propInfo2 in currentObject.GetType().GetProperties())
            {   // ignore indexed properties
                if (propInfo2.GetIndexParameters().Length == 0)
                {
                    object newObject = propInfo2.GetValue(currentObject, null);
                    if (newObject != null && searchedObjects.Add(newObject) &&
                        GetValue(newObject, propName, out value, searchedObjects))
                        return true;
                }
            }
            // property not found here
            value = null;
            return false;
        }
        #endregion
    }
}