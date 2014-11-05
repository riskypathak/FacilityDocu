
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
    
public partial class Module
{

    public Module()
    {

        this.ModuleSteps = new HashSet<ModuleStep>();

        this.ProjectDetails = new HashSet<ProjectDetail>();

        this.RiskAnalysis = new HashSet<RiskAnalysi>();

    }


    public int ModuleID { get; set; }

    public string ModuleName { get; set; }

    public Nullable<int> RigTypeID { get; set; }



    public virtual RigType RigType { get; set; }

    public virtual ICollection<ModuleStep> ModuleSteps { get; set; }

    public virtual ICollection<ProjectDetail> ProjectDetails { get; set; }

    public virtual ICollection<RiskAnalysi> RiskAnalysis { get; set; }

}

}
