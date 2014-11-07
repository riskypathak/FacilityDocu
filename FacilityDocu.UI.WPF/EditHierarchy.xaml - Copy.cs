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

        public EditHierarchy()
        {
            InitializeComponent();

            //this.DataContext = this;

            string projectID = "1";

            string projectPath = Path.Combine(Path.GetFullPath("Data\\ProjectXml"), string.Format("{0}.xml", projectID));

            project = FacilityDocu.UI.Utilities.ProjectXmlReader.ReadProjectXml(projectPath, false);

            txbProjectName.Text = project.Description;

            project.RigTypes[0].Modules.ToList().ForEach(m=>ExistingModules.Add(m));
            

            lstModules.ItemsSource = ExistingModules;

            //lstModules.ItemsSource = Modules;
            cmbModules.ItemsSource = service.GetModules(1).ToList();
        }

        private void lstModules_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            IList<StepDTO> steps = project.RigTypes[0].Modules.Single(m => m.Name.Equals((e.AddedItems[0] as ModuleDTO).Name.ToString())).Steps;
            steps.ToList().ForEach(m => lstSteps.Items.Add(m.Name));
        }

        private void ListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            IList<ActionDTO> actions = project.RigTypes[0].Modules.Single(m => m.Name.Equals((lstModules.SelectedValue as ModuleDTO).Name.ToString()))
                .Steps.First(s => s.Name.Equals(e.AddedItems[0].ToString())).Actions;

            lstActions.ItemsSource = actions;
            //actions.ToList().ForEach(m => lstActions.Items.Add(m.Name));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ExistingModules.Add(new ModuleDTO() { ModuleID = "123", Name = "Added" });

        }

        private void btnAddModule_Click(object sender, RoutedEventArgs e)
        {
            ModuleDTO addedItem = cmbModules.SelectedItem as ModuleDTO;
            ExistingModules.Add(new ModuleDTO() { ModuleID = addedItem.ModuleID, Name = addedItem.Name });

        }

        private void cmbModules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbSteps.ItemsSource = service.GetSteps(Convert.ToInt32((e.AddedItems[0] as ModuleDTO).ModuleID));
        }

        private void cmbSteps_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            cmbActions.ItemsSource = service.GetActions(Convert.ToInt32((e.AddedItems[0] as StepDTO).StepID));
        }
    }
}
