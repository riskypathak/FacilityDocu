using FacilityDocu.DTO;
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
        Dictionary<int, string> IsSync(List<int> inputProjects, bool fromTablet);
        
        [OperationContract]
        Dictionary<int, Dictionary<int, string>> SyncRequiredForUpdatedProjects(Dictionary<int, List<ActionDTO>> projectActionDTOs);

        [OperationContract]
        ProjectDTO GetProjectDetails(int projectID);

        [OperationContract]
        List<ActionDTO> GetProjectActions(int projectID, List<int> actionIds);

        [OperationContract]
        ProjectDTO UpdateProject(ProjectDTO projectDTO, string userName);

        [OperationContract]
        Dictionary<string, int> UpdateActionImages(ActionDTO action);

        [OperationContract]
        void UpdateActionAttachments(ActionDTO action);

        [OperationContract]
        void CreateTemplate(ProjectDTO projectDTO);
    }
}




    
      
    
    
      
    
      
    
 
