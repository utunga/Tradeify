using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Ninject;
using NLog;
using Offr;
using Offr.Json;
using Offr.Repository;
using Offr.Services;

namespace OffrConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            (new PushToCouchService()).Run();
        }

    }
}
