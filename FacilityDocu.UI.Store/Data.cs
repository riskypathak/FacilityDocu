using System.Threading.Tasks;
using Tablet_App.ServiceReference1;
using System;

namespace Tablet_App
{
    class Data
    {
        public static ProjectDTO CURRENT_PROJECT;
        public static RigTypeDTO CURRENT_RIG;
        public static ModuleDTO CURRENT_MODULE;
        public static StepDTO CURRENT_STEP;
        public static ActionDTO CURRENT_ACTION;
        public static ImageDTO MODIFYIMAGE;

        public static int menuClick = 0;

        public static string ProjectXmlPath = string.Empty;         
        public static string ImagesPath = string.Empty;
        public static string BackupPath = string.Empty;

        public static bool IsFromCrop;

        public async static Task<string> GetUserName()
        {
            string userName = string.Format("{0} {1}", await Windows.System.UserProfile.UserInformation.GetFirstNameAsync()
                   , await Windows.System.UserProfile.UserInformation.GetLastNameAsync());
            return userName;
        }
    }
}
