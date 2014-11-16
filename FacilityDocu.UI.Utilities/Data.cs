﻿using FacilityDocu.UI.Utilities.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.UI.Utilities
{
    public static class Data
    {
        public static string PROJECT_XML_FOLDER = System.IO.Path.GetFullPath("Data/ProjectXml");
        public static string PROJECT_IMAGES_FOLDER = System.IO.Path.GetFullPath("Data/Images");
        public static string PROJECT_ATTACHMENTS_FOLDER = System.IO.Path.GetFullPath("Data/Attachments");
        public static string PROJECT_OUTPUT_FOLDER = System.IO.Path.GetFullPath("Data/Output");
        public static string CONFIG_PATH = System.IO.Path.GetFullPath("Assets/config.xml");
        public static string ASSETS_PATH = System.IO.Path.GetFullPath("Assets");

        public static bool SYNC_DOWNLOAD = false;
        public static bool SYNC_DOWNLOAD_UPDATE = false;

        public static ProjectDTO CURRENT_PROJECT;

        public static string CURRENT_USER;

        public static IList<ToolDTO> AVAILABLE_TOOLS = null;
    }
}