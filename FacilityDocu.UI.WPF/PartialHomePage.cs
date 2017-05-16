﻿using FacilityDocLaptop.Model;
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

        partial void PartialInit()
        {
            SyncManager manager = new SyncManager();
            manager.DownloadMasterData();

            Data.MASTER_DATA.Where(m => m.Type == "LiftingGears").ToList()
                .ForEach(m => AllLiftingGears.Add(new MasterDataModel() { Id = m.Id, Name = m.Description }));

            Data.MASTER_DATA.Where(m => m.Type == "Risks").ToList()
                .ForEach(m => AllRisks.Add(new MasterDataModel() { Id = m.Id, Name = m.Description }));

            this.ShowPopUpLiftingGearCommand = new RelayCommand(o => { IsPopUpLiftingGearOpen = true; }, o => true);
            this.OKPopUpLiftingGearCommand = new RelayCommand(o => OKPopUpLiftingGearCommandExecute() , o => true);
            this.CancelPopUpLiftingGearCommand = new RelayCommand(o => { IsPopUpLiftingGearOpen = false; }, o => true);

            this.ShowPopUpRiskCommand = new RelayCommand(o => { IsPopUpRiskOpen = true; }, o => true);
            this.OKPopUpRiskCommand = new RelayCommand(o => OKPopUpRiskCommandExecute(), o => true);
            this.CancelPopUpRiskCommand = new RelayCommand(o => { IsPopUpRiskOpen = false; }, o => true);
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
