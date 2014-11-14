using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tablet_App.ServiceReference1;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Tablet_App
{


    class Data
    {
        //wcf service
       // public static ServiceReference1.Service1Client MyService = new ServiceReference1.Service1Client();

        public static ProjectDTO CURRENT_PROJECT;
        public static RigTypeDTO CURRENT_RIG;
        public static ModuleDTO CURRENT_MODULE;
        public static StepDTO CURRENT_STEP;
        public static ActionDTO CURRENT_ACTION;
        public static ImageDTO MODIFYIMAGE;

        public static int menuClick = 0;

        public static string ProjectXmlPath = "";         //xml path
        public static string ImagesPath = "";           //data path
        public static string BackupPath = "";          //backup path

        public static bool IsFromCrop;
    }
}
