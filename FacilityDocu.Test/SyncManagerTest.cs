using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FacilityDocu.UI.Utilities;
using System.Linq;
using System.Collections.Generic;

namespace FacilityDocu.Test
{
    [TestClass]
    public class SyncManagerTest
    {
        [TestMethod]
        public void TestUpdateProjectXml()
        {
            SyncManager manager = new SyncManager(new List<int> { 1, 2 });
            manager.UpdateProjectXml(null);
        }
    }
}
