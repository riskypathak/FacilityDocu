
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
    
public partial class Step
{

    public Step()
    {

        this.ModuleSteps = new HashSet<ModuleStep>();

        this.ProjectDetails = new HashSet<ProjectDetail>();

        this.StepActions = new HashSet<StepAction>();

    }


    public int StepID { get; set; }

    public string StepName { get; set; }



    public virtual ICollection<ModuleStep> ModuleSteps { get; set; }

    public virtual ICollection<ProjectDetail> ProjectDetails { get; set; }

    public virtual ICollection<StepAction> StepActions { get; set; }

}

}
