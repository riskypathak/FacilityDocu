using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Tablet_App
{
   public  class Component
    {


        static string Index;
        public void SetIndex (string i)
        {
            Index = i;

        }
        public string GetIndex()
        {
            return Index;
        }

       public string[] GetCategory()
        {
            string[] category = new string[15];
           switch(Index)
           {
               case "1":
                category[0]="Unload and Rig -Up Mud System";
                category[1]="Unload And Rig -Up Mud Pumps";
                category[2]="Unload And Rig-Up Substructure";
                category[3]="Unload And Rig-Up Mast";
                category[4]="Actions after-and Raising Mast and Subs";
                category[5]="Rig -Up Top Drive";
                category[6]="Unload and Rig-Up Power Plant";
                category[7]="Unload and Rig-up Rig Camp";
                category[8]="Unload and Rig-Up Maain Camp";
                category[9]="Unload and Rig-Up Diesel Tanks";
                category[10]="Unload And Position Drill Pipe Baskets and Toolboxes";
                category[11]="Unload And Position Miscellaneous";
                SetIndex("12");
               break;


               case "2":
                 category[0]="Rig down and load Mud system";
                category[1]="Rig down and load Mud pumps";
                category[2]="Rig down Topdrive";
                category[3]="Actions before lowering mast & substructure";
                category[4]="Rig down and load Mast";
                category[5]="Rig down and load Substructure";
                category[6]="Rig diwn and load Power Plant";
                category[7]="Rig down and load Rig Camp";
                category[8]="Rig down and load Main Camp";
                category[9]="Rig down and load Diesel tanks";
                category[10]="Load Drill-pipe Baskets & Toolboxes";
                category[11]="Load Miscellaneous";

               SetIndex("12");
               break;

               case "3":
               category[0] = "no data";
               category[1] = "no data";
               category[2] = "no data";
               category[3] = "no data";
               category[4] = "no data";
               category[5] = "no data";
               category[6] = "no data";


               SetIndex("7");
               break;


               default :

               break;
           }
            return category;
        }

       public string[] GetSubCategory(int i)
       {
           string[] subcategory = new string[30];
           if (i == 2)
           {


               switch (Index)
               {
                   case "1":
                     
                       subcategory[0]="Flush and Clean the system";
                       subcategory[1]="Disconnect power cables";
                       subcategory[2]="Disconnect cascade system";
                       subcategory[3]="Mud return line";
                       subcategory[4]="Poor-boy degasser";
                       subcategory[5]="Choke manifold";
                       subcategory[6]="Trip tank";
                       subcategory[7]="Shaker tank";
                       subcategory[8]="Centrifuges";
                       subcategory[9]="Intermediate tank";
                       subcategory[10]="Reserve tank";
                       subcategory[11]="Interconnecting cabling skid";
                       subcategory[12]="Suction tank";
                       subcategory[13]="Charge pump skid";
                       subcategory[14]="Premix - slug tank";
                       subcategory[15]="Silo dock skid";
                       subcategory[16]="Mixing pump skid";
                       subcategory[17]="Silo";
                       subcategory[18]="Mud diesel tank";
                       subcategory[19]="Water tank";
                       subcategory[20]="Cement tank";

                       SetIndex("21");
                     
                       break;

                   case "2":
                      subcategory[0]="Disconnect cable";
                       subcategory[1]="Disconnect HP lines";
                       subcategory[2]="Disconnect LP lines";
                       subcategory[3]="Mud pumps";
                       subcategory[4]="HP discharge";

                       SetIndex("5");
                       break;

                   case "3":
                      subcategory[0]="TDS system - Rig 901 -> Prepare for Rig down";
                      subcategory[0]="TDS system - Rig 901 -> Elevators";
                      subcategory[0]="TDS system - Rig 901 -> rotary mud hose";
                      subcategory[0]="TDS system - Rig 901 -> TDS service-loop";
                      subcategory[0]="TDS system - Rig 901 -> stops/stoppers TDS";
                      subcategory[0]="TDS system - Rig 901 -> counter balance cylinders";
                      subcategory[0]="TDS system - Rig 901 -> TDS bail lock device";
                      subcategory[0]="TDS system - Rig 901 -> Traveling block/ TDS bail";
                      subcategory[0]="TDS system - Rig 901 -> Traveling block/ Tds bail slings}";
                      subcategory[0]="TDS system - Rig 901 -> Remove TDS";
                      subcategory[1]="TDS system - Rig 88";
                      subcategory[2]="TDS system - Rig 604";

                       SetIndex("3");
                       break;

                   case "4":
                      subcategory[0]="Disconnect hoses";
                       subcategory[1]="Disconnect cable";
                       subcategory[2]="Mud return";
                       subcategory[3]="BOP hoist";
                       subcategory[4]="Winches in substructure";
                       subcategory[5]="HP standpipe";
                       subcategory[6]="Stairs rigfloor mud system";
                       subcategory[7]="Emergency slide";
                       subcategory[8]="Rigfloor elevator";
                       subcategory[9]="Staircase";
                       subcategory[10]="Winches on rigfloor";
                       subcategory[11]="Slick line unit";
                       subcategory[12]="Rig floor panels";
                       subcategory[13]="Rig up slingshot system";
                       subcategory[14]="Inspection substructure";
                       subcategory[15]="Pins setback - slingshot gin pole";
                       subcategory[16]="Knee braces";
                       subcategory[17]="Pins gin pole leg - top box";
                       subcategory[18]="Lower substructure";
                       subcategory[19]="Gin poles";
                       subcategory[20]="Rig down slingshot system";
                       subcategory[21]="Raising line";
                       subcategory[22]="Service loop A-frame";
                       subcategory[23]="Remove platform DS";
                       subcategory[24]="Traveling block support frame";
                       subcategory[25]="Prepare mast for lowering";
                       subcategory[26]="Lower mast";

                       SetIndex("27");
                       break;

                   case "5":
                      subcategory[0]="Steel-wire cables";
                       subcategory[1]="Racking board";
                       subcategory[2]="From high to lower supports";
                       subcategory[3]="Reeve-out drrill-line";
                       subcategory[4]="Rotary mud hose";
                       subcategory[5]="TDS service loop";
                       subcategory[6]="Mast raising line";
                       subcategory[7]="Dismantling mast";
                       subcategory[8]="A-frame";

                       SetIndex("9");
                       break;

                   case "6":
                      subcategory[0]="Iron Roughneck";
                       subcategory[1]="Drillers cabin";
                       subcategory[2]="Doghouse";
                       subcategory[3]="Platforms";
                       subcategory[4]="Rotary table";
                       subcategory[5]="Draw works";
                       subcategory[6]="Disconnect trolley beams";
                       subcategory[7]="Stabilizer spreader";
                       subcategory[8]="Rotary support spreader";
                       subcategory[9]="Draw works spreader";
                       subcategory[10]="I.R.D. skid";
                       subcategory[11]="Setback";
                       subcategory[12]="Drip-pan";
                       subcategory[13]="Remove trolley beams";
                       subcategory[14]="Knee braces";
                       subcategory[15]="Base box";
                       subcategory[16]="Substructure";
                       subcategory[17]="Matting";

                       SetIndex("18");
                       break;

                   case "7":
                      subcategory[0]="Disconnect power cables";
                       subcategory[1]="Disconnect diesel fuel lines";
                       subcategory[2]="Disconnect air lines";
                       subcategory[3]="MCC house";
                       subcategory[4]="VFD power house";
                       subcategory[5]="Generator power house";
                       subcategory[6]="Grasshopper";

                       SetIndex("7");
                       break;

                   case "8":
                      subcategory[0]="Walkways";
                       subcategory[1]="Disconnect the camp";
                       subcategory[2]="Generator";
                       subcategory[3]="Fuel tank";
                       subcategory[4]="Filter unit";
                       subcategory[5]="Water tank";

                       subcategory[6]="Individual Units -> Work Shops";
                       subcategory[7]="Individual Units -> Crew Tea Room ";
                       subcategory[8]="Individual Units -> Cliniq Medic";
                       subcategory[9]="Individual Units -> Training Meeting Room";
                       subcategory[10]="Individual Units -> Ablution Unit";
                       subcategory[11]="Individual Units -> Office Mech. Elec.";
                       subcategory[12]="Individual Units -> Office Geo";
                       subcategory[13]="Individual Units -> Office Com. ";
                       subcategory[14]="Individual Units -> Office STP";
                       subcategory[15]="Individual Units -> 4 Man sleeper with bath";
                       subcategory[16]="Individual Units -> Sleeper Mech. Elec.";
                       subcategory[17]="Individual Units -> Sleeper Com. Man";
                       subcategory[18]="Individual Units -> Sleeper STP";

                       subcategory[19]="Master Skids";
                       subcategory[20]="Cement Blocks";

                       SetIndex("9");
                       break;
                   case "9":
                      subcategory[0]="Walkways";
                       subcategory[1]="Disconnect the camp";
                       subcategory[2]="Individual Units";
                       subcategory[3]="Master Skids";
                       subcategory[4]="Sewage Treatment Plant & Lifting tank";
                       subcategory[5]="Workshop";
                       subcategory[6]="Filter Unit";
                       subcategory[7]="Raw Water tank";
                       subcategory[8]="Portable water tank";
                       subcategory[9]="Mosque";
                       subcategory[10]="Generator Unit";
                       subcategory[11]="Fuel tank";

                       SetIndex("12");
                       break;
                   case "10":
                       
                       subcategory[0]="Disconnect power cables";
                       subcategory[1]="Disconnect diesel lines";
                       subcategory[2]="Mud diesel tanks";
                       subcategory[3]="Fuel diesel tanks";

                       SetIndex("4");
                       break;
                   case "11":
                       subcategory[0]="Drill-pip baskets";
                       subcategory[1]="Toolboxes";
                       SetIndex("2");
                       break;
                   case "12":
                       subcategory[0]="40 ft container";
                       subcategory[1]="20 ft container";
                       subcategory[2]="Miscellaneous equipment";
                       subcategory[3]="Mud chemicals";
                       subcategory[4]="Accumulator unit";
                       subcategory[5]="BOP control line";
                       subcategory[6]="V-door";
                       subcategory[7]="Automatic catwalk system";
                       subcategory[8]="Pipe racks";
                       SetIndex("9");
                       break;




///////////////////////////RIG UP////////////////////////
                 

               }


           }
           else if(i==1)
           {
switch(Index)
{
    case "1":
        subcategory[0] = "position shaker Tank";
        subcategory[1] = "position degasser , install vaccum degasser ";
        subcategory[2] = "Position intermediate tank";
        subcategory[3] = "Position the cable corridor( interconnectingskid )";
        subcategory[4] = "Position reserve tanks";
        subcategory[5] = "Position suction tank";
        subcategory[6] = "Position super charge skid";
        subcategory[7] = "Position pre - mix / slug tank";
        subcategory[8] = "position mixing dock";
        subcategory[9] = "Position storage dock skid";
        subcategory[10] = "Position LP ground suitcases";
        subcategory[11] = "Position mud silo";
        subcategory[12] = "Position water tanks";
        subcategory[13] = "Position mud diesel tank #1 and #2";
        subcategory[14] = "Position rig diesel tank #1 and #2";
        subcategory[15] = "Install LP suitcase to cement tanks";
        subcategory[16] = "Position cement water tank #1 and #2";
        subcategory[17] = "Unload and position centrifuges";
        subcategory[18] = "Unload and position trip tank ";
        subcategory[19] = "Unload and position poor boy";
        subcategory[20] = "install all the power cables to the MCC - house";
        subcategory[21] = "Install cascade system";
        SetIndex("22");
        break;

    case "2":

        subcategory[0] = "Unload and position mud pumps";
        subcategory[1] = "Connect the low pressure suction lines";
        subcategory[2] = "Connect the HP (high pessure ) discharge lines";
        subcategory[3] = "Install the overpressure lines from MP 1 , 2 and 3";
        subcategory[4] = "Connect Mud pump (MP) 1 , 2 and 3 electrical";
        subcategory[5] = "Position and install HP lines";
        SetIndex("6");

        break;

    case "3":

        subcategory[0] = "Position substructure box";
        subcategory[1] = "install base box spreaders / beams";
        subcategory[2] = "Unload and position subsrtucture box DS";
        subcategory[3] = "Position knee braces";
        subcategory[4] = "Position trolley beams";
        subcategory[5] = "Position I.R.D frame";
        subcategory[6] = "Position drip pan (temporary )";
        subcategory[7] = "Install setback";
        subcategory[8] = "Install trolley beams";
        subcategory[9] = "Install rotary support beam ";
        subcategory[10] = "Install Drawworks spreader";
        subcategory[11] = "Install support beams ";
        subcategory[12] = "Install stabilizer spreader";
        subcategory[13] = "Install rotary table";
        subcategory[14] = "Install Drawworks spreader";
        subcategory[15] = "Install doghouse";
        subcategory[16] = "Install rightfloor panels DS and ODS";
        subcategory[17] = "Install drawworks platform with air receiver";
        subcategory[18] = "Install platform HPU";
        subcategory[19] = "Install drillers cabin ";
        subcategory[20] = "Install iron roughneck";
        subcategory[21] = "Connect Power cables and air hoses";
        SetIndex("22");
        break;
    case "4":
        subcategory[0] = "Assemble and install A- frame";
        subcategory[1] = "Position platform travelling block with platform";
        subcategory[2] = "Posiyion mast stands";
        subcategory[3] = "Assemble bottom mast section in combination with lower intermediate mast section DS";
        subcategory[4] = "Assemble bottom mast section in combination with lower intermediate mast section ODS";
        subcategory[5] = "Install spreader frame bottom mast section ";
        subcategory[6] = "Install spreaders , braces and TDS guide tracks";
        subcategory[7] = "Install upper intermediate mast section";
        subcategory[8] = "Install mast raising line DS and ODS";
        subcategory[9] = "Instal TDS service loop";
        subcategory[10] = "Install kelly hose ( rotary hose ) (2x)";
        subcategory[11] = "Install drill lines ";
        subcategory[12] = "Hang -off all winch , hoses and tong cables";
        subcategory[13] = "Install platforms lower mast section";
        subcategory[14] = "Install seveal supports";
        subcategory[15] = "Pull tension on drill line";
        subcategory[16] = "Mast inspection";
        subcategory[17] = "Raise mast on high mast stand";
        subcategory[18] = "Install rackin board";
        subcategory[19] = "Install derrick man s escape device , fall arrestor , stop chutes for derrick man and mast pulling line";
        SetIndex("20");
        break;


    case "5":

        subcategory[0] = "Raise mast";
        subcategory[1] = "Remove raising lines and equalizer";
        subcategory[2] = "Remove travelling bock and support frame";
        subcategory[3] = "install TDS";
        subcategory[4] = "Rig - up fall arrestor line";
        subcategory[5] = "Install casing stabbing board";
        subcategory[6] = "Raise substructure gin poles";
        subcategory[7] = "remove connection between bottom and top substructure box";
        subcategory[8] = "Raise substructure (rigfloor)";
        subcategory[9] = "position staircase DS";
        subcategory[10] = "Install corner floor panels (DS & ODS )";
        subcategory[11] = "Install rigfloor winches";
        subcategory[12] = "Position trip tank";
        subcategory[13] = "Install BOP winches";
        subcategory[14] = "install bell nipple ";
        subcategory[15] = "Install mud return line";
        subcategory[16] = "Install stars rigfloor - trip tank";
        subcategory[17] = "Install stand - pipe manifold";
        subcategory[18] = "Position staircase ODS";
        subcategory[19] = "Insatll rigfloor elevator";
        subcategory[20] = "Install emergency slide";
        subcategory[21] = "Position slick line unit";
        SetIndex("22");
        break;


    case "6":

        subcategory[0] = "Lift the top drive(TDS) including skid on rig floor";
        subcategory[1] = "Attach lifiting gear to travelling block";
        subcategory[2] = "Tilt TDS";
        subcategory[3] = "Make connections between guide track and transport frame";
        subcategory[4] = "Make connection transport frame and mast";
        subcategory[5] = "Remove lifting gear";
        subcategory[6] = "Connect travelling block to TDS bail";
        subcategory[7] = "Rig-up counter balance cylinders";
        subcategory[8] = "Remove bail lock device";
        subcategory[9] = "Remove stoppers";
        subcategory[10] = "Insatll and connect service loops";
        subcategory[11] = "Connect rotary mud hose ( kelly hose )";
        subcategory[12] = "Insatll elevator lins and elevator to TDS";
        SetIndex("13");
        break;


    case "7":

        subcategory[0] = "Lift and load grasshopper skid";
        subcategory[1] = "Unload and position VFD power house";
        subcategory[2] = "Lift and load MCC house";
        subcategory[3] = "unload and position generator power house";
        subcategory[4] = "connect air - lines";
        subcategory[5] = "Disconnect the diesel fuel line to and between the generators";
        subcategory[6] = "Close / tilt air exhaust of the generator";
        subcategory[7] = "Connect all power cables to and from VFD container";
        SetIndex("8");
        break;


    case "8":

        subcategory[0] = "Unload and position master skids";
        subcategory[1] = "Unload and position mosque";
        subcategory[2] = "Unload and position warehouse (2x)";
        subcategory[3] = "Unload and position master skids ";
        subcategory[4] = "Connect all power cables";
        subcategory[5] = "Connect all the water lines";
        subcategory[6] = "Connect all th sewage lines";
        subcategory[7] = "Connect all the data cables";
        subcategory[8] = "Position walkways";
        SetIndex("9");
        break;

    case "9":

        subcategory[0] = "Unload and posiiton master skids";
        subcategory[1] = "unload and position mosque";
        subcategory[2] = "Unload and position STP-150";
        subcategory[3] = "Connect all power cables";
        subcategory[4] = "Connect all the  water lines";
        subcategory[5] = "Connect all the sewage lines";
        subcategory[6] = "Conect all the data cables";
        subcategory[7] = "Position walkways";
        SetIndex("8");

        break;

    case "10":
        subcategory[0] = "Unload and position load mud diesel tanks";
        subcategory[1] = "Unload and position diesel fuel tank";
        subcategory[2] = "Connect all power cables to and from the diesel tanks";
        subcategory[3] = "Connect all the diesel supply lines to the mud system";
        subcategory[4] = "Connect all diesel fuel supply lines to the generators";
        SetIndex("5");

        break;

    case "11":

        subcategory[0] = "Load DP Basket";
        subcategory[1] = "Load Toolbox";
        SetIndex("2");
        break;


    case "12":

        subcategory[0] = "Unload containers";
        subcategory[1] = "Unload miscellaneous equipment";
        subcategory[2] = "Unloading of mud chemicals";
        subcategory[3] = "Unload and position miscellaneous";
        SetIndex("4");
        break;
   }


           }

           else
           {
               //MOVE SUbCATEGORY
           }

           return subcategory;
       }
        public string[] GetActionRigDown()
        {
            string[] action = new string[10];
            switch (Index)                                                                 
            {
                case "01.01":
                    action[0] = "Dump all access drilling mud and/or completion fluid";
                    action[1] = "Flush mud system with water";
                    action[2]= "Flush HP-piping with water";
                    action[3] = "Clean all the tanks compartments from the mud tanks";
                    SetIndex("4");
                break;

                case "01.02":

                    action[0] = "Disconnect all the power cables from mud tanks to MCC-house";
                    action[1] = "Disconnect and prepare for transport PAGA and gas alarm signals ";
                    
                    SetIndex("2");

                break;


                case "01.03":

                action[0] = "Disconnect and prepare for transport cascade system";
                
                SetIndex("1");

                break;


                case "01.04":

                action[0] = "Disconnect and remove the mud return line";
               
                SetIndex("1");

                break;

                case "01.05":

                action[0] = "Disconnect and load poor-boy degasser";
                SetIndex("1");

                break;

                case "01.06":

                action[0] = "Disconnect and load choke manifold";
                SetIndex("1");

                break;

                case "01.07":

                action[0] = "Rig down and remove trip tank";
                SetIndex("1");

                break;

                case "01.08":

                action[0] = "Rig down shaker skid";
                action[1] = "Rig down screw conveyor";
                action[2] = "Rig down and load Shaker tank (sand trap)";
                action[3] = "Rig down desilter/desander";
                SetIndex("4");

                break;

                case "01.09":

                action[0] = "Rig down and load centrifuges";
               
                SetIndex("1");

                break;

                case "01.10":

                action[0] = "Rig down and load intermediate tank";
                
                SetIndex("1");

                break;

                case "01.11":

                action[0] = "Rig down and load  reserve tank # X";
                
                SetIndex("1");

                break;

                case "01.12":

                action[0] = "Rig down and load Interconnecting cabling skid";
               
                SetIndex("1");

                break;

                case "01.13":

                action[0] = "Rig down and load suction tank";
                
                SetIndex("1");

                break;

                case "01.14":

                action[0] = "Rig down and load Charge pump skid";
              
                SetIndex("1");

                break;

                case "01.15":

                action[0] = "Rig down and load Premix - slug tank";
                
                SetIndex("1");

                break;

                case "01.16":

                action[0] = "Rig down and load Silo dock skid";
                
                SetIndex("1");

                break;

                case "01.17":

                action[0] = "Rig down and load Mixing pump skid";
                
                SetIndex("1");

                break;

                case "01.18":

                action[0] = "Rig down and load Silo’s 1 till 3";
                
                SetIndex("1");

                break;

                case "01.19":

                action[0] = "Rig down and load water/diesel supply line";
                action[1] = "Rig down and load mud diesel tanks";
               
                SetIndex("2");

                break;

                case "01.20":

                action[0] = "Rig down  and load Water tank #X";
                
                SetIndex("1");

                break;
                case "01.21":

                action[0] = "Rig down  and load Cement tanks #X";
                
                SetIndex("1");

                break;



              ////////////////////////2///////////////////////////

                case "02.01":

                action[0] = "Disconnect Mud pump (MP) 1, 2 & 3 electrical";

                SetIndex("1");

                break;


                case "02.02":

                action[0] = "Disconnect the overpressure lines from MP 1, 2 & 3.";
                action[1] = "Disconnect the High Pressure (HP) discharge lines";

                SetIndex("2");

                break;

                case "02.03":

                action[0] = "Disconnect the Low Pressure (LP) suction lines";

                SetIndex("1");

                break;

                case "02.04":

                action[0] = "Rig down and load MP #X";

                SetIndex("1");

                break;

                case "02.05":
                action[0] = "Rig down and remove HP pump manifold";
                action[1] = "Rig down and remove HP lines (skid)";
                action[2] = "Rig down HP hoses";

                SetIndex("3");

                break;


                   //////////////////////////3///////////////////////


                case "03.01.01":
                action[0] = "Prepare Top drive (TDS) for rig down";
                SetIndex("1");

                break;

                case "03.01.02":
                action[0] = "Remove elevator and elevator links from TDS";

                SetIndex("1");

                break;


                case "03.01.03":
                action[0] = "Remove the rotary mud hose from TDS";

                SetIndex("1");

                break;

                case "03.01.04":
                action[0] = "Disconnect and remove TDS service-loops";

                SetIndex("1");

                break;

                case "03.01.05":
                action[0] = "Install end stops/stoppers TDS";
                SetIndex("1");

                break;

                case "03.01.06":
                action[0] = "Disconnect counter balance cylinders";

                SetIndex("1");

                break;

                case "03.01.07":
                action[0] = "Install TDS bail lock device";

                SetIndex("1");

                break;

                case "03.01.08":
                action[0] = "Remove travelling block from the TDS bail.";

                SetIndex("1");

                break;

                case "03.01.09":
                action[0] = "Attach travelling block to the TDS bail with slings";

                SetIndex("1");

                break;

                case "03.01.10":
                action[0] = "Remove TDS including skid";
               
                SetIndex("1");

                break;



                case "03.02":
                action[0] = "No any actions!";

                SetIndex("1");

                break;

                case "03.03":
                action[0] = "No any actions!";

                SetIndex("1");

                break;



                    ////////////4/////////////////////


                case "04.01":
                action[0] = "Disconnect all air hoses in substructure";
                action[1] = "Disconnect all hydraulic hoses/piping in substructure";

                SetIndex("2");

                break;

                case "04.02":
                action[0] = "Disconnect power cables in substructure";
               
                SetIndex("1");

                break;

                case "04.03":
                action[0] = "Remove mud return line. See chapter ";
                action[0] = "Remove bell-nipple";
               
                SetIndex("2");

                break;

                case "04.04":
                action[0] = "Remove BOP hoists";
                action[0] = "";
                action[0] = "";
                action[0] = "";

                SetIndex("1");

                break;

                case "04.05":
                action[0] = "Rig down winches (DS and ODS) in substructure";
                action[0] = "";
                action[0] = "";
                action[0] = "";

                SetIndex("1");

                break;

                case "04.06":
                action[0] = "Remove High pressure  (HP) standpipe skid";

                SetIndex("1");

                break;

                case "04.07":
                action[0] = "Remove stairs rig floor - mud system";

                SetIndex("1");

                break;

                case "04.08":
                action[0] = "Remove emergency slide";

                SetIndex("1");

                break;

                case "04.09":
                action[0] = "Remove rig floor elevator";

                SetIndex("1");

                break;

                case "04.10":
                action[0] = "Remove staircase ODS side";
                action[1] = "Remove staircase DS side";


                SetIndex("1");

                break;

                case "04.11":
                action[0] = "Disconnect winches on rig floor";

                SetIndex("1");

                break;

                case "04.12":
                action[0] = "No any actions!Remove slick line unit";

                SetIndex("1");

                break;

                case "04.13":
                action[0] = "remove rig floor panels DS and ODS";

                SetIndex("1");

                break;

                case "04.14":
                action[0] = "Rig-up hydraulic slingshot system";

                SetIndex("1");

                break;

                case "04.15":
                action[0] = "Visual inspection substructure";

                SetIndex("1");

                break;

                case "04.16":
                action[0] = "Remove pins setback - slingshot gin pole";

                SetIndex("1");

                break;

                case "04.17":
                action[0] = "Remove pins knee braces back side";
                action[1] = "Remove pins knee braces front side";

                SetIndex("2");

                break;

                case "04.18":
                action[0] = "Remove pins gin pole leg - top box substructure";
        
                    SetIndex("1");

                break;

                case "04.19":
                action[0] = "Lower substructure";

                SetIndex("1");

                break;

                case "04.20":
                action[0] = "Lower the slingshot gin poles DS & ODS";

                SetIndex("1");

                break;

                case "04.21":

                SetIndex("1");
                action[0] = "Rig down hydraulic slingshot system";

                break;

                case "04.22":
                action[0] = "Install raising sling equalizer";
                action[1] = "Install raising slings";
                SetIndex("2");

                break;

                case "04.23":
                action[0] = "Remove outside service loop from A-frame";

                SetIndex("1");

                break;

                case "04.24":
                action[0] = "Remove outside service loop from A-frame";

                SetIndex("1");

                break;

                case "04.25":
                action[0] = "Position travelling block support frame";

                SetIndex("1");

                break;

                case "04.26":
                action[0] = "Remove climb assist ";
                action[1] = "Remove derrick-man escape";
                action[2] = "Release fall arrestor mast ladder";

                SetIndex("3");

                break;

                case "04.27":
                action[0] = "Visual inspection mast";
                action[1] = "Lower mast on High support";             

                SetIndex("2");

                break;




////////////////////////////////5///////////////////////////////

                case "05.01":
                action[0] = "Pull back all steel-wire cables";

                SetIndex("1");

                break;

                case "05.02":
                action[0] = "Remove racking board";

                SetIndex("1");

                break;

                case "05.03":
                action[0] = "Lower mast from high on to lower mast support";

                SetIndex("1");

                break;

                case "05.04":
                action[0] = "Reeve-out drill-line";

                SetIndex("1");

                break;

                case "05.05":
                action[0] = "Remove rotary mud hose";

                SetIndex("1");

                break;

                case "05.06":
                action[0] = "Remove Top-drive outside service-loop";
                action[1] = "Remove Top-drive inside service loop";

                SetIndex("2");

                break;

                case "05.07":
                action[0] = "Rig down mast raising line DS and ODS";

                SetIndex("1");

                break;

                case "05.08":
                action[0] = "Rig down lower intermediate in combination bottom mast section DS";
                action[1] = "Rig down lower intermediate in combination bottom mast section ODS";

                SetIndex("2");

                break;

                case "05.09":
                action[0] = "Remove A-frame";
                action[1] = "Disassemble A-frame";

                SetIndex("2");

                break;


//////////////////////////6/////////////////////////////////////////////////////



                case "06.01":
                action[0] = "Rig down and remove Iron Roughneck";

                SetIndex("1");

                break;

                case "06.02":
                action[0] = "Rig down Drillers cabin";

                SetIndex("1");

                break;

                case "06.03":
                action[0] = "Rig down and remove doghouse";
                action[1] = "Remove doghouse support frame";

                SetIndex("2");

                break;

                case "06.04":
                action[0] = "Remove backside & rear platform draw works";
                action[1] = "Remove platform rear rotary DS & ODS";
                action[2] = "Remove platform top box ODS";

                SetIndex("3");

                break;

                case "06.05":
                action[0] = "Remove rotary table";

                SetIndex("1");

                break;

                case "06.06":
                action[0] = "Remove Draw works";

                SetIndex("1");

                break;

                case "06.07":
                action[0] = "Disconnect trolley beam plus drip-pan";

                SetIndex("1");

                break;

                case "06.08":
                action[0] = "Remove stabilizer spreader";

                SetIndex("1");

                break;

                case "06.09":
                action[0] = "Remove rotary support beams";

                SetIndex("1");

                break;

                case "06.10":
                action[0] = "Remove draw works spreader";

                SetIndex("1");

                break;

                case "06.11":
                action[0] = "Remove I.R.D skid";

                SetIndex("1");

                break;

                case "06.12":
                action[0] = "Remove Setback";

                SetIndex("1");

                break;

                case "06.13":
                action[0] = "Remove drip-pan";

                SetIndex("1");

                break;

                case "06.14":
                action[0] = "Remove trolley beams";

                SetIndex("1");

                break;

                case "06.15":
                action[0] = "Remove knee braces (front and back)";

                SetIndex("1");

                break;

                case "06.16":
                action[0] = "Remove base box spreaders";

                SetIndex("1");

                break;

                case "06.17":
                action[0] = "Load substructure DS";
                action[1] = "Load substructure ODS";

                SetIndex("2");

                break;

                case "06.18":
                action[0] = "Load rig mating";

                SetIndex("1");

                break;



////////////////////////////////////////7///////////////////////////////////////////



                case "07.01":
                action[0] = "No any action!";

                SetIndex("1");

                break;

                case "07.02":
                action[0] = "Disconnect the diesel fuel lines to and between the ";

                SetIndex("1");

                break;

                case "07.03":
                action[0] = "Disconnect air-lines";

                SetIndex("1");

                break;

                case "07.04":
                action[0] = "Lift and load MCC house";

                SetIndex("1");

                break;

                case "07.05":
                action[0] = "Lift and load VFD power house";

                SetIndex("1");

                break;

                case "07.06":
                action[0] = "Lift and load generator power house #X";

                SetIndex("1");

                break;

                case "07.07":
                action[0] = "Remove cable tray between substructure and grasshopper skid.";
                action[1] = "Lift and load Grasshopper skid";

                SetIndex("2");

                break;

////////////////////////////////8//////////////////////////////////////////////
               
                case "08.01":               
                action[0] = "Load walkways";

                SetIndex("1");

                break;

                case "08.02":
                action[0] = "Disconnect all the data cables";
                action[1] = "Disconnect all the sewage lines";
                action[2] = "Disconnect all the water lines";
                action[3] = "Disconnect all power cables";

                SetIndex("4");

                break;

                case "08.03":
                action[0] = "Load the Generator Unit";

                SetIndex("1");

                break;

                case "08.04":
                action[0] = "Load of Fuel Tank";

                SetIndex("1");

                break;

                case "08.05":
                action[0] = "Load of Filter Unit";

                SetIndex("1");

                break;

                case "08.06":
                action[0] = "Load of Raw Water Tank";

                SetIndex("1");

                break;

                case "08.07.01":
                action[0] = "Load Mech. Work Shop";
                action[1] = "Load Elec. Work Shop";

                SetIndex("2");

                break;

                case "08.07.02":
                action[0] = "Load Crew Tea Room";

                SetIndex("1");

                break;

                case "08.07.03":
                action[0] = "Load Clinic Medic";

                SetIndex("1");

                break;

                case "08.07.04":
                action[0] = "Load Training Meeting Room";

                SetIndex("1");

                break;

                case "08.07.05":
                action[0] = "Load Ablution unit";

                SetIndex("1");

                break;

                case "08.07.06":
                action[0] = "Load 2 Man Office  Mech. & Elec.";

                SetIndex("1");

                break;

                case "08.07.07":
                action[0] = "Load 2 Man Office Geo";

                SetIndex("1");

                break;

                case "08.07.08":
                action[0] = "Load 2 Man Office Com. Man";

                SetIndex("1");

                break;

                case "08.07.09":
                action[0] = "Load 2 Man Office STP";

                SetIndex("1");

                break;

                case "08.07.10":
                action[0] = "Load 4 Man Sleeper with Bath";

                SetIndex("1");

                break;

                case "08.07.11":
                action[0] = "Load 2 Man Sleeper (VIP) Mech. & Elec.";

                SetIndex("1");

                break;

                case "08.07.12":
                action[0] = "Load 1 Man Sleeper (VIP) Company Man";

                SetIndex("1");

                break;

                case "08.07.13":
                action[0] = "Load 1 Man Sleeper VIP STP";

                SetIndex("1");

                break;



                case "08.08":
                action[0] = "<<to be included>>";

                SetIndex("1");

                break;

                case "08.09":
                action[0] = "Load Cement Blocks";

                SetIndex("1");
                break;


    /////////////////////////////////9////////////////////////            

                case "09.01":
                action[0] = "Load walkways";

                SetIndex("1");

                break;

                case "09.02":
                action[0] = "Disconnect all the water lines";
                action[1] = "Disconnect all power cables";

                SetIndex("2");

                break;

                case "09.03.01":
                action[0] = "Load 8 Man Sleeper without Bath";

                SetIndex("1");

                break;

                case "09.03.02":
                action[0] = "Load 4 Man Sleeper without Bath";

                SetIndex("1");

                break;

                case "09.03.03":
                action[0] = "Load 4 Man Sleeper with Bath";

                SetIndex("1");

                break;

                case "09.03.04":
                action[0] = "Load 2 Sleeper with Bath";

                SetIndex("1");

                break;

                case "09.03.05":
                action[0] = "Load Laundry";

                SetIndex("1");

                break;

                case "09.03.06":
                action[0] = "Load 1 Man Office/Sleeper with Bath";

                SetIndex("1");

                break;

                case "09.03.07":
                action[0] = "Load Ablution Unit";

                SetIndex("1");

                break;
                case "09.03.08":
                action[0] = "Load 3 Man Office";
                action[1] = "";

                SetIndex("1");

                break;

                case "09.03.09":
                action[0] = "Load Junior Recreation room";

                SetIndex("1");

                break;

                case "09.03.10":
                action[0] = "Load Cold Store";

                SetIndex("1");

                break;

                case "09.03.11":
                action[0] = "Load Dry Store";

                SetIndex("1");

                break;

                case "09.03.12":
                action[0] = "Load Junior Diner";

                SetIndex("1");

                break;

                case "09.03.13":
                action[0] = "Load Kitchen";

                SetIndex("1");

                break;

                case "09.03.14":
                action[0] = "Load Senior Diner";

                SetIndex("1");

                break;

                case "09.03.15":
                action[0] = "Load Senior Recreation room";

                SetIndex("1");

                break;

                case "09.03.16":
                action[0] = "Load Training/Meeting room";

                SetIndex("1");

                break;




                case "09.04.01":
                action[0] = "to be included";
                action[1] = "to be included";

                SetIndex("2");

                break;

                case "09.04.02":
                action[0] = "to be included";
                action[1] = "to be included";

                SetIndex("2");

                break;

                case "09.04.03":
                action[0] = "to be included";
                action[1] = "to be included";

                SetIndex("2");

                break;

                case "09.04.04":
                action[0] = "to be included";
                action[1] = "to be included";

                SetIndex("2");

                break;

                case "09.04.05":
                action[0] = "to be included";
                action[1] = "to be included";

                SetIndex("2");

                break;

                case "09.04.06":
                action[0] = "to be included";
                action[1] = "to be included";

                SetIndex("2");

                break;

                case "09.04.07":
                action[0] = "to be included";
                action[1] = "to be included";

                SetIndex("2");

                break;

                case "09.04.08":
                action[0] = "to be included";
                action[1] = "to be included";

                SetIndex("2");

                break;

                case "09.04.09":
                action[0] = "to be included";
                action[1] = "to be included";

                SetIndex("2");

                break;

                case "09.04.10":
                action[0] = "to be included";
                action[1] = "to be included";

                SetIndex("2");

                break;

                case "09.04.11":
                action[0] = "to be included";
                action[1] = "to be included";

                SetIndex("2");

                break;

                case "09.04.12":
                action[0] = "to be included";
                action[1] = "to be included";

                SetIndex("2");

                break;

                case "09.04.13":
                action[0] = "to be included";
                action[1] = "to be included";

                SetIndex("2");

                break;

                case "09.05":
                action[0] = "Load Sewage Treatment Plant & Lifting Tank";

                SetIndex("1");

                break;

                case "09.06":
                action[0] = "Load of Work Shop";

                SetIndex("1");

                break;

                case "09.07":
                action[0] = "Load of Filter Unit";
                action[1] = "";

                SetIndex("2");

                break;

                case "09.08":
                action[0] = "Load of Raw Water Tank";

                SetIndex("1");

                break;

                case "09.09":
                action[0] = "Load of Portable Water Tank";

                SetIndex("1");

                break;

                case "09.10":
                action[0] = "Load of Mosque";

                SetIndex("1");

                break;

                case "09.11":
                action[0] = "Load the Generator Unit";

                SetIndex("1");

                break;

                case "09.12":
                action[0] = "Load of Fuel Tank";

                SetIndex("1");

                break;

                case "09.13":
                action[0] = "Load Cement Blocks";

                SetIndex("1");

                break;


////////////////////////////////////////////10////////////////////////////////////////////

                case "10.01":
                action[0] = "Disconnect all power cables to and from the diesel tanks";
                SetIndex("1");

                break;

                case "10.02":
                action[0] = "Disconnect all diesel supply lines to the mud system";
                action[1] = "Disconnect all diesel fuel supply lines to the generators";
                SetIndex("2");

                break;

                case "10.03":
                action[0] = "Lift and load Mud diesel tank #X (450 bbl)";
                SetIndex("1");

                break;

                case "10.04":
                action[0] = "Lift and load diesel fuel tank #X (350 bbl)";
                SetIndex("1");

                break;

///////////////////////////////////11//////////////////////////////////////////


                case "11.01":
                action[0] = "Load DP basket";
                SetIndex("1");

                break;

                case "11.02":
                action[0] = "Load Toolbox";
                SetIndex("1");

                break;

////////////////////////////////////////12//////////////////////////////////////////

                case "12.01":
                action[0] = "Load 40 ft container";
                SetIndex("1");

                break;

                case "12.02":
                action[0] = "Load 20 ft container";
                SetIndex("1");

                break;

                case "12.03":
                action[0] = "Load miscellaneous equipment";
                SetIndex("1");

                break;

                case "12.04":
                action[0] = "Loading of mud chemicals";
                SetIndex("1");

                break;

                case "12.05":
                action[0] = "Load Accumulator unit";
                SetIndex("1");

                break;

                case "12.06":
                action[0] = "Load BOP control line suitcases";
                SetIndex("1");

                break;

                case "12.07":
                action[0] = "Remove & load V-door";
                SetIndex("1");

                break;

                case "12.08":
                action[0] = "Remove and load automatic catwalk system";
                SetIndex("1");

                break;

                case "12.09":
                action[0] = "Load pipe racks";
                SetIndex("1");

                break;


   
            }
            return action;
        }

        /////////////////////////////////RIG UP////////////////////

        public string[] GetActionRigUp()
        {
            string[] action = new string[10];
            switch (Index)
            {

                case "01.01":
                    action[0] = "Mark centre well";
                    action[1] = "Position shaker tank";
                    action[2] = "Install stairs shaker tank";
                    action[3] = "Install shaker skid";
                    action[4] = "Install suction & discharge lines mud cleaner";
                    action[5] = "Install roof shaker tank";
                    
                    SetIndex("6");
                    break;

                case "01.02":
                    action[0] = "Position degasser , Sand and Silt tank ";
                    action[1] = "Install vacuum degasser";
                    
                    SetIndex("2");
                    break;

               
                case "01.03":
                    action[0] = "Position Intermediate tank";
                    action[1] = "Install stall stairs to intermediate tank";
                    
                    SetIndex("2");
                    break;

                case "01.04":
                    action[0] = "Position the cable corridor (interconnecting skid)";
                    
                    SetIndex("1");
                    break;

                case "01.05":
                    action[0] = "Position reserve tank #x";
                    action[1] = "Position reserve tank #x";
                    action[2] = "Position reserve tank #x";
                    action[3] = "Position reserve tank #x";
                    action[4] = "Position reserve tank #x";
                    action[5] = "Position reserve tank #x";

                    SetIndex("6");
                    break;

                case "01.06":
                    action[0] = "Position suction tank";
                    
                    SetIndex("1");
                    break;

                case "01.07":
                    action[0] = "Position Super charge skid";
                    
                    SetIndex("1");
                    break;

                case "01.08":
                    action[0] = "Position pre-mix /slug tank";
                    action[1] = "Install stairs pre-mix / slug tank";
                    
                    SetIndex("2");
                    break;

                case "01.09":
                    action[0] = "Position mixing dock";
                   
                    SetIndex("1");
                    break;

                case "01.10":
                    action[0] = "Position storage dock skid";
                    action[1] = "Install silo’s on storage dock skid";
                    action[2] = "Install stairs storage dock skid";
                   
                    SetIndex("3");
                    break;

                case "01.11":
                    action[0] = "Position LP ground suitcases";
                    
                    SetIndex("1");
                    break;

                case "01.12":
                    action[0] = "Position Mud silo";
                    
                    SetIndex("1");
                    break;

                case "01.13":
                    action[0] = "Position water tank x";
                    action[1] = "Position water tank x till x";
                   
                    SetIndex("2");
                    break;

                case "01.14":
                    action[0] = "Position mud diesel tank #x & #x";
                    
                    SetIndex("1");
                    break;

                case "01.15":
                    action[0] = "Position rig diesel tank #x & #x";
                   
                    SetIndex("1");
                    break;

                case "01.16":
                    action[0] = "Install LP suitcase to cement tanks";
                    
                    SetIndex("1");
                    break;

                case "01.17":
                    action[0] = "Position cement water tank #x & #x";
                  
                    SetIndex("1");
                    break;

                case "01.18":
                    action[0] = "Unload and position centrifuges";
                    
                    SetIndex("1");
                    break;

                case "01.19":
                    action[0] = "Unload and position Trip tank";
                    
                    SetIndex("1");
                    break;

                case "01.20":
                    action[0] = "Unload and position Poor-boy";
                   
                    SetIndex("1");
                    break;

                case "01.21":
                    action[0] = "Install all the power cables to the MCC house";
                    action[1] = "Install all the air lines";
                    action[2] = "Install cutting ditches";
                    action[3] = "Install PAGA system";
                   
                    SetIndex("4");
                    break;

                case "01.22":
                    action[0] = "Install cascade system";
                    
                    SetIndex("1");
                    break;



//////////////////////////////////////2//////////////////////////////////////////////////


                case "02.01":
                    action[0] = "Mark position matting for mud pumps";
                    action[1] = "Unload and position Mud pump #x";
                    action[2] = "Unload and position Mud pump #x";
                    action[3] = "Unload and position Mud pump #x";
                   
                    SetIndex("4");
                    break;

                case "02.02":
                    action[0] = "Connect the low pressure (LP) suction lines";

                    SetIndex("1");
                    break;

                case "02.03":
                    action[0] = "Connect the High Pressure (HP) discharge lines";

                    SetIndex("1");
                    break;

                case "02.04":
                    action[0] = "Install the overpressure lines from MP 1, 2 & 3.";

                    SetIndex("1");
                    break;

                case "02.05":
                    action[0] = "Connect Mud pump (MP) 1, 2 & 3 electrical";

                    SetIndex("1");
                    break;

                case "02.06":
                    action[0] = "Position and install HP pump manifold";
                    action[1]=  "Position and install HP ground suitcases";
                    action[2] = "Rig-up HP hoses";

                    SetIndex("3");
                    break;

////////////////////////////////////////////// 3 //////////////////// RIG UP ////////


                case "03.01":
                    action[0] = "Mark centre well";
                    action[1] = "Mark position and position mating plan";
                    action[2] = "Mark position substructure box";
                    action[3] =  "Unload and position substructure box ODS";
                    SetIndex("4");
                    break;

                case "03.02":
                    action[0] = "Install base box spreaders (frames)";
                    action[1] = "Install base box spreader beams (K-frame)";

                    SetIndex("2");
                    break;

                case "03.03":
                    action[0] = "Unload and position substructure box DS";

                    SetIndex("1");
                    break;

                case "03.04":
                    action[0] = "Position Knee braces";

                    SetIndex("1");
                    break;

                case "03.05":
                    action[0] = "Position trolley beams";

                    SetIndex("1");
                    break;

                case "03.06":
                    action[0] = "Position I.R.D. frame";

                    SetIndex("1");
                    break;

                case "03.07":
                    action[0] = "Position drip-pan (temporary)";

                    SetIndex("1");
                    break;

                case "03.08":
                    action[0] = "Install Setback";

                    SetIndex("1");
                    break;

                case "03.09":
                    action[0] = "Install Trolley beams";

                    SetIndex("1");
                    break;

                case "03.10":
                    action[0] = "Install rotary support beam";

                    SetIndex("1");
                    break;

                case "03.11":
                    action[0] = "Install Drawworks spreader";

                    SetIndex("1");
                    break;

                case "03.12":
                    action[0] = "Install support beams";

                    SetIndex("1");
                    break;

                case "03.13":
                    action[0] = "Install stabilizer spreaders";

                    SetIndex("1");
                    break;

                case "03.14":
                    action[0] = "Install rotary table";

                    SetIndex("1");
                    break;

                case "03.15":
                    action[0] = "Install Drawworks";

                    SetIndex("1");
                    break;

                case "03.16":
                    action[0] = "Install doghouse supports";
                    action[1] = "Install doghouse";

                    SetIndex("2");
                    break;

                case "03.17":
                    action[0] ="Install rigfloor panels DS & ODS";

                    SetIndex("1");
                    break;

                case "03.18":
                    action[0] = "Install backside & rear platform drawworks";
                    action[1] = "Install platform with drawworks air receiver";

                    SetIndex("2");
                    break;

                case "03.19":
                    action[0] = "Install platform HPU ODS & HPU HPU &";

                    SetIndex("1");
                    break;

                case "03.20":
                    action[0] = "Install drillers cabin";

                    SetIndex("1");
                    break;

                case "03.21":
                    action[0] = "Install Iron Roughneck";

                    SetIndex("1");
                    break;

                case "03.22":
                    action[0] = "Connect power cables";
                    action[1] = "Connect air- & hydraulic hoses";
                    

                    SetIndex("2");
                    break;

                /////////// 4 //////////////// RIG UP //////// ACTIONS ///////Unload And Rig-Up Mast

                case "04.01":
                    action[0] = "Assemble and install Aframe";

                    SetIndex("1");
                    break;

                case "04.02":
                    action[0] = "Position platform travelling block with travelling block";

                    SetIndex("1");
                    break;

                case "04.03":
                    action[0] = "Position mast stands";
                    SetIndex("1");
                    break;

                case "04.04":
                    action[0] = "Assemble Bottom mast section in combination with lower intermediate mast section DS";

                    SetIndex("1");
                    break;

                case "04.05":
                    action[0] = "Assemble Bottom mast section in combination with lower intermediate mast section ODS";

                    SetIndex("1");
                    break;

                case "04.06":
                    action[0] = "Install spreader frame bottom mast section";

                    SetIndex("1");
                    break;

                case "04.07":
                    action[0] = "Install spreaders, braces and TDS guide tracks";

                    SetIndex("1");
                    break;

                case "04.08":
                    action[0] = "Install upper intermediate mast section";


                    SetIndex("1");
                    break;

                case "04.09":
                    action[0] = "Rig-up top section including crown skid";


                    SetIndex("1");
                    break;

                case "04.10":
                    action[0] = "Install mast raising line DS and ODS";


                    SetIndex("1");
                    break;

                case "04.11":
                    action[0] = "Install TDS service loop";


                    SetIndex("1");
                    break;

                case "04.12":
                    action[0] = "Install Kelly hose (rotary hose) (2x)";


                    SetIndex("1");
                    break;

                case "04.13":
                    action[0] = "Install drill line";
                    action[1] = "Install drill-line on drum";


                    SetIndex("2");
                    break;

                case "04.14":
                    action[0] = "Hang-off all winch, hoses and tong cables";


                    SetIndex("1");
                    break;

                case "04.15":
                    action[0] = "Install platforms lower mast section";


                    SetIndex("1");
                    break;

                case "04.16":
                    action[0] = "Install several supports";


                    SetIndex("1");
                    break;

                case "04.17":
                    action[0] = "Pull tension on drill-line";
                    


                    SetIndex("1");
                    break;

                case "04.18":
                    action[0] = "Mast inspection";
                 
                    SetIndex("1");
                    break;

                case "04.19":
                    action[0] = "Raise mast on high mast stand";
                   
                    SetIndex("1");
                    break;

                case "04.20":
                    action[0] = "Install racking board";

                    SetIndex("1");
                    break;

                case "04.21":
                    action[0] = "Install derrick man escape device";
                    action[1] = "Install fall arrestor and climb assistant ladder";
                    action[2]="Install stop chutes for derrick man";
                    action[3] = "Install mast pulling line";
                    


                    SetIndex("4");
                    break;


                /////////// 5 //////////////// RIG UP //////// ACTIONS ///////



                case "05.01":
                    action[0] = "Visual inspection Mast";
                    action[1] = "Raise mast";
                    
                    SetIndex("2");
                    break;

                case "05.02":
                    action[0] = "Remove raising lines & equalizer";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.03":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.04":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.05":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.06":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.07":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.08":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.09":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.10":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.11":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.12":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.13":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.14":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.15":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.16":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.17":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.18":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.19":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.20":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.21":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;

                case "05.22":
                    action[0] = "";
                    action[1] = "";

                    SetIndex("2");
                    break;


            
            
            
            }
            return action;
        }



        public string[] GetResources(string i)
        {
            string[] resources = new string[10];
            if (i == "")
            {
                switch (Index)
                {


                }
            }
            else if(i=="")
            {
                switch (Index)
                {

                }
            }
            return resources ;
        }

        //public string GetDetails(int index)
        //{
        //    string chapter;
        //    switch (index)
        //    {
        //        case 1:
        //            chapter = "";


        //    }
        //    return chapter;
        //}
    }
}
