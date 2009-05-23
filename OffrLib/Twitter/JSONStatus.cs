using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Offr.Twitter
{
    public class JSONStatus
    {
        public string text { get; set; }
        public int? to_user_id { get; set; }
        public string from_user { get; set; }
        public int id { get; set; }
        public int from_user_id { get; set; }
        public string iso_language_code { get; set; }
        public string source { get; set; }
        public string profile_image_url { get; set; }
        public string created_at {get; set; }
    }
}
