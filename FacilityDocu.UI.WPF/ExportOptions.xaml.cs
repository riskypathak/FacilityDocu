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
        ProjectDTO template = new ProjectDTO();

        private ObservableCollection<string>  _allExportFormats = new ObservableCollection<string>(new List<string>() { "PDF", "HTML" });
        public ObservableCollection<string> AllExportFormats { get { return _allExportFormats; } set { _allExportFormats = value; RaisePropertyChanged("AllExportFormats"); } }

        private string _selectedExportFormat;
        public string SelectedExportFormat { get { return _selectedExportFormat; } set { _selectedExportFormat = value; RaisePropertyChanged("SelectedExportFormat"); } }

        private ObservableCollection<string> _allPageLayouts = new ObservableCollection<string>(new List<string>() { "Landscape", "Portrait" });
        public ObservableCollection<string> AllPageLayouts { get { return _allPageLayouts; } set { _allPageLayouts = value; RaisePropertyChanged("AllPageLayouts"); } }

        private string _selectedPageLayout;
        public string SelectedPageLayout { get { return _selectedPageLayout; } set { _selectedPageLayout = value; RaisePropertyChanged("SelectedPageLayout"); } }

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
            this.SelectedPageLayout = AllPageLayouts.First();

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
            this.SaveCommand = new RelayCommand(o => { Exporter.Export(this.SelectedExportFormat, this.SelectedPageLayout, this.ExportPath); }, o => true);
        }
    }
}
