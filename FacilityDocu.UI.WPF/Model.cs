using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocLaptop
{
    public class Model
    {

        static string index;
        public void Setindex(string i)
        {
            index = i;
        }
        public string Getindex()
        {
            return index;
        }
        public string GetModuleName()
        {
            string modulename;
            
            switch(index)
            {
                case "01.01":
                case "01.02":
                case "01.03":
                 modulename = "General preparations";
                 Setindex("1");
                break;

                case "01.04":
                modulename = "Mud return line";
                Setindex("1");
                break;

                case "01.05":
                case "01.06":
                case "01.07":
                modulename = "Utility skid";
                Setindex("1");
                break;


                case "01.08":
                modulename = "Shaker tank";
                Setindex("1");
                break;

                case "01.09":
                modulename = "Centrifuges";
                Setindex("1");
                break;


                case "01.10":
                modulename = "Intermediate tank";
                Setindex("1");
                break;


                case "01.11":
                modulename = "Reserve tank 1 till X";
                Setindex("1");
                break;
               
                //case "01.12":
                //modulename = "Suction tank";
                //Setindex("1");
                //break;

                case "01.13":
                modulename = "Suction tank";
                Setindex("1");
                break;

                case "01.14":
                modulename = "Charge pump skid";
                Setindex("1");
                break;

                case "01.15":
                modulename = "Pre-mix slug tank";
                Setindex("1");
                break;

                case "01.16":
                case "01.17":
                modulename = "Silo dock skid";
                Setindex("1");
                break;

                case "01.18":
                modulename = "General preparations";
                Setindex("1");
                break;
                case "01.19":
                modulename = "General preparations";
                Setindex("1");
                break;
                case "01.20":
                modulename = "General preparations";
                Setindex("1");
                break;
                case "01.21":
                modulename = "General preparations";
                Setindex("1");
                break;




                default :
                modulename = "";
                break;
            }
            
            
            
            return modulename;
        }

    }
}
