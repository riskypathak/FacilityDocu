using FacilityDocLaptop.Model;
using FacilityDocLaptop.View.ViewModel;
using FacilityDocu.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FacilityDocLaptop
{
    public partial class HomePage : INotifyPropertyChanged
    {
        private bool _isPopUpLiftingGearOpen;
        public bool IsPopUpLiftingGearOpen { get { return _isPopUpLiftingGearOpen; } set { _isPopUpLiftingGearOpen = value; RaisePropertyChanged("IsPopUpLiftingGearOpen"); } }

        private ObservableCollection<MasterDataModel> _allLiftingGears = new ObservableCollection<MasterDataModel>();
        public ObservableCollection<MasterDataModel> AllLiftingGears { get { return _allLiftingGears; } set { _allLiftingGears = value; RaisePropertyChanged("AllLiftingGears"); } }

        private MasterDataModel _selectedLiftingGear;
        public MasterDataModel SelectedLiftingGear { get { return _selectedLiftingGear; } set { _selectedLiftingGear = value; RaisePropertyChanged("SelectedLiftingGear"); } }

        public ICommand ShowPopUpLiftingGearCommand { get; set; }
        public ICommand OKPopUpLiftingGearCommand { get; set; }
        public ICommand CancelPopUpLiftingGearCommand { get; set; }

        private bool _isPopUpRiskOpen;
        public bool IsPopUpRiskOpen { get { return _isPopUpRiskOpen; } set { _isPopUpRiskOpen = value; RaisePropertyChanged("IsPopUpRiskOpen"); } }

        private ObservableCollection<MasterDataModel> _allRisks = new ObservableCollection<MasterDataModel>();
        public ObservableCollection<MasterDataModel> AllRisks { get { return _allRisks; } set { _allRisks = value; RaisePropertyChanged("AllRisks"); } }

        private MasterDataModel _selectedRisk;
        public MasterDataModel SelectedRisk { get { return _selectedRisk; } set { _selectedRisk = value; RaisePropertyChanged("SelectedRisk"); } }

        public ICommand ShowPopUpRiskCommand { get; set; }
        public ICommand OKPopUpRiskCommand { get; set; }
        public ICommand CancelPopUpRiskCommand { get; set; }


        private bool _isPopUpDimensionOpen;
        public bool IsPopUpDimensionOpen { get { return _isPopUpRiskOpen; } set { _isPopUpRiskOpen = value; RaisePropertyChanged("IsPopUpDimensionOpen"); } }

        public ICommand ShowPopUpDimensionCommand { get; set; }
        public ICommand OKPopUpDimensionCommand { get; set; }
        public ICommand CancelPopUpDimensionCommand { get; set; }

        private double _dimensionLength;
        public double DimensionLength { get { return _dimensionLength; } set { _dimensionLength = value; RaisePropertyChanged("DimensionLength"); RaisePropertyChanged("Dimension"); } }

        private double _dimensionWidth;
        public double DimensionWidth { get { return _dimensionWidth; } set { _dimensionWidth = value; RaisePropertyChanged("DimensionWidth"); RaisePropertyChanged("Dimension"); } }

        private double _dimensionHeight;
        public double DimensionHeight { get { return _dimensionHeight; } set { _dimensionHeight = value; RaisePropertyChanged("DimensionHeight"); RaisePropertyChanged("Dimension"); } }

        private double _dimensionWeight;
        public double DimensionWeight { get { return _dimensionWeight; } set { _dimensionWeight = value; RaisePropertyChanged("DimensionWeight"); RaisePropertyChanged("Dimension"); } }

        public string Dimension
        {
            get
            {
                if (_dimensionLength == 0 || _dimensionWidth == 0 || _dimensionHeight == 0 || _dimensionWeight == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return $"{_dimensionLength.ToString("0.00")}X{_dimensionWidth.ToString("0.00")}X{_dimensionHeight.ToString("0.00")} Wg: {_dimensionWeight.ToString("0.00")}";
                }
            }
        }

        public string _role;
        public string Role { get { return _role; } set { _role = value; RaisePropertyChanged("Role"); } }

        partial void PartialInit()
        {
            SyncManager manager = new SyncManager();
            manager.DownloadMasterData();

            Data.MASTER_DATA.Where(m => m.Type == "LiftingGears").ToList()
                .ForEach(m => AllLiftingGears.Add(new MasterDataModel() { Id = m.Id, Name = m.Description }));

            Data.MASTER_DATA.Where(m => m.Type == "Risks").ToList()
                .ForEach(m => AllRisks.Add(new MasterDataModel() { Id = m.Id, Name = m.Description }));

            this.ShowPopUpLiftingGearCommand = new RelayCommand(o => { IsPopUpLiftingGearOpen = true; }, o => true);
            this.OKPopUpLiftingGearCommand = new RelayCommand(o => OKPopUpLiftingGearCommandExecute(), o => true);
            this.CancelPopUpLiftingGearCommand = new RelayCommand(o => { IsPopUpLiftingGearOpen = false; }, o => true);

            this.ShowPopUpRiskCommand = new RelayCommand(o => { IsPopUpRiskOpen = true; }, o => true);
            this.OKPopUpRiskCommand = new RelayCommand(o => OKPopUpRiskCommandExecute(), o => true);
            this.CancelPopUpRiskCommand = new RelayCommand(o => { IsPopUpRiskOpen = false; }, o => true);

            this.ShowPopUpDimensionCommand = new RelayCommand(o => { IsPopUpDimensionOpen = true; }, o => true);
            this.OKPopUpDimensionCommand = new RelayCommand(o => OKPopUpDimensionCommandExecute(), o => true);
            this.CancelPopUpDimensionCommand = new RelayCommand(o => { IsPopUpDimensionOpen = false; }, o => true);
        }

        private void OKPopUpDimensionCommandExecute()
        {
            IsPopUpDimensionOpen = false;
        }

        private void OKPopUpLiftingGearCommandExecute()
        {
            IsPopUpLiftingGearOpen = false;
        }

        private void OKPopUpRiskCommandExecute()
        {
            IsPopUpRiskOpen = false;
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
