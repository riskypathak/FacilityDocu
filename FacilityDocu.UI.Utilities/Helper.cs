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

        private static Phrase Format(string input)
        {
            return new Phrase(input, FontFactory.GetFont(FontFactory.HELVETICA, 8));
        }

        public static IList<string> GeneratePdf(ProjectDTO project)
        {
            IList<string> outputs = new List<string>();

            foreach (RigTypeDTO rigType in project.RigTypes)
            {
                string pdfPath = System.IO.Path.Combine(Data.PROJECT_OUTPUT_FOLDER, string.Format("{0}_{1}.pdf", project.Description, rigType.Name));
                outputs.Add(pdfPath);

                FileStream fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write);

                Document doc = new Document(iTextSharp.text.PageSize.A3, 15, 15, 15, 15);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);


                doc.AddTitle("RigDocu");
                doc.AddAuthor(project.CreatedBy.Name);
                doc.Open();
                PdfContentByte cb = writer.DirectContent;
                Footer PdfPageEventHelper = new Footer();
                writer.PageEvent = PdfPageEventHelper;


                doc.NewPage();
                ColumnText ct1 = new ColumnText(cb);
                ct1.SetSimpleColumn(new Phrase(new Chunk(string.Format("{0}", "Table of contents"), FontFactory.GetFont(FontFactory.HELVETICA, 25, Font.BOLD))), 190, 1150, 530, 36, 25, Element.ALIGN_TOP | Element.ALIGN_TOP);
                ct1.Go();
                Paragraph paragraph = new Paragraph("\n\n\n\n\n\n");
                doc.Add(paragraph);

                PdfPTable tblcontent = new PdfPTable(3);
                tblcontent.TotalWidth = PageSize.A3.Width - 30;

                //fix the absolute width of the table
                tblcontent.LockedWidth = true;

                //relative col widths in proportions - 1/3 and 2/3
                float[] widthstable = new float[] { 3f, 4f, 3f };
                tblcontent.SetWidths(widthstable);
                tblcontent.DefaultCell.Border = Rectangle.NO_BORDER;
                tblcontent.HorizontalAlignment = 1;
                tblcontent.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

                tblcontent.AddCell("S.No");
                tblcontent.AddCell("Chapter");
                tblcontent.AddCell("Page");

                tblcontent.AddCell("\n");
                tblcontent.AddCell("\n");
                tblcontent.AddCell("\n");

                foreach (ModuleDTO module in rigType.Modules)
                {
                    tblcontent.AddCell(Format(module.Number));
                    tblcontent.AddCell(Format(module.Name));
                    //tblcontent.AddCell(doc.PageNumber.ToString());
                    tblcontent.AddCell(string.Empty);

                    tblcontent.AddCell("\n");
                    tblcontent.AddCell("\n");
                    tblcontent.AddCell("\n");
                }

                doc.Add(tblcontent);

                foreach (ModuleDTO module in rigType.Modules)
                {
                    doc.NewPage();
                    ColumnText ct = new ColumnText(cb);
                    ct.SetSimpleColumn(new Phrase(new Chunk(string.Format("{0} {1}", module.Number, module.Name), FontFactory.GetFont(FontFactory.HELVETICA, 20, Font.BOLD))), 190, 1150, 530, 36, 25, Element.ALIGN_TOP | Element.ALIGN_TOP);
                    ct.Go();

                    iTextSharp.text.Paragraph paragraph1 = new iTextSharp.text.Paragraph("\n\n\n\n\n");
                    doc.Add(paragraph1);

                    foreach (StepDTO step in module.Steps)
                    {


                        PdfPTable tblAction = new PdfPTable(7);
                        tblAction.TotalWidth = PageSize.A3.Width - 30;
                        //fix the absolute width of the table
                        tblAction.LockedWidth = true;

                        //relative col widths in proportions - 1/3 and 2/3
                        float[] widths = new float[] { 2f, 4f, 2f, 4f, 4f, 2f, 4f };
                        tblAction.SetWidths(widths);
                        tblAction.HorizontalAlignment = 1;
                        tblAction.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;


                        tblAction.AddCell("Action");
                        tblAction.AddCell("Details");
                        tblAction.AddCell("Resources");
                        tblAction.AddCell("Tools");
                        tblAction.AddCell("Dimensions");
                        tblAction.AddCell("Picture no.");
                        tblAction.AddCell("Risks");

                        tblAction.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

                        foreach (ActionDTO action in step.Actions)
                        {
                            tblAction.AddCell(Format(action.Name));
                            tblAction.AddCell(Format(action.Description));
                            tblAction.AddCell(Format(string.Join("\n", action.Resources.Where(r => System.Convert.ToInt32(r.ResourceCount) > 0).Select(r => r.ResourceCount + " " + r.Name).ToArray())));
                            IList<string> tools = action.Tools.Select(r => r.Name).ToList();
                            tools.Add("\n"); tools.Add("Lifting Gears"); tools.Add(action.LiftingGears);
                            tblAction.AddCell(Format(string.Join("\n", tools)));
                            tblAction.AddCell(Format(action.Dimensions));
                            tblAction.AddCell(Format(string.Join("\n", action.Images.Select(i => string.Format("{0}.{1}", action.ActionID, i.ImageID)))));
                            tblAction.AddCell(Format(action.Risks));
                        }

                        doc.Add(tblAction);
                        doc.NewPage();

                        PdfPTable tblImage = new PdfPTable(4);
                        tblImage.TotalWidth = PageSize.A3.Width;

                        widths = new float[] { 4f, 4f, 4f, 4f };
                        tblImage.SetWidths(widths);
                        tblImage.HorizontalAlignment = 0;

                        tblImage.DefaultCell.Border = Rectangle.NO_BORDER;

                        foreach (ImageDTO image in step.Actions.SelectMany(a => a.Images))
                        {
                            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(image.Path);
                            img.ScaleToFit(200f, 180f);
                            //Give space before image
                            img.SpacingBefore = 10f;
                            //Give some space after the image
                            img.SpacingAfter = 5f;
                            img.Alignment = Element.ALIGN_JUSTIFIED;

                            PdfPCell imageCell = new PdfPCell(img);

                            tblImage.AddCell(imageCell);
                        }

                        foreach (ImageDTO image in step.Actions.SelectMany(a => a.Images))
                        {

                            tblImage.AddCell("Picture No. " + step.Actions.First(a => a.Images.Any(i => i.ImageID.Equals(image.ImageID))).ActionID + "." + image.Number);
                        }

                        doc.Add(tblImage);

                        doc.NewPage();
                    }
                }

                doc.Close();
            }

            return outputs;
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
