using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using FacilityDocu.UI.Utilities.Services;
using System.Collections.ObjectModel;

namespace FacilityDocLaptop
{
    /// <summary>
    /// Interaction logic for EditHierarchy.xaml
    /// </summary>
    public partial class EditHierarchy : Page
    {
        ProjectDTO project = null;

        public ObservableCollection<ModuleDTO> AvailableModules = new ObservableCollection<ModuleDTO>();
        public ObservableCollection<StepDTO> AvailableSteps = new ObservableCollection<StepDTO>();
        public ObservableCollection<ActionDTO> AvailableActions = new ObservableCollection<ActionDTO>();

        public ObservableCollection<ModuleDTO> ExistingModules = new ObservableCollection<ModuleDTO>();
        public ObservableCollection<StepDTO> ExistingSteps = new ObservableCollection<StepDTO>();
        public ObservableCollection<ActionDTO> ExistingActions = new ObservableCollection<ActionDTO>();

        Services.IFacilityDocuService service = new Services.FacilityDocuServiceClient();

        string projectID = "1";
        int rigTypeIndex = 0;
        int rigTypeID = 1;

        public EditHierarchy()
        {
            InitializeComponent();

            //this.DataContext = this;


            string projectPath = Path.Combine(Path.GetFullPath("Data\\ProjectXml"), string.Format("{0}.xml", projectID));

            project = FacilityDocu.UI.Utilities.ProjectXmlReader.ReadProjectXml(projectPath, false);
            txtProjectName.Text = project.Description;

            lstARigs.ItemsSource = project.RigTypes.ToList();
        }

        private void lstAModules_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            string selectedModuleID = (e.AddedItems[0] as ModuleDTO).ModuleID;

            ModuleDTO eModule = project.RigTypes[rigTypeIndex].Modules.SingleOrDefault(m => m.ModuleID.Equals(selectedModuleID));

            if (eModule != null)
            {
                eModule.Steps.ToList().ForEach(m => ExistingSteps.Add(m));
                lstESteps.ItemsSource = ExistingSteps;
            }
            else
            {
                ExistingSteps.Clear();
            }

            service.GetSteps(Convert.ToInt32(selectedModuleID)).ToList().ForEach(m =>
            {
                AvailableSteps.Clear();
                if (ExistingSteps.Count(s => s.StepID.Equals(m.StepID)) == 0)
                {
                    AvailableSteps.Add(m);
                }
            });

            lstASteps.ItemsSource = AvailableSteps;
        }

        private void lstEModules_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            string selectedModuleID = (e.AddedItems[0] as ModuleDTO).ModuleID;

            ModuleDTO eModule = project.RigTypes[rigTypeIndex].Modules.SingleOrDefault(m => m.ModuleID.Equals(selectedModuleID));

            if (eModule != null)
            {
                eModule.Steps.ToList().ForEach(m => ExistingSteps.Add(m));
                lstESteps.ItemsSource = ExistingSteps;
            }
            else
            {
                ExistingSteps.Clear();
            }

            service.GetSteps(Convert.ToInt32(selectedModuleID)).ToList().ForEach(m =>
            {
                AvailableSteps.Clear();
                if (ExistingSteps.Count(s => s.StepID.Equals(m.StepID)) == 0)
                {
                    AvailableSteps.Add(m);
                }
            });

            lstASteps.ItemsSource = AvailableSteps;
        }

        private void btmAModule_Click_1(object sender, RoutedEventArgs e)
        {
            string moduleID = (sender as Button).CommandParameter.ToString();

            ModuleDTO addedModule = AvailableModules.Single(m => m.ModuleID.Equals(moduleID));
            ExistingModules.Add(addedModule);

            AvailableModules.Remove(addedModule);
            AvailableModules.OrderBy(m => m.ModuleID);
        }


        private void btnEModule_Click_1(object sender, RoutedEventArgs e)
        {
            string moduleID = (sender as Button).CommandParameter.ToString();

            ModuleDTO addedModule = ExistingModules.Single(m => m.ModuleID.Equals(moduleID));
            AvailableModules.Add(addedModule);

            ExistingModules.Remove(addedModule);
            ExistingModules.OrderBy(m => m.ModuleID);
        }

        private void btnCancel_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnSave_Click_1(object sender, RoutedEventArgs e)
        {
            RigTypeDTO rigType = project.RigTypes[rigTypeIndex];
            rigType.Modules = ExistingModules.ToArray();
        }

        private void lstARigs_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            RigTypeDTO rigType = (e.AddedItems[0] as RigTypeDTO);

            rigType.Modules.ToList().ForEach(m => ExistingModules.Add(m));
            lstEModules.ItemsSource = ExistingModules;

            service.GetModules(rigTypeID).ToList().ForEach(m =>
            {
                if (ExistingModules.Count(r => r.ModuleID.Equals(m.ModuleID)) == 0)
                {
                    AvailableModules.Add(m);
                }
            });
            lstAModules.ItemsSource = AvailableModules;
        }
    }
}
