using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace FacilityDocLaptop
{

    public class conn
    {
        //WCF Added
        Services.FacilityDocuServiceClient MyService;
    }
    public class TempSaveData
    {

        static DataSet userTable;
        static DataSet moduleTable;
        static DataSet stepTable;
        static DataSet actionTable;
        static DataSet toolTable;
        static DataSet resourceTable;
        static DataSet imagesTable;
        static DataSet moduleStepTable;
        static DataSet rigTypeTable;
        static DataSet projectTable;
        static DataSet projectDetailTable;
        static DataSet imageDetailTable;
        static DataSet stepActionTable;
        static DataSet projectActionToolTable;
        static DataSet projectModuleResourceTable;
        static DataSet projectActionDimensionTable;
        static DataSet projectActionImageTable;
        static DataSet projectActionRiskTable;
        public void SetUserTable(DataSet tbl)
        {
            userTable = tbl;
        }
        public DataSet GetUserTable()
        {
            return userTable;
        }
    }

    public partial class HomePage : Window
    {
        string projectid;
        string[] sourcePath= new string[100];
        string[] pictureFilename = new string[100];
        string[] totalpictureinoneindex = new string[100];
        int imagecountdiff;
        string actionidstring;
        int liftinggearcount;
        int imagecount;
        int riskcount;
        int dimensioncount;
        int toolcount;
        int resourcecount;
        int actioncount;
        int stepcount;
        int modulecount;
        int rigtypecount;
        int rigtypeindex = 0;
        int moduleindex = 0;
        int stepindex = 0;
        int actionindex = 0;
        int resourceindex = 0;
        int toolsindex = 0;
        int dimensionindex = 0;
        int riskindex = 0;
        int imageindex = 0;
        int liftinggearindex = 0;

        conn connectiondb = new conn();
        DataSet ds = new DataSet();
        SqlDataAdapter da = new SqlDataAdapter();

        XElement xelement = XElement.Load("../../Assets/ProjectXML.xml");

        List<string> rigdata = new List<string>();
        List<string> moduledataname = new List<string>();
        List<string> moduledatanumber = new List<string>();
        List<string> moduleid = new List<string>();
        List<string> stepdataname = new List<string>();
        List<string> stepdatanumber = new List<string>();
        List<string> stepid = new List<string>();
        List<string> actionid = new List<string>();
        List<string> actiondata = new List<string>();
        List<string> actiondatanumber = new List<string>();
        List<string> actiondatadetails = new List<string>();
        List<string> actiondetailsdata = new List<string>();
        List<string> resourcedata = new List<string>();
        List<string> liftinggearsdata = new List<string>();
        List<string> toolsdata = new List<string>();
        List<string> dimensiondata = new List<string>();
        List<string> riskdata = new List<string>();
        List<string> imagedata = new List<string>();

        public HomePage()
        {
            InitializeComponent();
            IEnumerable<XElement> name = xelement.Elements();
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {

            //WCF Service
            Services.FacilityDocuServiceClient client = new Services.FacilityDocuServiceClient();
            if (client.Login(userName.Text, password.Password))
            {
                grdViewLogin.Visibility = Visibility.Collapsed;
                gridHomePage.Visibility = Visibility.Visible;
            }
            else
            {
                password.Password = "";
                MessageBox.Show("Invalid Username or Password");
            }
        }
        public void HideHomePage()         //--Hiding HomePage 
        {
            gridHomePage.Visibility = Visibility.Collapsed;
        }

        public void ShowHomePage()       //--Back to HomePage
        {
            gridHomePage.Visibility = Visibility.Visible;
        }
        public void MouseEnter()
        {
            if (this.Cursor != Cursors.Wait)

                Mouse.OverrideCursor = Cursors.Hand;
        }
        public void MouseLeave()
        {
            if (this.Cursor != Cursors.Wait)

                Mouse.OverrideCursor = Cursors.Arrow;
        }

        public void mergenewandoldimage()
        {
            for(int i=0;i< listboxPicture.Items.Count;i++)
            {
                if (i < imagedata.Count)
                    totalpictureinoneindex[i] = imagedata[i];
                else
                    totalpictureinoneindex[i] = pictureFilename[i - imagedata.Count];
            }
        }

        private void LoadProject_btn_Click(object sender, RoutedEventArgs e)
        {
            HideHomePage();
            loadProject_grid.Visibility = Visibility.Visible;

            var DATA = from query in xelement.Descendants("project").Elements("name")
                       select new
                       {
                           name = query.Value,
                       };
            foreach (var names in DATA)
            {
                ProjectList.Items.Add(names.name);
            }
        }

        private void NewProject_btn_Click(object sender, RoutedEventArgs e)
        {
            AllProject_ListBox.Items.Clear();
            ProjectName.Text = "";
            projectName_txt.Text = "";
            des_txtbox.Text = "";
            creationdate_txt.Text = "";
            createdby_txt.Text = "";
            updatedate_txt.Text = "";
            updatedby_txt.Text = "";
            newProject_grid.Visibility = Visibility.Visible;
            homePage.Title = "FacilityDocu - New Project";

            var DATA = from query in xelement.Descendants("project").Elements("name")
                       select new
                       {
                           name = query.Value,
                       };
            foreach (var names in DATA)
            {
                AllProject_ListBox.Items.Add(names.name);
            }
        }

        private void LastProject_btn_Click(object sender, RoutedEventArgs e)
        {
            HideHomePage();
            lastProject_grid.Visibility = Visibility.Visible;
        }

        private void ImportPhoto_btn_Click(object sender, RoutedEventArgs e)
        {
            HideHomePage();
            importPhotos_grid.Visibility = Visibility.Visible;
        }

        private void lastProject_back_Click(object sender, RoutedEventArgs e)
        {
            ShowHomePage();
            lastProject_grid.Visibility = Visibility.Collapsed;
        }

        private void newProject_back_Click(object sender, RoutedEventArgs e)
        {
         
            ShowHomePage();
            newProject_grid.Visibility = Visibility.Collapsed;
        }

        private void loadProject_back_Click(object sender, RoutedEventArgs e)
        {
            ShowHomePage();
            loadProject_grid.Visibility = Visibility.Collapsed;
        }

        private void importPhotos_back_Click(object sender, RoutedEventArgs e)
        {
            ShowHomePage();
            importPhotos_grid.Visibility = Visibility.Collapsed;
        }

        private void addPictures_btn_Click(object sender, RoutedEventArgs e)
        {
          var ofd = new OpenFileDialog() { Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png" };
            var result = ofd.ShowDialog();
            if (result == false) return;
                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.CacheOption = BitmapCacheOption.OnLoad;
            Image imagename = new Image();
            src.UriSource = new Uri(ofd.FileName.Trim(), UriKind.Relative);
            imagename.Source = src;
            listboxPicture.Items.Add(imagename);
            src.EndInit();
           

            imagecountdiff = listboxPicture.Items.Count - imagedata.Count;
            pictureFilename[imagecountdiff-1]= System.IO.Path.GetFileName(ofd.FileName);
            FileInfo fileInfo = new FileInfo(ofd.FileName);
            sourcePath[imagecountdiff-1] = fileInfo.DirectoryName;
        }

        private void AllProject_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ProjectName.Text = AllProject_ListBox.SelectedItem.ToString();
            }
            catch
            { }
        }

        private void ok_btn_Click(object sender, RoutedEventArgs e)
        {
            des_txtbox.Text = "";
            creationdate_txt.Text = "";
            updatedate_txt.Text = "";
            createdby_txt.Text = "";
            updatedby_txt.Text = "";
            projectName_txt.Text = ProjectName.Text;
            //////////////////////////////////////////////////////////////////////////////////////////
            var DATA = from query in xelement.Descendants("project").Where(query => query.Element("name").Value == ProjectName.Text).Elements("description")
                       select new
                       {
                           name = query.Value,
                       };
            foreach (var names in DATA)
            {
                des_txtbox.Text = names.name.ToString();

            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var DATA1 = from query in xelement.Descendants("project").Where(query => query.Element("name").Value == ProjectName.Text).Elements("createdtime")
                        select new
                        {
                            name = query.Value,
                        };
            foreach (var names in DATA1)
            {
                creationdate_txt.Text = names.name.ToString();

            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////
            var DATA2 = from query in xelement.Descendants("project").Where(query => query.Element("name").Value == ProjectName.Text).Elements("updatetime")
                        select new
                        {
                            name = query.Value,
                        };
            foreach (var names in DATA2)
            {
                updatedate_txt.Text = names.name.ToString();

            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var DATA3 = from query in xelement.Descendants("project").Where(query => query.Element("name").Value == ProjectName.Text).Elements("createdby")
                        select new
                        {
                            name = query.Value,
                        };
            foreach (var names in DATA3)
            {
                createdby_txt.Text = names.name.ToString();

            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            var DATA4 = from query in xelement.Descendants("project").Where(query => query.Element("name").Value == ProjectName.Text).Elements("updatedby")
                        select new
                        {
                            name = query.Value,
                        };
            foreach (var names in DATA4)
            {
                updatedby_txt.Text = names.name.ToString();
            }
        }
        private void ProjectList_StylusInRange(object sender, StylusEventArgs e)
        {
        }

        private void ProjectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var DATA5 = from query in xelement.Descendants("project").Where(query => query.Element("name").Value == ProjectList.SelectedItem.ToString()).Elements("description")
                        select new
                        {
                            name = query.Value,
                        };
            foreach (var names in DATA5)
            {
                prj_Descriptiontxt.Text = names.name.ToString();

            }
        }
        private void editRisk_btn_Click(object sender, RoutedEventArgs e)
        {
            homePage.Title = "FacilityDocu - Edit Risk";
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) //_ProjectName_HyperLink
        {
            if (projectName_txt.Text != "")
            {
                editPage_grid.Visibility = Visibility.Visible;
                gridHomePage.Visibility = Visibility.Collapsed;
                homePage.Title = "FacilityDocu - Edit Project";
                /////////////////////////////////////////////////////////////////////////
                var ID = from query in xelement.Descendants("project").Where(query => query.Element("name").Value == ProjectName.Text).Elements("id")
                         select new
                         {
                             name = query.Value,
                         };
                foreach (var names in ID)
                {
                    txtProjectID.Text = names.name.ToString();
                    projectid = names.name.ToString();

                }
                txtProjectDescription.Text = des_txtbox.Text;
                ////////////////////////////////////////////////////////////////////////////////////////
                Rig();
                Module();
                Step();
                Action();
                //  ActionDetails();
                Images();
                //Resources();
                LiftingGears();
                Tools();
                Dimension();
                Risk();
                listboxPicture.Items.Clear();
                imageload();
            }
        }

        private void txtblockActivity_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseEnter();
        }

        private void txtblockActivity_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseLeave();
        }

        private void editHierarchy_btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            homePage.Title = "FacilityDocu - Edit Hierarchy";
            editPage_grid.Visibility = Visibility.Collapsed;
            grid_EditHierarchy.Visibility = Visibility.Visible;

            for (int i = 0; i < moduledataname.Count; i++)
                listboxModules.Items.Add(moduledataname[i]);

            for (int i = 0; i < stepdataname.Count; i++)
            listboxSteps.Items.Add(stepdataname[i]);

            for (int i = 0; i < actiondata.Count; i++)
                listboxActions.Items.Add(actiondata[i]);
        }

        private void editHierarchy_btn_MouseEnter_1(object sender, MouseEventArgs e)
        {
            MouseEnter();
        }

        private void editHierarchy_btn_MouseLeave_1(object sender, MouseEventArgs e)
        {
            MouseLeave();
        }

        private void projectName_txt_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseEnter();
        }

        private void projectName_txt_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseLeave();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e) //Logout Button
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Do you want to log out?", "Log out Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                //log out activity
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e) //RigType Previous Button
        {
            if (rigtypeindex > 0)
            {
                txtRigType.Text = rigdata[--rigtypeindex];
            }
            else { 
                txtRigType.Text = rigdata[rigtypeindex];  }
            Module();
            Step();
            Action();
            //Resources();
            LiftingGears();
            Tools();
            Dimension();
            Risk();
            Images();
            listboxPicture.Items.Clear();
            imageload();
       }

        private void btnNext_Click(object sender, RoutedEventArgs e)  //RigType Next Button
        {
            if (rigtypeindex < rigtypecount - 1)
            {
                txtRigType.Text = rigdata[++rigtypeindex];
            }
            else
            {
                txtRigType.Text = rigdata[rigtypeindex];
            }
            Module();
            Step();
            Action();
            //Resources();
            LiftingGears();
            Tools();
            Dimension();
            Risk();
            Images();
            listboxPicture.Items.Clear();
            imageload();
       }

        public void Rig()
        {
            var RigData = from query in xelement.Descendants("project").Where(query => query.Element("name").Value == ProjectName.Text)
                            .Elements("rigs")
                            .Elements("rig")
                          select new
                          {
                              name = query.Attribute("type").Value,
                          };
            rigdata.Clear();
            moduledataname.Clear();
            moduledatanumber.Clear();
            stepdataname.Clear();
            stepdatanumber.Clear();
            actiondata.Clear();
            foreach (var names in RigData)
            {
                rigdata.Add(names.name.ToString());
              

            }
            rigtypecount = rigdata.Count;

            if (rigtypecount > 0)
                txtRigType.Text = rigdata[rigtypeindex];

            else
                txtRigType.Text = "";
        }

        public void Module()
        {
            var ModuleData = from query in xelement.Descendants("project").Where(query => query.Element("name").Value == ProjectName.Text)
                                 .Elements("rigs")
                                 .Elements("rig").Where(query => (string)query.Attribute("type") == txtRigType.Text)
                                 .Elements("modules")
                                 .Elements("module")
                             select new
                             {
                                 name = query.Element
                                        ("name").Value,
                                 number = query.Element
                                        ("number").Value,
                                 id= query.Element
                                        ("id").Value
                             };
            moduleindex = 0;
            moduledataname.Clear();
            moduledatanumber.Clear();
            stepdataname.Clear();
            stepdatanumber.Clear();
            actiondata.Clear();
            foreach (var names in ModuleData)
            {
                moduledataname.Add(names.name.ToString());
                moduledatanumber.Add(names.number.ToString());
                moduleid.Add(names.id.ToString());
            }
            modulecount = moduledataname.Count;
            if (modulecount > 0)
            {
                txtModule.Text = moduledataname[moduleindex];
                txtModuleID.Text = moduledatanumber[moduleindex];
            }
            else
            {
                txtModule.Text = "";
                txtModuleID.Text = "";
            }
        }

        public void Step()
        {
            var StepData = from query in xelement.Descendants("project").Where(query => query.Element("name").Value == ProjectName.Text)
                                 .Elements("rigs")
                                 .Elements("rig").Where(query => (string)query.Attribute("type") == txtRigType.Text)
                                 .Elements("modules")
                                 .Elements("module").Where(query => (string)query.Element("name").Value == txtModule.Text)
                                 .Elements("steps")
                                 .Elements("step")

                           select new
                           {
                               number = query.Element("number").Value,
                               name = query.Element("name").Value,
                               id = query.Element("id").Value,
                           };
            stepindex = 0;
            stepdataname.Clear();
            stepdatanumber.Clear();
            actiondata.Clear();
            foreach (var names in StepData)
            {
                stepdataname.Add(names.name.ToString());
                stepdatanumber.Add(names.number.ToString());
                stepid.Add(names.id.ToString());

            }
            stepcount = stepdataname.Count;
            if (stepcount > 0)
            {
                txtStepName.Text = stepdataname[stepindex];
                txtStepNumber.Text = stepdatanumber[stepindex];
            }
            else
            {
                txtStepName.Text = "";
                txtStepNumber.Text = "";
            }
        }

        public void Action()
        {
            var ActionData = from query in xelement.Descendants("project").Where(query => query.Element("name").Value == ProjectName.Text)
                                             .Elements("rigs")
                                             .Elements("rig").Where(query => (string)query.Attribute("type") == txtRigType.Text)
                                             .Elements("modules")
                                             .Elements("module").Where(query => (string)query.Element("name").Value == txtModule.Text)
                                             .Elements("steps")
                                             .Elements("step").Where(query => (string)query.Element("name").Value == txtStepName.Text)
                                             .Elements("actions")
                                             .Elements("action")
                                            
                             select new
                             {
                                   actionid=query.Element("id").Value,
                                   number=query.Element("number").Value,
                                   detail = query.Element("description").Value,
                                   name = query.Element("name").Value
                             };
            actionindex = 0;
            actiondata.Clear();
            actionid.Clear();
            actiondatadetails.Clear();
            actiondatanumber.Clear();
            resourcedata.Clear();

            foreach (var names in ActionData)
            {
                actionid.Add(names.actionid.ToString());
                actiondatadetails.Add(names.detail.ToString());
                actiondata.Add(names.name.ToString());
                actiondatanumber.Add(names.number.ToString());
            }
            actioncount = actiondata.Count;
            if (actioncount > 0)
            {
               
                txtAction.Text = actiondata[actionindex];
                txtDetails.Text=actiondatadetails[actionindex];
                txtActionNumber.Text = actiondatanumber[actionindex];

            }
            else
            {
                txtAction.Text = "";
                txtDetails.Text = "";
                txtActionNumber.Text ="";
            }
        }

  /*     public void Resources()
        {
            var ResourcesData = from query in xelement.Descendants("project")
                                .Where(query => query.Element("name")
                                    .Value == ProjectName.Text)
                                    .Elements("rigs")
                                             .Elements("rig").Where(query => (string)query.Attribute("type") == txtRigType.Text)
                                             .Elements("modules")
                                             .Elements("module").Where(query => (string)query.Element("name").Value == txtModule.Text)
                                             .Elements("steps")
                                             .Elements("step").Where(query => (string)query.Element("name").Value == txtStepName.Text)
                                             .Elements("actions")
                                             .Elements("action").Where(query => (string)query.Element("name").Value == txtAction.Text)
                                             .Elements("resources")
                                             .Elements("resource")
                                             .Elements("name")
                                            
                                select new
                                {
                                    name = query.Value,
                                };
            
            resourceindex = 0;
            resourcedata.Clear();
            listBoxResources.Items.Clear();
            foreach (var names in ResourcesData)
            {
                resourcedata.Add(names.name.ToString());
                listBoxResources.Items.Add(resourcedata[resourceindex++]);
            }
            resourcecount = resourcedata.Count;      
        }*/

        public void LiftingGears()
        {

            var LiftingGearsData = from query in xelement.Descendants("project")
                                .Where(query => query.Element("name")
                                .Value == ProjectName.Text)
                                .Elements("rigs")
                                .Elements("rig").Where(query => (string)query.Attribute("type") == txtRigType.Text)
                                .Elements("modules")
                                .Elements("module").Where(query => (string)query.Element("name").Value == txtModule.Text)
                                .Elements("steps")
                                .Elements("step").Where(query => (string)query.Element("name").Value == txtStepName.Text)
                                .Elements("actions")
                                .Elements("action").Where(query => (string)query.Element("name").Value == txtAction.Text)
                                .Elements("liftinggears")
                                select new
                                {
                                    name = query.Value,
                                };
            liftinggearindex = 0;
           liftinggearsdata.Clear();
            foreach (var names in LiftingGearsData)
            {
                 liftinggearsdata.Add(names.name.ToString());
            }
            liftinggearcount = liftinggearsdata.Count;
            if (liftinggearcount> 0)
            {
                txtGears.Text = liftinggearsdata[liftinggearindex ];
            }
            else
            {
              txtGears.Text= "";
            }
            



        }
        public void Tools()
        {
            var ToolsData = from query in xelement.Descendants("project")
                                .Where(query => query.Element("name")
                                    .Value == ProjectName.Text)
                                    .Elements("rigs")
                                             .Elements("rig").Where(query => (string)query.Attribute("type") == txtRigType.Text)
                                             .Elements("modules")
                                             .Elements("module").Where(query => (string)query.Element("name").Value == txtModule.Text)
                                             .Elements("steps")
                                             .Elements("step").Where(query => (string)query.Element("name").Value == txtStepName.Text)
                                             .Elements("actions")
                                             .Elements("action").Where(query => (string)query.Element("name").Value == txtAction.Text)
                                             .Elements("tools")
                                             .Elements("tool")
                                             .Elements("name")
                            select new
                            {
                                name = query.Value,
                            };
            toolsindex = 0;
            toolsdata.Clear();
            listboxTools.Items.Clear();
            foreach (var names in ToolsData)
            {
                toolsdata.Add(names.name.ToString());
                listboxTools.Items.Add(toolsdata[toolsindex++]);
            }
            toolcount = toolsdata.Count;
        }

        public void Dimension()
        {
            var DimensionData = from query in xelement.Descendants("project")
                                .Where(query => query.Element("name")
                                .Value == ProjectName.Text)
                                .Elements("rigs")
                                .Elements("rig").Where(query => (string)query.Attribute("type") == txtRigType.Text)
                                .Elements("modules")
                                .Elements("module").Where(query => (string)query.Element("name").Value == txtModule.Text)
                                .Elements("steps")
                                .Elements("step").Where(query => (string)query.Element("name").Value == txtStepName.Text)
                                .Elements("actions")
                                .Elements("action").Where(query => (string)query.Element("name").Value == txtAction.Text)
                                .Elements("dimensions")
                                select new
                                {
                                    name = query.Value,
                                };
                dimensionindex = 0;
                dimensiondata.Clear();
            foreach (var names in DimensionData)
            {
                dimensiondata.Add(names.name.ToString());
            }
            dimensioncount = dimensiondata.Count;
            if(dimensioncount>0)
            {
                txtDimensions.Text = dimensiondata[actionindex];
            }
            else
            {
                txtDimensions.Text = "";
            }
            
        }
        public void Risk()
        {
            var RiskData = from query in xelement.Descendants("project")
                                .Where(query => query.Element("name")
                                .Value == ProjectName.Text)
                                .Elements("rigs")
                                .Elements("rig").Where(query => (string)query.Attribute("type") == txtRigType.Text)
                                .Elements("modules")
                                .Elements("module").Where(query => (string)query.Element("name").Value == txtModule.Text)
                                .Elements("steps")
                                .Elements("step").Where(query => (string)query.Element("name").Value == txtStepName.Text)
                                .Elements("actions")
                                .Elements("action").Where(query => (string)query.Element("name").Value == txtAction.Text)
                                .Elements("risks")
                                select new
                                {
                                    name = query.Value,
                                };
            riskindex = 0;
            riskdata.Clear();
            foreach (var names in RiskData)
            {
                riskdata.Add(names.name.ToString());
            }
            riskcount = riskdata.Count;
            if (riskcount > 0)
            {
                txtRisks.Text = riskdata[actionindex];
            }
            else
            {
                txtRisks.Text ="";
            }
        }
        public void Images()
        {
            var ImageData = from query in xelement.Descendants("project")
                               .Where(query => query.Element("name")
                               .Value == ProjectName.Text)
                               .Elements("rigs")
                               .Elements("rig").Where(query => (string)query.Attribute("type") == txtRigType.Text)
                               .Elements("modules")
                               .Elements("module").Where(query => (string)query.Element("name").Value == txtModule.Text)
                               .Elements("steps")
                               .Elements("step").Where(query => (string)query.Element("name").Value == txtStepName.Text)
                               .Elements("actions")
                               .Elements("action").Where(query => (string)query.Element("name").Value == txtAction.Text)
                               .Elements("images")
                               .Elements("image")
                               .Elements("name")
                            select new
                            {
                                name = query.Value
                            };
            imageindex = 0;
            imagedata.Clear();
            foreach (var names in ImageData)
            {
                imagedata.Add(names.name.ToString());

            }
            imagecount = imagedata.Count;

        }
        public void imageload()
        {
            MessageBox.Show(imagecount.ToString());
             for (int i = 0; i < imagedata.Count; i++)
              {
            BitmapImage bimg = new BitmapImage();
            Image imagename = new Image();
            //bimg.UriSource = new Uri(@"H:/DataP/4001/Image1.png");
            bimg.BeginInit();
            bimg.UriSource = new Uri(@"C:/Users/Yashasvi/Pictures/DataP/"+projectid+"."+ rigdata[rigtypeindex]+"."+ moduleid[moduleindex]+"."+stepid[stepindex]+"."+actionid[actionindex] + "/" + imagedata[i]);
            imagename.Source = bimg;
            bimg.EndInit();
            listboxPicture.Items.Add(imagename);
             }
        }

        public void createfolder()
        {
            if (!Directory.Exists(@"C:/Users/Yashasvi/Pictures/DataP/"+projectid+"."+ rigdata[rigtypeindex]+"."+ moduleid[moduleindex]+"."+stepid[stepindex]+"."+actionid[actionindex]))
            {
                Directory.CreateDirectory(@"C:/Users/Yashasvi/Pictures/DataP/"+projectid+"."+ rigdata[rigtypeindex]+"."+ moduleid[moduleindex]+"."+stepid[stepindex]+"."+actionid[actionindex]);
            }
        }

        private void btnModuleRight_Click_1(object sender, RoutedEventArgs e)
        {
            if (modulecount > 0)
            {
                if (moduleindex < modulecount - 1)
                {
                    txtModule.Text = moduledataname[++moduleindex];
                    txtModuleID.Text = moduledatanumber[moduleindex];
                }
                else
                {
                    txtModule.Text = moduledataname[moduleindex];
                    txtModuleID.Text = moduledatanumber[moduleindex];
                }
                Step();
                Action();
                //Resources();
                LiftingGears();
                Tools();

                Dimension();
                Risk();
                Images();
                listboxPicture.Items.Clear();
                imageload();

            }
        }

        private void btnModuleLeft_Click_1(object sender, RoutedEventArgs e)
        {
            if (modulecount > 0)
            {
                if (moduleindex > 0)
                {
                    txtModule.Text = moduledataname[--moduleindex];
                    txtModuleID.Text = moduledatanumber[moduleindex];
                }
                else
                {
                    txtModule.Text = moduledataname[moduleindex];
                    txtModuleID.Text = moduledatanumber[moduleindex];
                }
                Step();
                Action();
                //Resources();
                LiftingGears();
                Tools();
                Dimension();
                Risk();
                Images();
                listboxPicture.Items.Clear();
                imageload();
            }
        }

        private void btnStepRight_Click(object sender, RoutedEventArgs e)
        {
            if (stepcount > 0)
            {
                if (stepindex < stepcount - 1)
                {
                    
               
                 txtStepName.Text = stepdataname[++stepindex];
                  txtStepNumber.Text = stepdatanumber[stepindex];
                }
                else
                {
                txtStepName.Text = stepdataname[stepindex];
                txtStepNumber.Text = stepdatanumber[stepindex];
                   
                }
                Action();
                //Resources();
                LiftingGears();
                Tools();
                Dimension();
                Risk();
                Images();
                listboxPicture.Items.Clear();
                imageload();
            }
        }

        private void btnStepLeft_Click(object sender, RoutedEventArgs e)
        {
            if (stepcount > 0)
            {
                if (stepindex > 0)
                {
                    txtStepName.Text = stepdataname[--stepindex];
                   
                }
                else
                {
                    txtStepName.Text = stepdataname[stepindex];
                    
                }

                Action();
                //Resources();
                LiftingGears();
                Tools();
                Dimension();
                Risk();
                Images();
                listboxPicture.Items.Clear();
                imageload();
            }
        }

        private void btnActionRight_Click(object sender, RoutedEventArgs e)
        {
            if (actioncount > 0)
            {
                if (actionindex < actioncount - 1)
                {
                   txtAction.Text = actiondata[++actionindex];
                   txtActionNumber.Text = actiondatanumber[actionindex];
                   txtDetails.Text = actiondatadetails[actionindex];
                }         
                else
                {
                    txtAction.Text = actiondata[actionindex];
                    txtActionNumber.Text = actiondatanumber[actionindex];
                    txtDetails.Text = actiondatadetails[actionindex];
                }
                //Resources();
                LiftingGears();
                Tools();
                Dimension();
                Risk();
                Images();
                listboxPicture.Items.Clear();
                imageload();
            }
        }

        private void btnActionLeft_Click(object sender, RoutedEventArgs e)
        {
            if (actioncount > 0)
            {
                if (actionindex > 0)
                {
                    txtAction.Text = actiondata[--actionindex];
                    txtActionNumber.Text = actiondatanumber[actionindex];
                    txtDetails.Text = actiondatadetails[actionindex];

                }
                else
                {
                    txtAction.Text = actiondata[actionindex];
                    txtActionNumber.Text = actiondatanumber[actionindex];
                    txtDetails.Text = actiondatadetails[actionindex];
                }
                //Resources();
                LiftingGears();
                Tools();
                Dimension();
                Risk();
                Images();
                listboxPicture.Items.Clear();
                imageload();
            }
        }

        private void listboxTools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

      
        private void txtK_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //char c = Convert.ToChar(e.Text);
            //if (Char.IsNumber(c))
            //    e.Handled = false;
            //else
            //    e.Handled = true;

            //base.OnPreviewTextInput(e);

            //   Regex rx = new Regex("\\b(?<word>\\w+)\\s+(\\k<word>)\\b",
            //  RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex regex = new Regex("^[.][0-9]+$|^[0-9]*[.]{0,1}[0-9]*$");
            // Regex rx = new Regex(@"^-?\d+(\.\d{2})?$");
            e.Handled = !regex.IsMatch((sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text));
        }
        public void ProjectSave()
        {
          
            
            XDocument projectSave = new XDocument(
                        new XElement("project",
                   new XElement("id", txtProjectID.Text),
                   new XElement("name", projectName_txt.Text),
                   new XElement("description", des_txtbox.Text),
                   new XElement("createdtime", creationdate_txt.Text),
                   new XElement("createdby", createdby_txt.Text),
                   new XElement("updatedby", updatedby_txt.Text),
                   new XElement("updatetime", updatedate_txt.Text),
                   new XElement("rigs",
                   new XElement("rig",
                   new XAttribute("type", txtRigType.Text),
                   new XElement("modules",
                   new XElement("module",
                   new XElement("id", txtModuleID.Text),
                   new XElement("name", txtModule.Text),
                // new XElement("number",),

              new XElement("steps",
                      new XElement("step",
                // new XElement("id", txt),
                          new XElement("number", txtStepNumber.Text),
                         new XElement("name", txtStepName.Text),

                          new XElement("actions",
                      new XElement("action",
                          new XElement("id",actionid[actionindex]),
                          new XElement("number", txtActionNumber.Text),
                          new XElement("name", txtAction.Text),
                          new XElement("description", txtDetails.Text),
                          new XElement("images",
                           Enumerable.Range(0,listboxPicture.Items.Count).Select(i=>new XElement("image",
                                  new XElement("name",totalpictureinoneindex[i])))),


                          //new XElement("images",
                // new XElement("image",
                // new XElement("id", OfflineData.tempimageid),
                //new XElement("number", OfflineData.tempimage_no),
                //new XElement("name", OfflineData.imageName),
                //new XElement("tags", OfflineData.tempimagetag),
                //new XElement("description", OfflineData.tempimage_description),
                //new XElement("comments")
              
                new XElement("tools",
               Enumerable.Range(0,toolsdata.Count).Select(i=>new XElement("tool",
                //  new XElement("id",txtToolID.Text),
                new XElement("name", listboxTools.Items[i].ToString()))),

                    new XElement("resources",
                   Enumerable.Range(0,resourcedata.Count).Select(i=> new XElement("resource",
                //new XElement("id", txtResourceID.Text),
                            new XElement("name", "test"))),
                //    new XElement("count",hh )

                        new XElement("risks",
                            txtRisks.Text),

                          new XElement("dimensions",
                     txtDimensions.Text),

                       new XElement("liftinggears",
                     txtGears.Text)

                    ))))))

                                          )))))); 

            projectSave.Save(@"H://Project.xml");
            MessageBox.Show("Succesfully Saved");
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            createfolder();
            ///////////////////////////////////////////////////////////////
            string targetPath = @"C:/Users/Yashasvi/Pictures/DataP/"+projectid+"."+ rigdata[rigtypeindex]+"."+ moduleid[moduleindex]+"."+stepid[stepindex]+"."+actionid[actionindex];
          //  FileInfo fileInfo;
            for (int i = 0; i < imagecountdiff; i++)
            {
               string sourceFile = System.IO.Path.Combine(sourcePath[i], pictureFilename[i]);
              string destFile = System.IO.Path.Combine(targetPath, pictureFilename[i]);
         
               System.IO.File.Copy(sourceFile, destFile, true);
            }
            mergenewandoldimage();
        /////////////////////////////////////////////////////////////////////////////
            ProjectSave();
        }
    }
}
