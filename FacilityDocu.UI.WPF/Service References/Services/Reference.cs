﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FacilityDocLaptop.Services {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Services.IFacilityDocuService")]
    public interface IFacilityDocuService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/Login", ReplyAction="http://tempuri.org/IFacilityDocuService/LoginResponse")]
        bool Login(string userName, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/Login", ReplyAction="http://tempuri.org/IFacilityDocuService/LoginResponse")]
        System.Threading.Tasks.Task<bool> LoginAsync(string userName, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/IsSync", ReplyAction="http://tempuri.org/IFacilityDocuService/IsSyncResponse")]
        System.Collections.Generic.Dictionary<int, bool> IsSync(System.Collections.Generic.Dictionary<int, System.DateTime> ProjectsData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/IsSync", ReplyAction="http://tempuri.org/IFacilityDocuService/IsSyncResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<int, bool>> IsSyncAsync(System.Collections.Generic.Dictionary<int, System.DateTime> ProjectsData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/GetProjectDetails", ReplyAction="http://tempuri.org/IFacilityDocuService/GetProjectDetailsResponse")]
        FacilityDocu.UI.Utilities.Services.ProjectDTO[] GetProjectDetails(int[] ProjectIDs);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/GetProjectDetails", ReplyAction="http://tempuri.org/IFacilityDocuService/GetProjectDetailsResponse")]
        System.Threading.Tasks.Task<FacilityDocu.UI.Utilities.Services.ProjectDTO[]> GetProjectDetailsAsync(int[] ProjectIDs);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/UpdateProject", ReplyAction="http://tempuri.org/IFacilityDocuService/UpdateProjectResponse")]
        void UpdateProject(FacilityDocu.UI.Utilities.Services.ProjectDTO projectDTO);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/UpdateProject", ReplyAction="http://tempuri.org/IFacilityDocuService/UpdateProjectResponse")]
        System.Threading.Tasks.Task UpdateProjectAsync(FacilityDocu.UI.Utilities.Services.ProjectDTO projectDTO);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/UpdateActionImages", ReplyAction="http://tempuri.org/IFacilityDocuService/UpdateActionImagesResponse")]
        void UpdateActionImages(FacilityDocu.UI.Utilities.Services.ActionDTO action);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/UpdateActionImages", ReplyAction="http://tempuri.org/IFacilityDocuService/UpdateActionImagesResponse")]
        System.Threading.Tasks.Task UpdateActionImagesAsync(FacilityDocu.UI.Utilities.Services.ActionDTO action);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/GetTools", ReplyAction="http://tempuri.org/IFacilityDocuService/GetToolsResponse")]
        FacilityDocu.UI.Utilities.Services.ToolDTO[] GetTools();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/GetTools", ReplyAction="http://tempuri.org/IFacilityDocuService/GetToolsResponse")]
        System.Threading.Tasks.Task<FacilityDocu.UI.Utilities.Services.ToolDTO[]> GetToolsAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IFacilityDocuServiceChannel : FacilityDocLaptop.Services.IFacilityDocuService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class FacilityDocuServiceClient : System.ServiceModel.ClientBase<FacilityDocLaptop.Services.IFacilityDocuService>, FacilityDocLaptop.Services.IFacilityDocuService {
        
        public FacilityDocuServiceClient() {
        }
        
        public FacilityDocuServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public FacilityDocuServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FacilityDocuServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public FacilityDocuServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool Login(string userName, string password) {
            return base.Channel.Login(userName, password);
        }
        
        public System.Threading.Tasks.Task<bool> LoginAsync(string userName, string password) {
            return base.Channel.LoginAsync(userName, password);
        }
        
        public System.Collections.Generic.Dictionary<int, bool> IsSync(System.Collections.Generic.Dictionary<int, System.DateTime> ProjectsData) {
            return base.Channel.IsSync(ProjectsData);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<int, bool>> IsSyncAsync(System.Collections.Generic.Dictionary<int, System.DateTime> ProjectsData) {
            return base.Channel.IsSyncAsync(ProjectsData);
        }
        
        public FacilityDocu.UI.Utilities.Services.ProjectDTO[] GetProjectDetails(int[] ProjectIDs) {
            return base.Channel.GetProjectDetails(ProjectIDs);
        }
        
        public System.Threading.Tasks.Task<FacilityDocu.UI.Utilities.Services.ProjectDTO[]> GetProjectDetailsAsync(int[] ProjectIDs) {
            return base.Channel.GetProjectDetailsAsync(ProjectIDs);
        }
        
        public void UpdateProject(FacilityDocu.UI.Utilities.Services.ProjectDTO projectDTO) {
            base.Channel.UpdateProject(projectDTO);
        }
        
        public System.Threading.Tasks.Task UpdateProjectAsync(FacilityDocu.UI.Utilities.Services.ProjectDTO projectDTO) {
            return base.Channel.UpdateProjectAsync(projectDTO);
        }
        
        public void UpdateActionImages(FacilityDocu.UI.Utilities.Services.ActionDTO action) {
            base.Channel.UpdateActionImages(action);
        }
        
        public System.Threading.Tasks.Task UpdateActionImagesAsync(FacilityDocu.UI.Utilities.Services.ActionDTO action) {
            return base.Channel.UpdateActionImagesAsync(action);
        }
        
        public FacilityDocu.UI.Utilities.Services.ToolDTO[] GetTools() {
            return base.Channel.GetTools();
        }
        
        public System.Threading.Tasks.Task<FacilityDocu.UI.Utilities.Services.ToolDTO[]> GetToolsAsync() {
            return base.Channel.GetToolsAsync();
        }
    }
}
