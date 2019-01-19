using FacilityDocLaptop.Model;
using FacilityDocLaptop.View.ViewModel;
using FacilityDocu.DTO;
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
        private string _otherToolText;
        public string OtherToolText { get { return _otherToolText; } set { _otherToolText = value; RaisePropertyChanged("OtherToolText"); } }

        private bool _isOtherTool;
        public bool IsOtherTool { get { return _isOtherTool; } set { _isOtherTool = value; RaisePropertyChanged("IsOtherTool"); } }

        private string _selectedType;
        public string SelectedType { get { return _selectedType; } set { _selectedType = value; RaisePropertyChanged("SelectedType"); } }

        private ObservableCollection<string> _allTools = new ObservableCollection<string>();
        public ObservableCollection<string> AllActionItems { get { return _allTools; } set { _allTools = value; RaisePropertyChanged("AllActionItems"); } }

        private string selectedTool;
        public string SelectedActionItem
        {
            get { return selectedTool; }
            set
            {
                selectedTool = value;

                if (!string.IsNullOrEmpty(selectedTool))
                {

                    if (SelectedType == "Tools") SelectedActionItems.Add(selectedTool);
                    else if (SelectedType == "LiftingGears") SelectedLiftingGears.Add(selectedTool);
                    else if (SelectedType == "Risks") SelectedRisks.Add(selectedTool);
                    else if (SelectedType == "People") SelectedPeople.Add(new ResourceDTO() { Name = selectedTool, ResourceCount = "1" });
                    else if (SelectedType == "Machines") SelectedMachines.Add(new ResourceDTO() { Name = selectedTool, ResourceCount = "1" });
                    
                    IsPopUpActionOpen = false;
                }
                RaisePropertyChanged("SelectedActionItem");
            }
        }

        private ObservableCollection<string> _selectedTools = new ObservableCollection<string>();
        public ObservableCollection<string> SelectedActionItems { get { return _selectedTools; } set { _selectedTools = value; RaisePropertyChanged("SelectedActionItems"); } }

        private ObservableCollection<string> _selectedLiftingGears = new ObservableCollection<string>();
        public ObservableCollection<string> SelectedLiftingGears { get { return _selectedLiftingGears; } set { _selectedLiftingGears = value; RaisePropertyChanged("SelectedLiftingGears"); } }

        private ObservableCollection<string> _selectedRisks = new ObservableCollection<string>();
        public ObservableCollection<string> SelectedRisks { get { return _selectedRisks; } set { _selectedRisks = value; RaisePropertyChanged("SelectedRisks"); } }

        private ObservableCollection<ResourceDTO> _selectedPeople = new ObservableCollection<ResourceDTO>();
        public ObservableCollection<ResourceDTO> SelectedPeople { get { return _selectedPeople; } set { _selectedPeople = value; RaisePropertyChanged("SelectedPeople"); } }

        private ObservableCollection<ResourceDTO> _selectedMachines = new ObservableCollection<ResourceDTO>();
        public ObservableCollection<ResourceDTO> SelectedMachines { get { return _selectedMachines; } set { _selectedMachines = value; RaisePropertyChanged("SelectedMachines"); } }

        private bool _isPopUpToolOpen;
        public bool IsPopUpActionOpen
        {
            get { return _isPopUpToolOpen; }
            set
            {
                _isPopUpToolOpen = value;

                if (value)
                {
                    SelectedActionItem = string.Empty;
                    OtherToolText = string.Empty;
                    //If true then populate
                    if (SelectedType == "Tools")
                    {
                        AllActionItems = new ObservableCollection<string>(Data.MASTER_DATA.Where(m => m.Type == "Tools"
                        && !SelectedActionItems.Contains(m.Description)).Select(t => t.Description).ToList());
                    }
                    else if (SelectedType == "LiftingGears")
                    {
                        AllActionItems = new ObservableCollection<string>(Data.MASTER_DATA.Where(m => m.Type == "LiftingGears"
                        && !SelectedLiftingGears.Contains(m.Description)).Select(t => t.Description).ToList());
                    }
                    else if (SelectedType == "Risks")
                    {
                        AllActionItems = new ObservableCollection<string>(Data.MASTER_DATA.Where(m => m.Type == "Risks"
                        && !SelectedRisks.Contains(m.Description)).Select(t => t.Description).ToList());
                    }
                    else if (SelectedType == "People")
                    {
                        AllActionItems = new ObservableCollection<string>(Data.MASTER_DATA.Where(m => m.Type == "People"
                        && !SelectedPeople.Any(p => p.Name == m.Description)).Select(t => t.Description).ToList());
                    }
                    else if (SelectedType == "Machines")
                    {
                        AllActionItems = new ObservableCollection<string>(Data.MASTER_DATA.Where(m => m.Type == "Machine"
                        && !SelectedMachines.Any(p => p.Name == m.Description)).Select(t => t.Description).ToList());
                    }
                }

                RaisePropertyChanged("IsPopUpActionOpen");
            }
        }

        public ICommand ShowPopUpToolCommand { get; set; }
        public ICommand ShowPopUpLiftingGearCommand { get; set; }
        public ICommand ShowPopUpRiskCommand { get; set; }
        public ICommand ShowPopUpPeopleCommand { get; set; }
        public ICommand ShowPopUpMachineCommand { get; set; }

        public ICommand ClosePopUpActionCommand { get; set; }

        public ICommand RemoveToolCommand { get; set; }
        public ICommand RemoveLiftingGearCommand { get; set; }
        public ICommand RemoveRiskCommand { get; set; }
        public ICommand RemovePeopleCommand { get; set; }
        public ICommand RemoveMachineCommand { get; set; }

        public ICommand ShowOtherPopUpActionCommand { get; set; }

        public ICommand AddOtherPopUpToolCommand { get; set; }

        private bool _isPopUpDimensionOpen;
        public bool IsPopUpDimensionOpen { get { return _isPopUpDimensionOpen; } set { _isPopUpDimensionOpen = value; RaisePropertyChanged("IsPopUpDimensionOpen"); } }

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
            
            this.ShowPopUpDimensionCommand = new RelayCommand(o => { IsPopUpDimensionOpen = true; }, o => true);
            this.OKPopUpDimensionCommand = new RelayCommand(o => OKPopUpDimensionCommandExecute(), o => true);
            this.CancelPopUpDimensionCommand = new RelayCommand(o => { IsPopUpDimensionOpen = false; }, o => true);

            this.RemoveToolCommand = new RelayCommand(o =>
            {
                SelectedActionItems.Remove(o.ToString());
            }, o => true);
            this.RemoveLiftingGearCommand = new RelayCommand(o =>
            {
                SelectedLiftingGears.Remove(o.ToString());
            }, o => true);
            this.RemoveRiskCommand = new RelayCommand(o =>
            {
                SelectedRisks.Remove(o.ToString());
            }, o => true);
            this.RemovePeopleCommand = new RelayCommand(o =>
            {
                SelectedPeople.Remove(o as ResourceDTO);

            }, o => true);
            this.RemoveMachineCommand = new RelayCommand(o =>
            {
                SelectedMachines.Remove(o as ResourceDTO);

            }, o => true);

            this.ShowPopUpToolCommand = new RelayCommand(o => { SelectedType = "Tools"; IsPopUpActionOpen = true; IsOtherTool = false; }, o => true);
            this.ShowPopUpLiftingGearCommand = new RelayCommand(o => { SelectedType = "LiftingGears"; IsPopUpActionOpen = true; IsOtherTool = false; }, o => true);
            this.ShowPopUpRiskCommand = new RelayCommand(o => { SelectedType = "Risks"; IsPopUpActionOpen = true; IsOtherTool = false; }, o => true);
            this.ShowPopUpPeopleCommand = new RelayCommand(o =>
            {
                SelectedType = "People";
                IsPopUpActionOpen = true;
                IsOtherTool = false;
            }, o => true);
            this.ShowPopUpMachineCommand = new RelayCommand(o =>
            {
                SelectedType = "Machines";
                IsPopUpActionOpen = true;
                IsOtherTool = false;
            }, o => true);

            this.ClosePopUpActionCommand = new RelayCommand(o => IsPopUpActionOpen = false, o => true);
            this.ShowOtherPopUpActionCommand = new RelayCommand(o => { IsOtherTool = !IsOtherTool; }, o => true);

            this.AddOtherPopUpToolCommand = new RelayCommand(o =>
            {
                if (SelectedType == "Tools")
                {
                    SelectedActionItems.Add(OtherToolText);
                }
                else if (SelectedType == "LiftingGears")
                {
                    SelectedLiftingGears.Add(OtherToolText);
                }
                else if (SelectedType == "Risks")
                {
                    SelectedRisks.Add(OtherToolText);
                }
                else if (SelectedType == "People")
                {
                    SelectedPeople.Add(new ResourceDTO() { Name = OtherToolText, ResourceCount = "1" });
                }
                else if (SelectedType == "Machines")
                {
                    SelectedMachines.Add(new ResourceDTO() { Name = OtherToolText, ResourceCount = "1" });
                }
                IsPopUpActionOpen = false;

            }, o => !string.IsNullOrEmpty(OtherToolText));
        }

        private void OKPopUpDimensionCommandExecute()
        {
            IsPopUpDimensionOpen = false;
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
