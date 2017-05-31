using FacilityDocLaptop.View.ViewModel;
using FacilityDocu.DTO;
using FacilityDocu.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace FacilityDocLaptop
{
    /// <summary>
    /// Interaction logic for CustomTemplate.xaml
    /// </summary>
    public partial class ExportOptions : Window, INotifyPropertyChanged
    {
        private ObservableCollection<string> _allExportFormats = new ObservableCollection<string>(new List<string>() { "PDF", "HTML" });
        public ObservableCollection<string> AllExportFormats { get { return _allExportFormats; } set { _allExportFormats = value; RaisePropertyChanged("AllExportFormats"); } }

        private string _selectedExportFormat;
        public string SelectedExportFormat { get { return _selectedExportFormat; } set { _selectedExportFormat = value; RaisePropertyChanged("SelectedExportFormat"); } }

        private string _exportPath;
        public string ExportPath { get { return _exportPath; } set { _exportPath = value; RaisePropertyChanged("ExportPath"); } }

        public ICommand BrowseCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #region Property Changed
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion

        public ExportOptions()
        {
            InitializeComponent();
            this.DataContext = this;
            //Provide Default Values to ComboBoxes
            this.SelectedExportFormat = AllExportFormats.First();

            this.BrowseCommand = new RelayCommand(o =>
            {
                FolderBrowserDialog folder = new FolderBrowserDialog();
                folder.RootFolder = Environment.SpecialFolder.Desktop;
                if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.ExportPath = folder.SelectedPath;
                }
            }, o => true);

            this.CancelCommand = new RelayCommand(o => { this.Close(); }, o => true);
            this.SaveCommand = new RelayCommand(o =>
            {
                string projectPath = System.IO.Path.Combine(Data.PROJECT_XML_FOLDER, string.Format("{0}.xml", Data.CURRENT_PROJECT.ProjectID));

                Data.CURRENT_PROJECT = ProjectXmlReader.ReadProjectXml(projectPath, false);

                string outputFile = Exporter.Export(Data.CURRENT_PROJECT, this.SelectedExportFormat, this.ExportPath);

                System.Windows.MessageBox.Show($"{this.SelectedExportFormat} output generated at {outputFile}");


            }, o => !string.IsNullOrEmpty(this.SelectedExportFormat) && !string.IsNullOrEmpty(this.ExportPath));
        }
    }
}
