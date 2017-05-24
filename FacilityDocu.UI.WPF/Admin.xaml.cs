using FacilityDocLaptop.Model;
using FacilityDocLaptop.View.ViewModel;
using FacilityDocu.DTO;
using FacilityDocu.UI.Utilities;
using RestSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FacilityDocLaptop
{
    /// <summary>
    /// Interaction logic for Admin.xaml
    /// </summary>
    public partial class Admin : Window, INotifyPropertyChanged
    {
        private ObservableCollection<string> _allTypes = new ObservableCollection<string>();
        public ObservableCollection<string> AllTypes { get { return new ObservableCollection<string>(new List<string>() { "LiftingGears", "Tools", "Risks", "People", "Machine" }); } }

        private string _selectedType;
        public string SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;

                MasterData.Clear();
                Data.MASTER_DATA.Where(m => m.Type == value).ToList()
                .ForEach(m => MasterData.Add(new MasterDataModel() { Id = m.Id, Name = m.Description }));

                RaisePropertyChanged("SelectedType");
            }
        }

        private ObservableCollection<MasterDataModel> _masterData = new ObservableCollection<MasterDataModel>();
        public ObservableCollection<MasterDataModel> MasterData { get { return _masterData; } set { _masterData = value; RaisePropertyChanged("AllLiftingGears"); } }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand DeleteCommand { get; set; }

        public Admin()
        {
            InitializeComponent();
            this.DataContext = this;

            SyncManager manager = new SyncManager();
            manager.DownloadMasterData();

            this.SaveCommand = new RelayCommand(o =>
            {
                SaveCommandExecute();

            }, o => MasterData.Count > 0);

            this.DeleteCommand = new RelayCommand(o =>
            {
                MasterData.Remove(o as MasterDataModel);

            }, o => MasterData.Count > 0);


            this.CancelCommand = new RelayCommand(o => { this.Close(); }, o => true);
            this.SelectedType = "LiftingGears";
        }

        private void SaveCommandExecute()
        {
            List<MasterDTO> masterData = new List<MasterDTO>();

            foreach (var p in MasterData) //Running Normal Sync
            {
                masterData.Add(new MasterDTO()
                {
                    Description = p.Name,
                    Id = p.Id,
                    Type = this.SelectedType
                });
            }

            SyncManager manager = new SyncManager();
            manager.UpdateMasterData(masterData);

            Task.Run(() =>
            {
                manager.DownloadMasterData();
            });

            MessageBox.Show("Data Saved Successfully.");
        }

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
    }
}
