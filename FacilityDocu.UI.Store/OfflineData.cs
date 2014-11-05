using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Tablet_App
{


    class OfflineData
    {
        //wcf service
        public static ServiceReference1.Service1Client MyService = new ServiceReference1.Service1Client();

        public static int menuClick = 0;
        public static int picCount = 0;
        public static string folderName;
        public static string projectXml;

        public static string cuser = "mohan";
        public static string inumber;
        public static int profileflag = 0;
        public static string pn = "";
        public static string ides = "";
        public static string itag = "";
        //for final page
        public static int initialcome = 0;
        public static Image pImage = new Image();

        //main page load -- setting page -- 
        public static string xmlp = "";         //xml path
        public static string datap = "";           //data path
        public static string backupp = "";          //backup path

        public static int backup_flag = 0;
        public static int xml_flag = 0;
        public static int image_flag = 0;

       //camera page--store the multiple capture images
        public static List<BitmapImage> imagebk = new List<BitmapImage>();
   
        public static BitmapImage editpic = new BitmapImage();
       
        public static StorageFile tempPhotoloc;
        

        //project nedded
        public static string tempProjectID;
        public static string tempProjectName;
        public static string tempProjectDescription;
        public static string tempProjectCreatedBy="User";
        public static string tempProjectCreatedDate;
        public static string tempProjectUpdateDate;

        //rig type
        public static string tempRigType;
      

        //category--module
        public static string tempModuleID;
        public static string tempModuleName;
        public static string tempModuleNo;

        //subcategory--step
        public static string tempStepID;
        public static string tempStepName;
        public static string tempStepNo;
       
       //action
        public static string tempActionID;
        public static string tempActionName;
        public static string tempActionNo;
        public static string tempActionDescription;

        //image
        public static string tempimageid;
        public static string tempimage_no;
        public static string imageName;
        public static string tempimagecomment;
        public static string tempimagetag;
        public static string tempimage_description;

        //comment
        public static string tempcommenttext;
        public static string tempcommentdate;
        public static string tempcommentuser;
       

       
        //edit page for undo redo event
        public static List<BitmapImage> undoImage = new List<BitmapImage>();
       
      
    }
}
