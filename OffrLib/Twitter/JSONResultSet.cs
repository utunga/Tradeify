using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Twitter
{
    public class JSONResultSet
    {
        public JSONStatus[] results;
        public int since_id { get; set; }
        public int max_id { get; set; }
        public string refresh_url { get; set; }
        public int results_per_page { get; set; }
        public int total { get; set; }
        public double completed_in { get; set; }
        public int page { get; set; }
        public string query { get; set; }
    }
}
