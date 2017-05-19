using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Office.Interop.Word;
using NotesFor.HtmlToOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.UI.Utilities
{
    public class Exporter
    {
        public static void ConvertHtmlToWord(string htmlFilePath)
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document wordDoc = null;

            try
            {
                var savePathDocx = Path.GetFullPath("Html2PdfTest.docx");
                wordDoc = word.Documents.Open(FileName: htmlFilePath, ReadOnly: false);
                wordDoc.SaveAs2(FileName: savePathDocx, FileFormat: WdSaveFormat.wdFormatXMLDocument);
            }

            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                wordDoc.Close(Type.Missing, Type.Missing, Type.Missing);
                Marshal.FinalReleaseComObject(wordDoc);


                word.Quit();
                Marshal.FinalReleaseComObject(word);
            }
        }
    }
}
