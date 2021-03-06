﻿using System.Threading.Tasks;
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
        public static object menuClick;
        public static string ProjectXmlPath = string.Empty;
        public static string ImagesPath = string.Empty;
        public static string BackupPath = string.Empty;
        public static bool IsFromCrop;
        public static bool SYNC_PROCESS;
        public static bool PUBLISH_PROCESS;

        public static string ACTION_SELECT_PROJECT_ID;
        public static string ACTION_SELECT_RIGTYPE_ID;
        public static string ACTION_SELECT_MODULE_ID;
        public static string ACTION_SELECT_STEP_ID;
        public static string ACTION_SELECT_ACTION_ID;
 

        public async static Task<string> GetUserName()
        {
            string userName = string.Format("{0}", await Windows.System.UserProfile.UserInformation.GetDisplayNameAsync());
            return userName;
        }
    }
}