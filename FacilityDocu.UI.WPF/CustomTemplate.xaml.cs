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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FacilityDocLaptop
{
    /// <summary>
    /// Interaction logic for CustomTemplate.xaml
    /// </summary>
    public partial class CustomTemplate : Window
    {
        ProjectDTO template = new ProjectDTO();

        public CustomTemplate()
        {
            InitializeComponent();

            template.RigTypes = (new List<RigTypeDTO>()).ToArray();

            cmbRig.Items.Add("Rig Up");
            cmbRig.Items.Add("Rig Down");
            cmbRig.Items.Add("Rig Move");

            cmbRig.SelectedIndex = 0;
        }

        public CustomTemplate(int templateID)
        {
            InitializeComponent();

            template = Helper.GetTemplate(templateID);



            int counter = 1;
            foreach (RigTypeDTO rig in template.RigTypes)
            {
                foreach (ModuleDTO module in rig.Modules)
                {
                    module.ModuleID = string.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"), counter++);

                    foreach (StepDTO step in module.Steps)
                    {
                        step.StepID = string.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"), counter++);
                    }
                }
            }

            //    string.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"), counter++)



            cmbRig.Items.Add("Rig Up");
            cmbRig.Items.Add("Rig Down");
            cmbRig.Items.Add("Rig Move");

            cmbRig.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            template.Closed = false;
            template.CreatedBy = new UserDTO() { UserName = Data.CURRENT_USER };
            template.CreationDate = DateTime.Now;
            template.Description = txtName.Text;
            template.LastUpdatedAt = DateTime.Now;
            template.LastUpdatedBy = new UserDTO() { UserName = Data.CURRENT_USER };
            template.Template = true;

            for (int i = 0; i < template.RigTypes.Length; i++)
            {
                for (int j = 0; j < template.RigTypes[i].Modules.Length; j++)
                {
                    for (int k = 0; k < template.RigTypes[i].Modules[j].Steps.Length; k++)
                    {
                        ActionDTO action = new ActionDTO() { Description = "New Action", Name = "New Action" };

                        template.RigTypes[i].Modules[j].Steps[k].Actions = (new List<ActionDTO>() { action }).ToArray();
                    }
                }
            }

            Helper.AddTemplate(template);
            (new SyncManager()).Sync();

            MessageBox.Show("Template saved sucessfully!!!");
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lstModules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string rigTypeID = (cmbRig.SelectedIndex + 1).ToString();

                ModuleDTO selectedModule = template.RigTypes
                    .FirstOrDefault(r => r.RigTypeID == rigTypeID).Modules
                    .FirstOrDefault(m => m == (e.AddedItems[0]) as ModuleDTO);

                if (selectedModule != null)
                {
                    lstASteps.ItemsSource = selectedModule.Steps;
                }
            }
        }

        private void cmbRig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRig.SelectedIndex == 0 && template.RigTypes.FirstOrDefault(r => r.RigTypeID == "1") == null)
            {
                IList<RigTypeDTO> rigTypes = template.RigTypes.ToList();
                rigTypes.Add(new RigTypeDTO() { RigTypeID = "1", Name = "Rigup", Modules = (new List<ModuleDTO>()).ToArray() });

                template.RigTypes = rigTypes.ToArray();
            }
            else if (cmbRig.SelectedIndex == 1 && template.RigTypes.FirstOrDefault(r => r.RigTypeID == "2") == null)
            {
                IList<RigTypeDTO> rigTypes = template.RigTypes.ToList();
                rigTypes.Add(new RigTypeDTO() { RigTypeID = "2", Name = "RigDown", Modules = (new List<ModuleDTO>()).ToArray() });

                template.RigTypes = rigTypes.ToArray();
            }
            else if (cmbRig.SelectedIndex == 2 && template.RigTypes.FirstOrDefault(r => r.RigTypeID == "3") == null)
            {
                IList<RigTypeDTO> rigTypes = template.RigTypes.ToList();
                rigTypes.Add(new RigTypeDTO() { RigTypeID = "3", Name = "RigMove", Modules = (new List<ModuleDTO>()).ToArray() });

                template.RigTypes = rigTypes.ToArray();
            }

            RefreshScreen();
        }

        private void btnAddModule_Click(object sender, RoutedEventArgs e)
        {
            ModuleDTO module = new ModuleDTO() { ModuleID = DateTime.Now.ToString("yyyyMMddHHmmssfff"), Name = txtNewModule.Text, Steps = (new List<StepDTO>()).ToArray() };
            string rigTypeID = (cmbRig.SelectedIndex + 1).ToString();

            IList<ModuleDTO> modules = template.RigTypes.FirstOrDefault(r => r.RigTypeID == rigTypeID).Modules.ToList();
            modules.Add(module);

            template.RigTypes.FirstOrDefault(r => r.RigTypeID == rigTypeID).Modules = modules.ToArray();

            lstModules.ItemsSource = modules;
            RefreshScreen();
        }

        private void btnAddStep_Click(object sender, RoutedEventArgs e)
        {
            StepDTO step = new StepDTO() { StepID = DateTime.Now.ToString("yyyyMMddHHmmssfff"), Name = txtNewStep.Text, Actions = (new List<ActionDTO>()).ToArray() };

            string rigTypeID = (cmbRig.SelectedIndex + 1).ToString();

            ModuleDTO selectedModule = (lstModules.SelectedItem as ModuleDTO);

            if (selectedModule != null)
            {
                IList<StepDTO> steps = template.RigTypes.FirstOrDefault(r => r.RigTypeID == rigTypeID).Modules.FirstOrDefault(m => m.Name == selectedModule.Name).Steps.ToList();
                steps.Add(step);

                template.RigTypes.FirstOrDefault(r => r.RigTypeID == rigTypeID).Modules.FirstOrDefault(m => m == selectedModule).Steps = steps.ToArray();
            }
            else
            {
                MessageBox.Show("Select a module before adding step");
            }

            RefreshScreen();
        }

        private void RefreshScreen()
        {
            txtNewModule.Text = string.Empty;
            txtNewStep.Text = string.Empty;

            string rigTypeID = (cmbRig.SelectedIndex + 1).ToString();

            lstModules.ItemsSource = template.RigTypes.FirstOrDefault(r => r.RigTypeID == rigTypeID).Modules;

            ModuleDTO selectedModule = (lstModules.SelectedItem as ModuleDTO);

            if (selectedModule != null)
            {
                lstASteps.ItemsSource = template.RigTypes.FirstOrDefault(r => r.RigTypeID == rigTypeID).Modules.FirstOrDefault(m => m == selectedModule).Steps;
            }
        }

        private void btnRemoveModule_Click(object sender, RoutedEventArgs e)
        {
            string moduleID = (sender as Button).CommandParameter.ToString();

            string rigTypeID = (cmbRig.SelectedIndex + 1).ToString();

            IList<ModuleDTO> modules = template.RigTypes.FirstOrDefault(r => r.RigTypeID == rigTypeID).Modules.ToList();

            ModuleDTO module = modules.FirstOrDefault(m => m.ModuleID == moduleID);
            modules.Remove(module);

            template.RigTypes.FirstOrDefault(r => r.RigTypeID == rigTypeID).Modules = modules.ToArray();

            RefreshScreen();
        }

        private void btnRemoveStep_Click(object sender, RoutedEventArgs e)
        {
            string stepID = (sender as Button).CommandParameter.ToString();

            string rigTypeID = (cmbRig.SelectedIndex + 1).ToString();

            ModuleDTO selectedModule = (lstModules.SelectedItem as ModuleDTO);

            IList<StepDTO> steps = template.RigTypes.FirstOrDefault(r => r.RigTypeID == rigTypeID).Modules.FirstOrDefault(m => m == selectedModule).Steps.ToList();

            StepDTO step = steps.FirstOrDefault(s => s.StepID == stepID);
            steps.Remove(step);

            template.RigTypes.FirstOrDefault(r => r.RigTypeID == rigTypeID).Modules.FirstOrDefault(m => m == selectedModule).Steps = steps.ToArray();

            RefreshScreen();
        }
    }
}
