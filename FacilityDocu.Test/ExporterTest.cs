using FacilityDocu.UI.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfSharp;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace FacilityDocu.Test
{
    [TestClass]
    public class ExporterTest
    {
        [TestMethod]
        public void HtmlToWordTest()
        {
            //string html = File.ReadAllText();
            Exporter.ConvertHtmlToWord(@"E:\Works\Projects\Oliver\Code\FacilityDocu\FacilityDocu.UI.WPF\HtmlOutputExample.html");
        }

        [TestMethod]
        public void HtmlFileToPdfFileTest()
        {
            string html = File.ReadAllText(@"E:\Works\Projects\Oliver\Code\FacilityDocu\FacilityDocu.UI.WPF\HtmlOutputExample.html");
            var config = new PdfGenerateConfig();
            config.PageOrientation = PageOrientation.Landscape;
            config.PageSize = PageSize.A4;
            config.SetMargins(0);
            PdfDocument pdf = PdfGenerator.GeneratePdf(html, config);
            pdf.Save("document.pdf");
        }
    }
}
