﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.Services.Rest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new Nancy.Hosting.Self.NancyHost(new Uri("http://localhost:9876")))
            {
                host.Start();
                Console.WriteLine("Running on http://*:9876");
                Console.ReadLine();
            }
        }
    }
}
