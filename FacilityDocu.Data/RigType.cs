
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
    
public partial class RigType
{

    public RigType()
    {

        this.Modules = new HashSet<Module>();

        this.ProjectDetails = new HashSet<ProjectDetail>();

    }


    public int RigTypeID { get; set; }

    public string Name { get; set; }



    public virtual ICollection<Module> Modules { get; set; }

    public virtual ICollection<ProjectDetail> ProjectDetails { get; set; }

}

}
