using FacilityDocu.UI.Utilities.Services;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.UI.Utilities
{
    public static class Helper
    {
        private static Phrase Format(string input)
        {
            return new Phrase(input, FontFactory.GetFont(FontFactory.HELVETICA, 8));
        }

        public static void GeneratePdf(ProjectDTO project)
        {
            foreach (RigTypeDTO rigType in project.RigTypes)
            {
                string pdfPath = System.IO.Path.GetFullPath(string.Format("Data/Output/{0}_{1}.pdf", project.Description, rigType.Name));
                FileStream fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write);

                Document doc = new Document(iTextSharp.text.PageSize.A4, 5, 5, 20, 15);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);


                doc.AddTitle("FacilityDocu");
                doc.AddAuthor(project.CreatedBy.Name);
                doc.Open();

                PdfContentByte cb = writer.DirectContent;

                foreach (ModuleDTO module in rigType.Modules)
                {
                    doc.NewPage();
                    doc.Add(new Chunk(string.Format("{0} {1}", module.Number, module.Name)));

                    foreach (StepDTO step in module.Steps)
                    {
                        PdfPTable tblAction = new PdfPTable(7);
                        tblAction.TotalWidth = PageSize.A4.Width;
                        //fix the absolute width of the table
                        tblAction.LockedWidth = true;

                        //relative col widths in proportions - 1/3 and 2/3
                        float[] widths = new float[] { 1f, 3f, 1f, 2f, 2f, 0.5f, 2f };
                        tblAction.SetWidths(widths);
                        tblAction.HorizontalAlignment = 0;

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
                            tblAction.AddCell(Format(string.Join("\n", action.Resources.Select(r => r.Name).ToArray())));
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
                        tblImage.TotalWidth = PageSize.A4.Width;

                        widths = new float[] { 1f, 1f, 1f, 1f };
                        tblImage.SetWidths(widths);
                        tblImage.HorizontalAlignment = 0;

                        foreach (ImageDTO image in step.Actions.SelectMany(a=>a.Images))
                        {
                            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(image.Path);
                            img.ScaleToFit(140f, 120f);
                            //Give space before image
                            img.SpacingBefore = 10f;
                            //Give some space after the image
                            img.SpacingAfter = 1f;
                            img.Alignment = Element.ALIGN_LEFT;

                            PdfPCell imageCell = new PdfPCell(img);

                            tblImage.AddCell(imageCell);
                        }
                        doc.Add(tblImage);

                        doc.NewPage();
                    }
                }

                doc.Close();
            }




            //PdfContentByte cb = wri.DirectContent;
            //PdfContentByte cb2 = wri.DirectContent;

            //// cb.SetColorFill(BaseColor.BLUE);
            //ColumnText ct = new ColumnText(cb);
            //ColumnText ct2 = new ColumnText(cb2);
            //ct.SetSimpleColumn(new Phrase(new Chunk(ProjectName + "\n", FontFactory.GetFont(FontFactory.HELVETICA, 80, Font.NORMAL))), 100, 600, 530, 36, 25, Element.ALIGN_LEFT | Element.ALIGN_TOP);
            //ct2.SetSimpleColumn(new Phrase(new Chunk("Rig Type: " + pdf_RigType, FontFactory.GetFont(FontFactory.HELVETICA, 50, Font.NORMAL))), 150, 500, 530, 36, 25, Element.ALIGN_LEFT | Element.ALIGN_TOP);
            //ct.Go();
            //ct2.Go();

            //doc.NewPage();

            //doc.Add(new iTextSharp.text.Paragraph(""));
            //PdfContentByte cb3 = wri.DirectContent;
            //ColumnText ct3 = new ColumnText(cb3);
            //ct3.SetSimpleColumn(new Phrase(new Chunk("Module List", FontFactory.GetFont(FontFactory.HELVETICA, 40, Font.NORMAL))), 100, 770, 530, 36, 25, Element.ALIGN_LEFT | Element.ALIGN_TOP);
            //ct3.Go();

            //iTextSharp.text.Paragraph paragraph = new iTextSharp.text.Paragraph("\n\n\n");
            //doc.Add(paragraph);
            ////RomanList romanlist = new RomanList(true, 100);
            ////romanlist.IndentationLeft = 30f;
            ////romanlist.Add(Module);
            //iTextSharp.text.List list = new iTextSharp.text.List(iTextSharp.text.List.ALPHABETICAL, 40f);
            //list.IndentationLeft = 40f;
            //list.Add(pdf_Module);
            //doc.Add(list);

            //doc.NewPage();
            //doc.Add(new iTextSharp.text.Paragraph(""));



            ///////////////////IMAGE///////////////////
            //int i = 0, k = 0, l = 0, m = 0, n = 0;

            //while (i < pdf_imagedataname.Count())
            //{
            //    iTextSharp.text.Image PNG = iTextSharp.text.Image.GetInstance(XMLPath + "data/" + pdf_imagedataname[i] + ".jpg");
            //    // PNG.ScalePercent(10f); //size according to percentage

            //    PNG.ScaleToFit(250f, 500f);  //rectange

            //    // PNG.Border = iTextSharp.text.Rectangle.BOX; //border to images

            //    ///  PNG.BorderColor = iTextSharp.text.BaseColor.YELLOW;
            //    //  PNG.BorderWidth = 5f;

            //    PNG.SetAbsolutePosition(k + m, l + n); //position test
            //    doc.Add(PNG);
            //    i++;
            //    k = k + 100;
            //    l = l + 100;
            //    m = m + 50;
            //    n = n + 50;
            //}





            //doc.Close();
        }
    }
}
