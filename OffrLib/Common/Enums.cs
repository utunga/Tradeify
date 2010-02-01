using System;
using System.Collections.Generic;
using System.Linq;

namespace Offr.Common
{
    public static class Enums
    {
        /// <summary>
        /// Helper method to turn the values of a the Enum.GetValues() call which is, unhelpfully an Array into an IEnumerable
        /// thank you to: http://damieng.com/blog/2008/04/10/using-linq-to-foreach-over-an-enum-in-c
        /// </summary>
        public static IEnumerable<T> Get<T>()
        {
            return System.Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// helper method courtesy of http://stackoverflow.com/questions/1082532/how-to-tryparse-for-enum-value to avoid having try/catch around enum parsing
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="strEnumValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(this string strEnumValue, TEnum defaultValue)
        {
            if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
                return defaultValue;

            return (TEnum)Enum.Parse(typeof(TEnum), strEnumValue);
        }


        public static bool TryParse<TEnum>(string strEnumValue, out TEnum parsedEnum)
        {
            if (!Enum.IsDefined(typeof(TEnum), strEnumValue))
            {
                parsedEnum = default(TEnum);
                return false;
            }
            else
            {
                parsedEnum = (TEnum) Enum.Parse(typeof (TEnum), strEnumValue);
                return true;
            }

        }
    }
}
