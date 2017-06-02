using DocumentFormat.OpenXml.Wordprocessing;
using FacilityDocu.DTO;
using Microsoft.Office.Interop.Word;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Pechkin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace FacilityDocu.UI.Utilities
{
    public class Exporter
    {
        public static string Export(ProjectDTO project, string selectedExportFormat, string exportPath)
        {
            string outputFolderPath = string.Empty;

            if (selectedExportFormat == "PDF")
            {
                var config = new PdfGenerateConfig();
                config.PageOrientation = PageOrientation.Landscape;
                config.PageSize = PdfSharp.PageSize.A4;
                config.SetMargins(5);

                PdfDocument outputPDFDocument = new PdfDocument();

                outputFolderPath = Path.Combine(exportPath, $"{DateTime.Now.ToString("yyyyMMddhhmmss")}.pdf");
                StringBuilder menuRigHtml = new StringBuilder();
                for (int rigIndex = 1; rigIndex <= project.RigTypes.Count; rigIndex++)
                {
                    RigTypeDTO rig = project.RigTypes[rigIndex - 1];

                    for (int moduleIndex = 1; moduleIndex <= rig.Modules.Count; moduleIndex++)
                    {
                        ModuleDTO module = rig.Modules[moduleIndex - 1];

                        for (int stepIndex = 1; stepIndex <= module.Steps.Count; stepIndex++)
                        {
                            StepDTO step = module.Steps[stepIndex - 1];

                            for (int actionIndex = 1; actionIndex <= step.Actions.Count; actionIndex++)
                            {
                                ActionDTO action = step.Actions[actionIndex - 1];

                                StringBuilder htmlLayout = new StringBuilder(File.ReadAllText("Assets\\pdflayout.html"));

                                htmlLayout.Replace("<!--=%STYLEPATH%-->", Path.GetFullPath("Content\\pdfstyles.css"));
                                htmlLayout.Replace("<!--=%LOGOPATH%-->", Path.GetFullPath("Content\\pdfheaderimage.PNG"));

                                htmlLayout.Replace("<!--=%PROJECTDETAILS%-->", $"{project.Description} - {project.CreationDate.ToString("dd MMM yyyy")}");
                                htmlLayout.Replace("<!--=%RIGNAME%-->", $"RIG {rig.Name}");
                                htmlLayout.Replace("<!--=%MODULENAME%-->", $"Chapter {moduleIndex}. {module.Name}");
                                htmlLayout.Replace("<!--=%STEPNAME%-->", $"Step {moduleIndex}.{stepIndex} {step.Name}");

                                htmlLayout.Replace("<!--=%ACTIONNAME%-->", $"{actionIndex}. {action.Name.Replace("Run>", "span>").Replace("Foreground=\"#FFFF0000\"", "style=\"color: red\"")}");
                                htmlLayout.Replace("<!--=%ACTIONDESC%-->", action.Description.Replace("Run>", "span>").Replace("Foreground=\"#FFFF0000\"", "class=\"redfont\""));

                                htmlLayout.Replace("<!--=%ACTIONDIMENSIONS%-->", action.Dimensions.Replace("\n", "<br />"));

                                StringBuilder machineHtml = new StringBuilder();
                                foreach (string machine in action.Machines.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    string[] vals = machine.Split(new char[1] { Data.SUBSEPERATOR });
                                    machineHtml.Append($"• {vals[0]} - {vals[1]}<br/>");
                                }
                                htmlLayout.Replace("<!--=%ACTIONMACHINES%-->", machineHtml.ToString());

                                StringBuilder peopleHtml = new StringBuilder();
                                foreach (string people in action.People.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    string[] vals = people.Split(new char[1] { Data.SUBSEPERATOR });
                                    peopleHtml.Append($"• {vals[0]} - {vals[1]}<br/>");
                                }
                                htmlLayout.Replace("<!--=%ACTIONPEOPLE%-->", peopleHtml.ToString());

                                StringBuilder toolHtml = new StringBuilder();
                                foreach (string tool in action.Tools.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    toolHtml.Append($"•  {tool}<br/>");
                                }
                                htmlLayout.Replace("<!--=%ACTIONTOOLS%-->", toolHtml.ToString());

                                StringBuilder lgHtml = new StringBuilder();
                                foreach (string lg in action.LiftingGears.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    lgHtml.Append($"•  {lg}<br/>");
                                }
                                htmlLayout.Replace("<!--=%ACTIONLIFTINGGEARS%-->", lgHtml.ToString());


                                StringBuilder riskHtml = new StringBuilder();
                                foreach (string risk in action.Risks.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    riskHtml.Append($"•  {risk}<br/>");
                                }
                                htmlLayout.Replace("<!--=%ACTIONRISKS%-->", riskHtml.ToString());

                                IList<ImageDTO> images = action.Images.Where(a => a.Used).ToList();
                                StringBuilder imageHtml = new StringBuilder();
                                int imageIndex = 1;

                                if (images.Count >= imageIndex)
                                {
                                    imageHtml.Append("<tr>");
                                    for (imageIndex = 1; imageIndex <= 3; imageIndex++)
                                    {
                                        if (images.Count >= imageIndex)
                                        {
                                            ImageDTO image = images[imageIndex - 1];
                                            imageHtml.Append($"<td class=\"imgProject\"><img class=\"imgProject\" src=\"{image.Path}\" /></td>");
                                        }
                                    }
                                    imageHtml.Append("</tr>");
                                }
                                if (images.Count >= imageIndex)
                                {
                                    imageHtml.Append("<tr>");
                                    for (imageIndex = 4; imageIndex <= 6; imageIndex++)
                                    {
                                        if (images.Count >= imageIndex)
                                        {
                                            ImageDTO image = images[imageIndex - 1];
                                            imageHtml.Append($"<td class=\"imgProject\"><img class=\"imgProject\" src=\"{image.Path}\" /></td>");
                                        }
                                    }
                                    imageHtml.Append("</tr>");
                                }

                                htmlLayout.Replace("<!--=%ACTIONIMAGES%-->", imageHtml.ToString());

                                StringBuilder attachmentHtml = new StringBuilder();
                                foreach (AttachmentDTO attach in action.Attachments)
                                {
                                    try
                                    {
                                        attachmentHtml.Append($"• {Path.GetFileName(attach.Path)}<br/>");
                                    }
                                    catch (Exception ex) { }
                                }

                                htmlLayout.Replace("<!--=%ACTIONATTACHMENTS%-->", attachmentHtml.ToString());

                                if (action.IsAnalysis)
                                {
                                    StringBuilder hiraHtml = new StringBuilder();
                                    foreach (RiskAnalysisDTO ra in action.RiskAnalysis)
                                    {
                                        string bgColor;
                                        string risk = Helper.GetRisk(ra.L, ra.S.ToString(), out bgColor);

                                        hiraHtml.Append($"<tr>" +
                                        $"<td class=\"tablecontent\">{ra.Activity.Replace("\n", "<br />")}</td>" +
                                        $"<td class=\"tablecontent\">{ra.Danger.Replace("\n", "<br />")}</td>" +
                                        $"<td class=\"tablecontent\" style=\"background-color:{bgColor}\">{risk}</td>" +
                                        $"<td class=\"tablecontent\">{ra.Controls.Replace("\n", "<br />")}</td>" +
                                        $"<td class=\"tablecontent\">{ra.Responsible.Replace("\n", "<br />")}</td>" +
                                        "</tr>");
                                    }
                                    htmlLayout.Replace("<!--=%ACTIONHIRA%-->", hiraHtml.ToString());
                                }

                                PdfDocument pdfDoc = PdfGenerator.GeneratePdf(htmlLayout.ToString(), config);
                                string tempFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}.pdf");
                                pdfDoc.Save(tempFile);

                                PdfDocument inputDocument = PdfReader.Open(tempFile, PdfDocumentOpenMode.Import);
                                PdfSharp.Pdf.PdfPage pdfPage = inputDocument.Pages[0];

                                outputPDFDocument.AddPage(pdfPage);

                                foreach (AttachmentDTO attach in action.Attachments)
                                {
                                    if (File.Exists(attach.Path))
                                    {
                                        inputDocument = PdfReader.Open(attach.Path, PdfDocumentOpenMode.Import);

                                        foreach (PdfPage aPage in inputDocument.Pages)
                                        {
                                            outputPDFDocument.AddPage(aPage);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                outputPDFDocument.Save(outputFolderPath);
            }
            else if (selectedExportFormat == "HTML")
            {
                outputFolderPath = ExportHtml(project, exportPath);
            }

            return outputFolderPath;
        }

        private static string ExportHtml(ProjectDTO project, string exportPath)
        {
            string outputFolderPath;
            StringBuilder menuRigHtml = new StringBuilder();
            for (int rigIndex = 1; rigIndex <= project.RigTypes.Count; rigIndex++)
            {
                menuRigHtml.Append($"<a href=\"{rigIndex}_1_1_1.html\">RIG {project.RigTypes[rigIndex - 1].Name}</a>");
            }

            //Create sub folder
            outputFolderPath = Path.Combine(exportPath, DateTime.Now.ToString("yyyyMMddhhmmss"));
            Directory.CreateDirectory(outputFolderPath);

            string pagesFolderPath = Path.Combine(outputFolderPath, "Pages");
            string contentFolderPath = Path.Combine(pagesFolderPath, "Content");
            Directory.CreateDirectory(pagesFolderPath);
            Directory.CreateDirectory(contentFolderPath);

            string indexFilePath = Path.Combine(outputFolderPath, "index.html");
            File.WriteAllText(indexFilePath, "<meta http-equiv=\"refresh\" content=\"0; url = Pages/1_1_1_1.html\" />");

            //Copy Content Folder
            string sourceContentFolderPath = Path.GetFullPath("Content");
            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourceContentFolderPath, "*.*", SearchOption.TopDirectoryOnly))
            {
                File.Copy(newPath, newPath.Replace(sourceContentFolderPath, contentFolderPath), true);
            }

            for (int rigIndex = 1; rigIndex <= project.RigTypes.Count; rigIndex++)
            {
                RigTypeDTO rig = project.RigTypes[rigIndex - 1];
                StringBuilder menuModuleHtml = new StringBuilder();
                for (int moduleIndex = 1; moduleIndex <= rig.Modules.Count; moduleIndex++)
                {
                    menuModuleHtml.Append($"<a href=\"{rigIndex}_{moduleIndex}_1_1.html\">Chapter {moduleIndex}. {rig.Modules[moduleIndex - 1].Name}</a>");
                }

                for (int moduleIndex = 1; moduleIndex <= rig.Modules.Count; moduleIndex++)
                {
                    ModuleDTO module = rig.Modules[moduleIndex - 1];
                    StringBuilder menuStepHtml = new StringBuilder();
                    for (int stepIndex = 1; stepIndex <= module.Steps.Count; stepIndex++)
                    {
                        menuStepHtml.Append($"<a href=\"{rigIndex}_{moduleIndex}_{stepIndex}_1.html\">Step {moduleIndex}.{stepIndex}. {module.Steps[stepIndex - 1].Name}</a>");
                    }

                    for (int stepIndex = 1; stepIndex <= module.Steps.Count; stepIndex++)
                    {
                        StepDTO step = module.Steps[stepIndex - 1];

                        for (int actionIndex = 1; actionIndex <= step.Actions.Count; actionIndex++)
                        {
                            ActionDTO action = step.Actions[actionIndex - 1];

                            StringBuilder htmlLayout = new StringBuilder(File.ReadAllText("Assets\\htmllayout.html"));
                            htmlLayout.Replace("<!--=%PROJECTDETAILS%-->", $"{project.Description} - {project.CreationDate.ToString("dd MMM yyyy")}");
                            htmlLayout.Replace("<!--=%MENURIGS%-->", menuRigHtml.ToString());
                            htmlLayout.Replace("<!--=%RIGNAME%-->", $"RIG {rig.Name}");
                            htmlLayout.Replace("<!--=%MENUMODULES%-->", menuModuleHtml.ToString());
                            htmlLayout.Replace("<!--=%MODULENAME%-->", $"Chapter {moduleIndex}. {module.Name}");
                            htmlLayout.Replace("<!--=%MENUSTEPS%-->", menuStepHtml.ToString());
                            htmlLayout.Replace("<!--=%STEPNAME%-->", $"Step {moduleIndex}.{stepIndex} {step.Name}");

                            int previousActionIndex = actionIndex == 1 ? step.Actions.Count : actionIndex - 1;
                            int nextActionIndex = actionIndex == step.Actions.Count ? 1 : actionIndex + 1;
                            htmlLayout.Replace("<!--=%LINKPREVIOUSNEXTACTION%-->",
                                $"<a href=\"{rigIndex}_{moduleIndex}_{stepIndex}_{previousActionIndex}.html\" class=\"previous\">&#8249;</a>&nbsp;&nbsp;" +
                                $"<a href=\"{rigIndex}_{moduleIndex}_{stepIndex}_{nextActionIndex}.html\" class=\"next\">&#8250;</a>");

                            htmlLayout.Replace("<!--=%ACTIONNAME%-->", $"{actionIndex}. {action.Name.Replace("Run>", "span>").Replace("Foreground=\"#FFFF0000\"", "style=\"color: red\"")}");
                            htmlLayout.Replace("<!--=%ACTIONDESC%-->", action.Description.Replace("Run>", "span>").Replace("Foreground=\"#FFFF0000\"", "style=\"color: red\""));
                            htmlLayout.Replace("<!--=%ACTIONDIMENSIONS%-->", action.Dimensions.Replace("\n", "<br />"));

                            StringBuilder machineHtml = new StringBuilder();
                            foreach (string machine in action.Machines.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                string[] vals = machine.Split(new char[1] { Data.SUBSEPERATOR });
                                machineHtml.Append($"<li>{vals[0]} - {vals[1]}</li>");
                            }
                            htmlLayout.Replace("<!--=%ACTIONMACHINES%-->", machineHtml.ToString());

                            StringBuilder peopleHtml = new StringBuilder();
                            foreach (string people in action.People.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                string[] vals = people.Split(new char[1] { Data.SUBSEPERATOR });
                                peopleHtml.Append($"<li>{vals[0]} - {vals[1]}</li>");
                            }
                            htmlLayout.Replace("<!--=%ACTIONPEOPLE%-->", peopleHtml.ToString());

                            StringBuilder toolHtml = new StringBuilder();
                            foreach (string tool in action.Tools.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                toolHtml.Append($"<li>{tool}</li>");
                            }
                            htmlLayout.Replace("<!--=%ACTIONTOOLS%-->", toolHtml.ToString());

                            StringBuilder lgHtml = new StringBuilder();
                            foreach (string lg in action.LiftingGears.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                lgHtml.Append($"<li>{lg}</li>");
                            }
                            htmlLayout.Replace("<!--=%ACTIONLIFTINGGEARS%-->", lgHtml.ToString());


                            StringBuilder riskHtml = new StringBuilder();
                            foreach (string risk in action.Risks.Split(new char[1] { Data.SEPERATOR }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                riskHtml.Append($"<li>{risk}</li>");
                            }
                            htmlLayout.Replace("<!--=%ACTIONRISKS%-->", riskHtml.ToString());


                            StringBuilder imageHtml = new StringBuilder();
                            foreach (ImageDTO image in action.Images.Where(i=>i.Used))
                            {
                                try
                                {
                                    //Copy image path to content
                                    File.Copy(image.Path, Path.Combine(contentFolderPath, Path.GetFileName(image.Path)), true);

                                    imageHtml.Append($"<div class=\"rdimg\"><a rel=\"rigduco\" href=\"./Content/{ Path.GetFileName(image.Path)}\" class=\"swipebox\" title=\"{Path.GetFileName(image.Path)}\"><img class=\"imgProject\" src=\"./Content/{Path.GetFileName(image.Path)}\" /></a></div>");
                                }
                                catch (Exception ex) { }
                            }
                            htmlLayout.Replace("<!--=%ACTIONIMAGES%-->", imageHtml.ToString());

                            StringBuilder attachmentHtml = new StringBuilder();
                            foreach (AttachmentDTO attach in action.Attachments)
                            {
                                try
                                {
                                    //Copy image path to content
                                    File.Copy(attach.Path, Path.Combine(contentFolderPath, Path.GetFileName(attach.Path)), true);

                                    attachmentHtml.Append($"<a href=\"./Content/{Path.GetFileName(attach.Path)}\" target=\"newwindow\">{attach.Name}</a><br />");
                                }
                                catch(Exception ex) { }
                            }

                            htmlLayout.Replace("<!--=%ACTIONATTACHMENTS%-->", attachmentHtml.ToString());

                            if (action.IsAnalysis)
                            {
                                StringBuilder hiraHtml = new StringBuilder();
                                foreach (RiskAnalysisDTO ra in action.RiskAnalysis)
                                {
                                    string bgColor;
                                    string risk = Helper.GetRisk(ra.L, ra.S.ToString(), out bgColor);

                                    hiraHtml.Append($"<tr>" +
                                    $"<td class=\"tablecontent\">{ra.Activity.Replace("\n", "<br />")}</td>" +
                                    $"<td class=\"tablecontent\">{ra.Danger.Replace("\n", "<br />")}</td>" +
                                    $"<td class=\"tablecontent\" style=\"background-color:{bgColor}\">{risk}</td>" +
                                    $"<td class=\"tablecontent\">{ra.Controls.Replace("\n", "<br />")}</td>" +
                                    $"<td class=\"tablecontent\">{ra.Responsible.Replace("\n", "<br />")}</td>" +
                                    "</tr>");
                                }
                                htmlLayout.Replace("<!--=%ACTIONHIRA%-->", hiraHtml.ToString());
                            }

                            string actionFilePath = Path.Combine(pagesFolderPath, $"{rigIndex}_{moduleIndex}_{stepIndex}_{actionIndex}.html");
                            File.WriteAllText(actionFilePath, htmlLayout.ToString());
                        }
                    }
                }
            }

            return outputFolderPath;
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
