using System;
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
            using (var host = new Nancy.Hosting.Self.NancyHost(new Uri("http://localhost:8765")))
            {
                host.Start();
                Console.WriteLine("Running on http://*:8765");
                Console.ReadLine();
            }
        }
    }
}
