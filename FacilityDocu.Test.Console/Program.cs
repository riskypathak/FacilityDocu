﻿using FacilityDocu.Test.Console.Services;
using FacilityDocu.UI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestUpdateProjectXml();
            //TestUpdateProjectDatabase(1);

            //TestProjectSync();

            //TestActionImageUpload();
            TestGeneratePdf();
        }

        private static void TestProjectSync()
        {
            SyncManager manager = new SyncManager();
            manager.IsSyncRequired();
        }

        private static void TestUpdateProjectXml()
        {
            SyncManager manager = new SyncManager(new List<int> { 1});
            manager.UpdateProjectXml(null);
        }

        private static void TestUpdateProjectDatabase(int projectID)
        {
            SyncManager manager = new SyncManager();
            manager.UpdateDatabase("1", false);
        }

        private static void TestActionImageUpload()
        {
            SyncManager manager = new SyncManager();
            manager.UploadImages("54");
        }

        private static void TestGeneratePdf()
        {
            string xmlPath = Path.GetFullPath(string.Format("Data/ProjectXml/{0}.xml", "20150227131059550"));

            IList<string> exportPage = new List<string>();
            exportPage.Add("UP_");

            Helper.GeneratePdf(ProjectXmlReader.ReadProjectXml(xmlPath, false), "a4", exportPage, string.Empty, "");
        }
    }
}
