//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FacilityDocu.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProjectDetail
    {
        public ProjectDetail()
        {
            this.ProjectActionAttachments = new HashSet<ProjectActionAttachment>();
            this.ProjectActionImages = new HashSet<ProjectActionImage>();
            this.RiskAnalysis = new HashSet<RiskAnalysi>();
        }
    
        public int ProjectDetailID { get; set; }
        public Nullable<int> ProjectID { get; set; }
        public Nullable<int> StepID { get; set; }
        public string Risks { get; set; }
        public string Dimensions { get; set; }
        public string LiftingGears { get; set; }
        public string ActionName { get; set; }
        public string Description { get; set; }
        public Nullable<bool> ActionNameWarning { get; set; }
        public Nullable<bool> ActionDescriptionWarning { get; set; }
        public string ImportantActionname { get; set; }
        public string ImportantActionDescription { get; set; }
        public bool IsAnalysis { get; set; }
        public string PublishedBy { get; set; }
        public Nullable<System.DateTime> PublishedDate { get; set; }
        public string Tools { get; set; }
        public string People { get; set; }
        public string Machines { get; set; }
    
        public virtual Project Project { get; set; }
        public virtual ICollection<ProjectActionAttachment> ProjectActionAttachments { get; set; }
        public virtual ICollection<ProjectActionImage> ProjectActionImages { get; set; }
        public virtual User User { get; set; }
        public virtual Step Step { get; set; }
        public virtual ICollection<RiskAnalysi> RiskAnalysis { get; set; }
    }
}
