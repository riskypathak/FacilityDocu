using FacilityDocu.UI.Utilities.Services;
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

        public static bool SYNCPROCESS = false;

        public static ProjectDTO CURRENT_PROJECT;
    }
}
