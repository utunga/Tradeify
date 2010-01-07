using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Offr.Json;

namespace OffrConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime time = DateTime.Now;
            Debug.Assert(time.Equals(JSON.Deserialize<DateTime>(JSON.Serialize(time))));
            Console.Out.WriteLine("time :" + time.ToLongTimeString() + " JSON.Deserialize<DateTime>(JSON.Serialize(time)):" + JSON.Deserialize<DateTime>(JSON.Serialize(time)).ToLongTimeString());
        }
    }
}
