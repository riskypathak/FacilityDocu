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
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/GetModules", ReplyAction="http://tempuri.org/IFacilityDocuService/GetModulesResponse")]
        FacilityDocu.UI.Utilities.Services.ModuleDTO[] GetModules(int RigTypeID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/GetModules", ReplyAction="http://tempuri.org/IFacilityDocuService/GetModulesResponse")]
        System.Threading.Tasks.Task<FacilityDocu.UI.Utilities.Services.ModuleDTO[]> GetModulesAsync(int RigTypeID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/GetSteps", ReplyAction="http://tempuri.org/IFacilityDocuService/GetStepsResponse")]
        FacilityDocu.UI.Utilities.Services.StepDTO[] GetSteps(int ModuleID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/GetSteps", ReplyAction="http://tempuri.org/IFacilityDocuService/GetStepsResponse")]
        System.Threading.Tasks.Task<FacilityDocu.UI.Utilities.Services.StepDTO[]> GetStepsAsync(int ModuleID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/GetActions", ReplyAction="http://tempuri.org/IFacilityDocuService/GetActionsResponse")]
        FacilityDocu.UI.Utilities.Services.ActionDTO[] GetActions(int ActionID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IFacilityDocuService/GetActions", ReplyAction="http://tempuri.org/IFacilityDocuService/GetActionsResponse")]
        System.Threading.Tasks.Task<FacilityDocu.UI.Utilities.Services.ActionDTO[]> GetActionsAsync(int ActionID);
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
        
        public FacilityDocu.UI.Utilities.Services.ModuleDTO[] GetModules(int RigTypeID) {
            return base.Channel.GetModules(RigTypeID);
        }
        
        public System.Threading.Tasks.Task<FacilityDocu.UI.Utilities.Services.ModuleDTO[]> GetModulesAsync(int RigTypeID) {
            return base.Channel.GetModulesAsync(RigTypeID);
        }
        
        public FacilityDocu.UI.Utilities.Services.StepDTO[] GetSteps(int ModuleID) {
            return base.Channel.GetSteps(ModuleID);
        }
        
        public System.Threading.Tasks.Task<FacilityDocu.UI.Utilities.Services.StepDTO[]> GetStepsAsync(int ModuleID) {
            return base.Channel.GetStepsAsync(ModuleID);
        }
        
        public FacilityDocu.UI.Utilities.Services.ActionDTO[] GetActions(int ActionID) {
            return base.Channel.GetActions(ActionID);
        }
        
        public System.Threading.Tasks.Task<FacilityDocu.UI.Utilities.Services.ActionDTO[]> GetActionsAsync(int ActionID) {
            return base.Channel.GetActionsAsync(ActionID);
        }
    }
}
