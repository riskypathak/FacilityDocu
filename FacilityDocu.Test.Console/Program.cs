using FacilityDocu.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            TestUpdateProjectXml();
        }

        private static void TestUpdateProjectXml()
        {
            SyncManager manager = new SyncManager(new List<int> { 1, 2 });
            manager.UpdateProjectXml();
        }
    }
}
