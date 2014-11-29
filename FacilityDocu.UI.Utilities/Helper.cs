using FacilityDocu.UI.Utilities.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml.Linq;

namespace FacilityDocu.UI.Utilities
{
    public static class Helper
    {
        public static bool Login(string userName, string password)
        {
            bool isLogin = false;

            XDocument xdoc = XDocument.Load(Data.CONFIG_PATH);

            try
            {
                IFacilityDocuService service = new FacilityDocuServiceClient();
                isLogin = service.Login(userName, password);

                if (isLogin)
                {
                    xdoc.Element("config").Element("lastlogin").Value = userName;
                }
            }
            catch (EndpointNotFoundException)
            {
                if (xdoc.Element("config").Element("lastlogin").Value.Equals(userName))
                {
                    isLogin = true;
                }
            }

            return isLogin;
        }

        public static void GetTools()
        {
            IFacilityDocuService service = new FacilityDocuServiceClient();

            try
            {
                Data.AVAILABLE_TOOLS = service.GetTools();
            }
            catch (EndpointNotFoundException)
            {
                Data.AVAILABLE_TOOLS = new List<ToolDTO>();
            }
        }

        public static bool IsNew(string id)
        {
            bool returnValue;
            returnValue = (id.Length >= 15 || id.Equals("0") || id.StartsWith("-")) ? true : false;

            return returnValue;
        }

        private static Phrase Format(object input)
        {
            return new Phrase(input.ToString(), FontFactory.GetFont(FontFactory.HELVETICA, 8));
        }

        public static IList<string> GeneratePdf(ProjectDTO project)
        {
            IList<string> outputs = new List<string>();

            foreach (RigTypeDTO rigType in project.RigTypes)
            {
                string pdfPath = System.IO.Path.Combine(Data.PROJECT_OUTPUT_FOLDER, string.Format("{0}_{1}.pdf", project.Description, rigType.Name));
                outputs.Add(pdfPath);

                GenerateRigPDF(project, rigType, pdfPath);

                pdfPath = System.IO.Path.Combine(Data.PROJECT_OUTPUT_FOLDER, string.Format("{0}_{1}_RiskAnalysis.pdf", project.Description, rigType.Name));
                outputs.Add(pdfPath);

                GenerateRigRiskAnalysisPDF(project, rigType, pdfPath);
            }

            return outputs;
        }

        private static void GenerateRigRiskAnalysisPDF(ProjectDTO project, RigTypeDTO rigType, string pdfPath)
        {
            FileStream fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write);

            Document doc = new Document(new RectangleReadOnly(842, 595), 88f, 88f, 10f, 10f);
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
                tblAction.AddCell("K");
                tblAction.AddCell("B");
                tblAction.AddCell("E");
                tblAction.AddCell("Risk");
                tblAction.AddCell("Control Measure");
                tblAction.AddCell("K'");
                tblAction.AddCell("B'");
                tblAction.AddCell("E'");
                tblAction.AddCell("Risk'");

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
                                tblAction.AddCell(Format(analysis.K));
                                tblAction.AddCell(Format(analysis.B));
                                tblAction.AddCell(Format(analysis.E));
                                tblAction.AddCell(Format(analysis.Risk));
                                tblAction.AddCell(Format(analysis.Controls));
                                tblAction.AddCell(Format(analysis.K_));
                                tblAction.AddCell(Format(analysis.B_));
                                tblAction.AddCell(Format(analysis.E_));
                                tblAction.AddCell(Format(analysis.Risk_));
                            }
                        }
                    }

                    doc.Add(tblAction);
                    doc.NewPage();
                }
            }

            doc.Close();
        }

        private static void GenerateRigPDF(ProjectDTO project, RigTypeDTO rigType, string pdfPath)
        {
            FileStream fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write);

            Document doc = new Document(new RectangleReadOnly(842, 595), 88f, 88f, 10f, 10f);
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
            tblcontent.DefaultCell.Border = Rectangle.NO_BORDER;
            tblcontent.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cell = new PdfPCell(new Phrase("Table of Contents"));
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = Rectangle.NO_BORDER;

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
                        tblAction.AddCell(Format(string.Join("\n", action.Resources.Where(r => System.Convert.ToInt32(r.ResourceCount) > 0).Select(r => r.ResourceCount + " " + r.Name).ToArray())));
                        IList<string> tools = action.Tools.Select(r => r.Name).ToList();
                        tools.Add("\n"); tools.Add("Lifting Gears"); tools.Add(action.LiftingGears);
                        tblAction.AddCell(Format(string.Join("\n", tools)));
                        tblAction.AddCell(Format(action.Dimensions));
                        tblAction.AddCell(Format(string.Join("\n", action.Images.Select(i => string.Format("{0}.{1}", action.ActionID, i.Number)))));
                        tblAction.AddCell(Format(action.Risks));
                    }

                    doc.Add(tblAction);
                    doc.NewPage();

                    PdfPTable tblImage = new PdfPTable(4);
                    tblImage.TotalWidth = doc.PageSize.Width;

                    widths = new float[] { 4f, 4f, 4f, 4f };
                    tblImage.SetWidths(widths);
                    tblImage.HorizontalAlignment = 0;

                    tblImage.DefaultCell.Border = Rectangle.NO_BORDER;

                    foreach (ImageDTO image in step.Actions.SelectMany(a => a.Images).Where(i => i.Used == true))
                    {
                        PdfPTable tblImageCell = new PdfPTable(1);
                        tblImageCell.TotalWidth = 100f;
                        tblImageCell.DefaultCell.Border = Rectangle.NO_BORDER;

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

        public partial class Footer : PdfPageEventHelper
        {
            PdfContentByte cb;
            PdfTemplate template;

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }

            public override void OnEndPage(PdfWriter writer, Document doc)
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

            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
            }
        }

        public static void WriteLog(string message, System.Diagnostics.EventLogEntryType type = System.Diagnostics.EventLogEntryType.Information)
        {
            System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
            appLog.Source = "FacilityDocu-LaptopApp";
            appLog.WriteEntry(message, type);
        }
    }
}
