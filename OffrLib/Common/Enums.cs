using System.Collections.Generic;
using System.Linq;

namespace Offr.Common
{
    public class Enums
    {
        /// <summary>
        /// Helper method to turn the values of a the Enum.GetValues() call which is, unhelpfully an Array into an IEnumerable
        /// thank you to: http://damieng.com/blog/2008/04/10/using-linq-to-foreach-over-an-enum-in-c
        /// </summary>
        public static IEnumerable<T> Get<T>()
        {
            return System.Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
