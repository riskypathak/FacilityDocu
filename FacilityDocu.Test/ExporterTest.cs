using FacilityDocu.UI.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.Test
{
    [TestClass]
    public class ExporterTest
    {
        [TestMethod]
        public void HtmlToWordTest()
        {
            //string html = File.ReadAllText();
            Exporter.ConvertHtmlToWord(@"E:\Works\Projects\Oliver\Code\FacilityDocu\FacilityDocu.UI.WPF\bin\Debug\export.html");
        }
    }
}
