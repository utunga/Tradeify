using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Common
{
    public static class Util
    {
        public static string ConcatStringArray(IEnumerable<string> reasons)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string reason in reasons)
            {
                sb.Append(reason.ToString());
                sb.Append(",");
            }
            if (sb.Length>0)
            {
                sb.Remove(sb.Length-1,1); //remove the trailing ","
            }
            return sb.ToString();
        }
    }
}
