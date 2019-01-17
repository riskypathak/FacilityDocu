using FacilityDocu.DTO;
using FacilityDocu.UI.Utilities.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Xml.Linq;

namespace FacilityDocu.UI.Utilities
{
    public static class Helper
    {
        public static string GetRisk(string l, string s, out string bgColor)
        {
            string risk = "";

            Dictionary<string, int> mapping = new Dictionary<string, int>()
            { { "A", 1}, { "B", 2},{ "C", 3},{ "D", 4},{ "E", 5}};

            if (!string.IsNullOrEmpty(l) && !string.IsNullOrEmpty(s))
            {
                int lNumeric = mapping[l];

                int result = lNumeric * Convert.ToInt16(s);

                if (result < 5)
                {
                    risk = $"Low {l}{s.ToString()}";
                    bgColor = "Green";
                }
                else if (result >= 10)
                {
                    risk = $"High {l}{s.ToString()}";
                    bgColor = "Red";
                }
                else
                {
                    risk = $"MED {l}{s.ToString()}";
                    bgColor = "Yellow";
                }
            }
            else
            {
                risk = "";
                bgColor = "White";
            }

            return risk;
        }

        public static bool isInternetAvailable()
        {
            bool isInternetAvailable = false;
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadString("http://www.google.com");
                isInternetAvailable = true;
            }
            catch (WebException exception)
            {

            }

            return isInternetAvailable;
        }

        public static bool Login(string userName, string password, out string role)
        {
            bool isLogin = false;
            role = "";
            XDocument xdoc = XDocument.Load(Data.CONFIG_PATH);

            try
            {
                IFacilityDocuService service = new FacilityDocuServiceClient();
                role = service.Login(userName, password);

                if (!string.IsNullOrEmpty(role))
                {
                    isLogin = true;
                    xdoc.Element("config").Element("lastlogin").Value = userName;
                    xdoc.Element("config").Element("lastrole").Value = role;
                }
            }
            catch (EndpointNotFoundException)
            {
                if (xdoc.Element("config").Element("lastlogin").Value.Equals(userName))
                {
                    isLogin = true;
                    role = xdoc.Element("config").Element("lastrole").Value;
                }
            }

            return isLogin;
        }

        public static void AddTemplate(ProjectDTO projectDTO)
        {
            IFacilityDocuService service = new FacilityDocuServiceClient();

            service.CreateTemplate(projectDTO);
        }

        public static ProjectDTO GetTemplate(int templateID)
        {
            IFacilityDocuService service = new FacilityDocuServiceClient();

            return service.GetProjectDetails(templateID);
        }

        public static bool IsNew(string id)
        {
            bool returnValue;
            returnValue = (id.Length >= 15 || id.Equals("0") || id.StartsWith("-")) ? true : false;

            return returnValue;
        }

        public static void WriteLog(string message, System.Diagnostics.EventLogEntryType type = System.Diagnostics.EventLogEntryType.Information)
        {
            System.Diagnostics.EventLog appLog = new System.Diagnostics.EventLog();
            appLog.Source = "FacilityDocu-LaptopApp";
            appLog.WriteEntry(message, type);
        }
    }
}
