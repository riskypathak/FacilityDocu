using DocumentFormat.OpenXml.Wordprocessing;
using FacilityDocu.DTO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Office.Interop.Word;
using Pechkin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace FacilityDocu.UI.Utilities
{
    public class Exporter
    {
        public static IList<string> GeneratePdf(ProjectDTO project, string layoutview, IList<string> exportPage, string textPath, string type)
        {
            IList<string> outputs = new List<string>();
            if (layoutview == "a4")
            {
                foreach (RigTypeDTO rigType in project.RigTypes)
                {
                    if (exportPage.Contains(string.Concat(rigType.Name, "_")))
                    {
                        string pdfPath = string.Empty;

                        if (type == "Pdf")
                        {
                            pdfPath = System.IO.Path.Combine(textPath, string.Format("{0}_{1}_{2}.pdf", project.Description, rigType.Name, layoutview));
                        }
                        else if (type == "Doc")
                        {
                            pdfPath = System.IO.Path.Combine(textPath, string.Format("{0}_{1}_{2}.docx", project.Description, rigType.Name, layoutview));
                        }


                        outputs.Add(pdfPath);

                        bool printRiskAnalysis = false;

                        if (exportPage.Contains(string.Concat(rigType.Name, "RiskAnalysis")))
                        {
                            printRiskAnalysis = true;
                        }
                        GenerateA4Pdf(rigType, pdfPath, printRiskAnalysis, type);
                    }
                }
            }
            else
            {
                foreach (RigTypeDTO rigType in project.RigTypes)
                {
                    if (exportPage.Contains(string.Concat(rigType.Name, "_")))
                    {
                        string pdfPath = System.IO.Path.Combine(textPath, string.Format("{0}_{1}_{2}.pdf", project.Description, rigType.Name, layoutview));
                        outputs.Add(pdfPath);


                        GenerateRigNonA4PDF(project, rigType, pdfPath, layoutview);
                    }

                    if (exportPage.Contains(string.Concat(rigType.Name, "RiskAnalysis")))
                    {
                        string pdfPath = System.IO.Path.Combine(textPath, string.Format("{0}_{1}_RiskAnalysis_{2}.pdf", project.Description, rigType.Name, layoutview));
                        outputs.Add(pdfPath);

                        GenerateRigRiskAnalysisPDF(project, rigType, pdfPath, layoutview);
                    }
                }
            }

            return outputs;
        }

        private static Phrase Format(object input)
        {
            return new Phrase(input.ToString(), FontFactory.GetFont(FontFactory.HELVETICA, 8));
        }

        private static void GenerateRigRiskAnalysisPDF(ProjectDTO project, RigTypeDTO rigType, string pdfPath, string layoutview)
        {
            FileStream fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write);

            iTextSharp.text.Document doc = null;

            if (layoutview == "landscape")
            {
                doc = new iTextSharp.text.Document(new RectangleReadOnly(842, 595), 88f, 88f, 10f, 10f);
            }
            else if (layoutview == "listview")
            {
                doc = new iTextSharp.text.Document(new RectangleReadOnly(842, 595), 88f, 88f, 10f, 10f);
            }

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);

            doc.AddTitle("RigDocu - Risk Analysis");
            doc.AddAuthor(project.CreatedBy.Name);
            doc.Open();
            PdfContentByte cb = writer.DirectContent;
            Footer PdfPageEventHelper = new Footer();
            writer.PageEvent = PdfPageEventHelper;

            foreach (ModuleDTO module in rigType.Modules)
            {
                doc.NewPage();

                doc.Add(new Phrase(string.Format("{0} {1}\n\n", module.Number, module.Name)));

                PdfPTable tblAction = new PdfPTable(11);
                tblAction.TotalWidth = doc.PageSize.Width - 200;
                //fix the absolute width of the table
                tblAction.LockedWidth = true;


                float[] widths = new float[] { 2f, 4f, 1f, 1f, 1f, 1f, 3f, 1f, 1f, 1f, 1f };
                tblAction.SetWidths(widths);
                tblAction.HorizontalAlignment = Element.ALIGN_CENTER;
                tblAction.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

                tblAction.AddCell("Activity");
                tblAction.AddCell("Danger/Consequence");
                tblAction.AddCell("L");
                tblAction.AddCell("S");
                tblAction.AddCell("Risk");
                tblAction.AddCell("Control Measure");
                tblAction.AddCell("Responsible");

                foreach (StepDTO step in module.Steps)
                {
                    foreach (ActionDTO action in step.Actions)
                    {
                        if (action.IsAnalysis)
                        {
                            foreach (RiskAnalysisDTO analysis in action.RiskAnalysis)
                            {
                                tblAction.AddCell(Format(analysis.Activity));
                                tblAction.AddCell(Format(analysis.Danger));
                                tblAction.AddCell(Format(analysis.L));
                                tblAction.AddCell(Format(analysis.S));
                                tblAction.AddCell(Format(analysis.Risk));
                                tblAction.AddCell(Format(analysis.Controls));
                                tblAction.AddCell(Format(analysis.Responsible));
                            }
                        }
                    }

                    doc.Add(tblAction);
                    doc.NewPage();
                }
            }

            doc.Close();
        }

        private static void GenerateRigNonA4PDF(ProjectDTO project, RigTypeDTO rigType, string pdfPath, string layoutview)
        {
            FileStream fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write);

            iTextSharp.text.Document doc = null;

            if (layoutview == "landscape")
            {
                doc = new iTextSharp.text.Document(new RectangleReadOnly(842, 595), 88f, 88f, 10f, 10f);
            }
            else if (layoutview == "listview")
            {
                doc = new iTextSharp.text.Document(new RectangleReadOnly(842, 595), 88f, 88f, 10f, 10f);
            }

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);


            doc.AddTitle("RigDocu");
            doc.AddAuthor(project.CreatedBy.Name);
            doc.Open();
            PdfContentByte cb = writer.DirectContent;
            Footer PdfPageEventHelper = new Footer();
            writer.PageEvent = PdfPageEventHelper;


            doc.NewPage();
            PdfPTable tblcontent = new PdfPTable(2);
            tblcontent.TotalWidth = doc.PageSize.Width;

            //fix the absolute width of the table
            tblcontent.LockedWidth = true;

            //relative col widths in proportions - 1/3 and 2/3
            float[] widthstable = new float[] { 1f, 4f };
            tblcontent.SetWidths(widthstable);
            tblcontent.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            tblcontent.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cell = new PdfPCell(new Phrase("Table of Contents"));
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER;

            tblcontent.AddCell(cell);

            tblcontent.AddCell("\n");
            tblcontent.AddCell("\n");
            tblcontent.AddCell("\n");
            tblcontent.AddCell("\n");

            tblcontent.AddCell("S.No");
            tblcontent.AddCell("Chapter");

            tblcontent.AddCell("\n");
            tblcontent.AddCell("\n");

            foreach (ModuleDTO module in rigType.Modules)
            {
                tblcontent.AddCell(Format(module.Number));
                tblcontent.AddCell(Format(module.Name));

                tblcontent.AddCell("\n");
                tblcontent.AddCell("\n");
            }

            doc.Add(tblcontent);

            foreach (ModuleDTO module in rigType.Modules)
            {
                doc.NewPage();

                doc.Add(new Phrase(string.Format("{0} {1}\n\n", module.Number, module.Name)));

                foreach (StepDTO step in module.Steps)
                {
                    PdfPTable tblAction = new PdfPTable(7);
                    tblAction.TotalWidth = doc.PageSize.Width - 200;
                    //fix the absolute width of the table
                    tblAction.LockedWidth = true;


                    float[] widths = new float[] { 2f, 4f, 2f, 4f, 4f, 2f, 4f };
                    tblAction.SetWidths(widths);
                    tblAction.HorizontalAlignment = Element.ALIGN_CENTER;
                    tblAction.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

                    tblAction.AddCell("Action");
                    tblAction.AddCell("Details");
                    tblAction.AddCell("Resources");
                    tblAction.AddCell("Tools");
                    tblAction.AddCell("Dimensions");
                    tblAction.AddCell("Picture no.");
                    tblAction.AddCell("Risks");

                    foreach (ActionDTO action in step.Actions)
                    {
                        tblAction.AddCell(Format(action.Name));
                        tblAction.AddCell(Format(action.Description));
                        string people = action.People.Replace(Data.SEPERATOR, '\n').Replace(Data.SUBSEPERATOR, ' ');
                        string machines = action.Machines.Replace(Data.SEPERATOR, '\n').Replace(Data.SUBSEPERATOR, ' ');
                        tblAction.AddCell(Format($"{people} \n {machines}"));

                        string tools = action.Tools.Replace(Data.SEPERATOR, '\n');
                        string liftingGears = action.LiftingGears.Replace(Data.SEPERATOR, '\n');

                        tblAction.AddCell(Format($"{tools}\nLifting Gears\n{liftingGears}"));
                        tblAction.AddCell(Format(action.Dimensions));
                        tblAction.AddCell(Format(string.Join("\n", action.Images.Select(i => string.Format("{0}.{1}", action.ActionID, i.Number)))));
                        tblAction.AddCell(Format(action.Risks.Replace(Data.SEPERATOR, '\n')));
                    }

                    doc.Add(tblAction);
                    doc.NewPage();

                    PdfPTable tblImage = new PdfPTable(4);
                    tblImage.TotalWidth = doc.PageSize.Width;

                    widths = new float[] { 4f, 4f, 4f, 4f };
                    tblImage.SetWidths(widths);
                    tblImage.HorizontalAlignment = 0;

                    tblImage.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    foreach (ImageDTO image in step.Actions.SelectMany(a => a.Images).Where(i => i.Used == true))
                    {
                        PdfPTable tblImageCell = new PdfPTable(1);
                        tblImageCell.TotalWidth = 100f;
                        tblImageCell.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(image.Path);
                        img.ScaleToFitHeight = true;
                        img.ScaleToFit(100f, 100f);

                        tblImageCell.AddCell(img);
                        tblImageCell.AddCell("Picture No. " + step.Actions.First(a => a.Images.Any(i => i.ImageID.Equals(image.ImageID))).ActionID + "." + image.Number);

                        tblImage.AddCell(tblImageCell);
                    }

                    doc.Add(tblImage);

                    doc.NewPage();
                }
            }
            doc.Close();
        }

        private static void GenerateA4Pdf(RigTypeDTO rigType, string outputFilePath, bool printRiskAnalysis, string type)
        {
            string finalHtml = "<html><head></head><body style=\"font-family:Verdana;font-size:small\">";

            string htmlLayout = File.ReadAllText("Assets\\pdfa4.html");

            int moduleNo = 1;

            foreach (ModuleDTO module in rigType.Modules)
            {
                int stepNo = 1;
                foreach (StepDTO step in module.Steps)
                {
                    int actionNo = 1;
                    string newActionHtml = string.Empty;
                    foreach (ActionDTO action in step.Actions)
                    {
                        newActionHtml = htmlLayout.Replace("<!--=%MODULESTEPNAME%-->", string.Format("{2}: {0}-{1}", module.Name, step.Name, string.Format("{0}.{1}", moduleNo.ToString("00"), stepNo.ToString("00"))));
                        newActionHtml = newActionHtml.Replace("<!--=%ACTIONAME%-->", action.Name);
                        newActionHtml = newActionHtml.Replace("<!--=%ACTIONDETAILS%-->", action.Description);

                        string resourcesPeopleHtml = string.Empty;

                        foreach (var res in action.People.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var value = res.Split(Data.SUBSEPERATOR);
                            resourcesPeopleHtml += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", value[0], value[1]);
                        }

                        newActionHtml = newActionHtml.Replace("<!--=%RESOURCESPEOPLE%-->", resourcesPeopleHtml);

                        string resourcesMachineHtml = string.Empty;

                        foreach (var res in action.Machines.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var value = res.Split(Data.SUBSEPERATOR);
                            resourcesMachineHtml += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", value[0], value[1]);
                        }
                        newActionHtml = newActionHtml.Replace("<!--=%RESOURCESMACHINES%-->", resourcesMachineHtml);


                        newActionHtml = newActionHtml.Replace("<!--=%ACTIONDIMENSIONS%-->", string.Join("<br/>", action.Dimensions.ToArray()));

                        newActionHtml = newActionHtml.Replace("<!--=%LIFTINGGEARS%-->", string.Join("<br/>", string.Join("<br/>", action.LiftingGears.Split('\n'))));


                        string toolHtml = string.Empty;

                        foreach (var tool in action.Tools.Split(Data.SEPERATOR))
                        {
                            toolHtml += string.Format("<li>{0}</li>", tool);

                        }
                        newActionHtml = newActionHtml.Replace("<!--=%ACTIONTOOLS%-->", toolHtml);


                        if (action.Images.Count > 0)
                        {
                            string imageHtml = "<table>";
                            for (int i = 0; i < action.Images.Count; i++)
                            {
                                imageHtml += string.Format("<tr><td><img width=\"100\" height=\"100\" src=\"{0}\" /></td>", action.Images[i].Path);

                                i = i + 1;

                                if (i != action.Images.Count)
                                {
                                    imageHtml += string.Format("<td><img width=\"100\" height=\"100\" src=\"{0}\" /></td>", action.Images[i].Path);
                                }

                                imageHtml += "</tr>";
                            }

                            imageHtml += "</table>";

                            newActionHtml = newActionHtml.Replace("<!--=%ACTIONIMAGES%-->", imageHtml);
                        }

                        if (printRiskAnalysis && action.IsAnalysis)
                        {
                            string analysisHtml = string.Empty;

                            foreach (RiskAnalysisDTO an in action.RiskAnalysis)
                            {
                                string bgColor;
                                string risk = Helper.GetRisk(an.L, an.S.ToString(), out bgColor);
                                analysisHtml += $"<tr><td>{an.Activity}</td><td>{an.Danger}</td><td>{an.L}</td><td>{an.S}</td><td bgcolor='{bgColor}'>{risk}</td><td>{an.Controls}</td><td>{an.Responsible}</td></tr>";
                            }

                            newActionHtml = newActionHtml.Replace("<!--=%ACTIONRISKANALYSIS%-->", analysisHtml);
                        }

                        actionNo++;

                        finalHtml += newActionHtml;
                    }
                    stepNo++;
                }

                moduleNo++;
            }

            finalHtml += "</body></html>";

            File.WriteAllText("export.html", finalHtml);

            byte[] pdfBuf = new SimplePechkin(new GlobalConfig()).Convert(finalHtml);

            System.IO.File.WriteAllBytes(outputFilePath, pdfBuf);
        }

        private static void SaveAsWordOrPdf(string htmlFile, string outputFilePath, string type)
        {
            Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document wordDoc = new Microsoft.Office.Interop.Word.Document();

            try
            {
                Object oMissing = System.Reflection.Missing.Value;
                wordDoc = word.Documents.Add(ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                word.Visible = false;
                Object filepath = Path.GetFullPath(htmlFile);
                Object confirmconversion = System.Reflection.Missing.Value;
                Object readOnly = false;
                Object saveto = Path.Combine(Path.GetDirectoryName(outputFilePath), outputFilePath);
                Object oallowsubstitution = System.Reflection.Missing.Value;

                wordDoc = word.Documents.Open(ref filepath, ref confirmconversion, ref readOnly, ref oMissing,
                                              ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                                              ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                                              ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                object fileFormat = type == "Pdf" ? Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF : Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatXMLDocument;
                wordDoc.SaveAs(ref saveto, ref fileFormat, ref oMissing, ref oMissing, ref oMissing,
                               ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                               ref oMissing, ref oMissing, ref oMissing, ref oallowsubstitution, ref oMissing,
                               ref oMissing);
            }

            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                wordDoc.Close(Type.Missing, Type.Missing, Type.Missing);
                Marshal.FinalReleaseComObject(wordDoc);


                word.Quit();
                Marshal.FinalReleaseComObject(word);

                if (File.Exists(Path.GetFullPath("Export.html")))
                {
                    File.Delete(Path.GetFullPath("Export.html"));
                }
            }
        }

        public partial class Footer : PdfPageEventHelper
        {
            PdfContentByte cb;
            PdfTemplate template;

            public override void OnOpenDocument(PdfWriter writer, iTextSharp.text.Document document)
            {
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }

            public override void OnEndPage(PdfWriter writer, iTextSharp.text.Document doc)
            {
                BaseColor grey = new BaseColor(128, 128, 128);
                iTextSharp.text.Font font = FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.NORMAL, grey);
                //tbl footer
                PdfPTable footerTbl = new PdfPTable(1);
                footerTbl.TotalWidth = doc.PageSize.Width;

                Chunk myFooter = new Chunk("Page " + (doc.PageNumber), FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, grey));
                PdfPCell footer = new PdfPCell(new Phrase(myFooter));
                footer.Border = iTextSharp.text.Rectangle.NO_BORDER;
                footer.HorizontalAlignment = Element.ALIGN_CENTER;
                footerTbl.AddCell(footer);

                footerTbl.WriteSelectedRows(0, -1, 0, (doc.BottomMargin + 80), writer.DirectContent);
            }

            public override void OnCloseDocument(PdfWriter writer, iTextSharp.text.Document document)
            {
                base.OnCloseDocument(writer, document);
            }
        }

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

        public static void ConvertHtmlToPdf(string htmlFilePath)
        {
            string html = File.ReadAllText(htmlFilePath);
            PdfSharp.Pdf.PdfDocument pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
            pdf.Save("document.pdf");
        }
    }
}
