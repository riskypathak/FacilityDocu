using FacilityDocu.UI.Utilities;
using FacilityDocu.UI.Utilities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FacilityDocLaptop
{
    /// <summary>
    /// Interaction logic for CustomTemplate.xaml
    /// </summary>
    public partial class ExportOptions : Window
    {
        ProjectDTO template = new ProjectDTO();

        public ExportOptions()
        {
            InitializeComponent();

            txtExportPath.Text = Data.EXPORT_PDF_PATH;

            cmbTemplates.Items.Add("Landscape Full Page");
            cmbTemplates.Items.Add("Landscape List Mode");
            cmbTemplates.Items.Add("A4");
            cmbTemplates.SelectedIndex = 2;

            cmbContent.Items.Add("Pdf");
            cmbContent.Items.Add("Doc");
            cmbContent.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string projectPath = System.IO.Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", Data.CURRENT_PROJECT.ProjectID));

            Data.CURRENT_PROJECT = ProjectXmlReader.ReadProjectXml(projectPath, false);

            string layoutType = "landscape";

            if (cmbTemplates.SelectedIndex == 1)
            {
                layoutType = "listview";
            }
            else if (cmbTemplates.SelectedIndex == 2)
            {
                layoutType = "a4";
            }
            else
            {
                layoutType = "landscape";
            }

            IList<string> exportPage = new List<string>();

            if(chkRigUp.IsChecked.Value)
            {
                exportPage.Add("UP_");
            }

            if (chkRigDown.IsChecked.Value)
            {
                exportPage.Add("Down_");
            }

            if (chkRigMove.IsChecked.Value)
            {
                exportPage.Add("Move_");
            }

            if (chkRigUpRisk.IsChecked.Value)
            {
                exportPage.Add("UPRiskAnalysis");
            }

            if (chkRigDownRisk.IsChecked.Value)
            {
                exportPage.Add("DownRiskAnalysis");
            }

            if (chkRigMoveRisk.IsChecked.Value)
            {
                exportPage.Add("MoveRiskAnalysis");
            }

            IList<string> outputs = Helper.GeneratePdf(Data.CURRENT_PROJECT, layoutType, exportPage, txtExportPath.Text, cmbContent.SelectedValue.ToString());

            System.Windows.MessageBox.Show(string.Concat("Files Generated at\n", string.Join("\n", outputs.ToArray())));
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.RootFolder = Environment.SpecialFolder.Desktop;
            if(folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtExportPath.Text = Data.EXPORT_PDF_PATH = folder.SelectedPath;
            }

        }
    }
}
