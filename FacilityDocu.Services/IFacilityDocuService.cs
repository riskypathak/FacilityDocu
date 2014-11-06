﻿using FacilityDocu.DTO;
using FacilityDocu.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace FacilityDocu.Services
{
    [ServiceContract]
    public interface IFacilityDocuService
    {
        [OperationContract]
        bool Login(string userName, string password);

        [OperationContract]
        Dictionary<int, bool> IsSync(Dictionary<int, DateTime> ProjectsData);

        [OperationContract]
        List<ProjectDTO> GetProjectDetails(IList<int> ProjectIDs);

        [OperationContract]
        void UpdateProject(ProjectDTO projectDTO);

        [OperationContract]
        List<ModuleDTO> GetModules(int RigTypeID);

        [OperationContract]
        IList<StepDTO> GetSteps(int ModuleID);

        [OperationContract]
        IList<ActionDTO> GetActions(int ActionID);

        [OperationContract]
        void UpdateActionImages(ActionDTO action);
    }
}




    
      
    
    
      
    
      
    
 
