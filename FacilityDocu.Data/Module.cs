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
            this.Steps = new HashSet<Step>();
        }
    
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public Nullable<int> RigTypeID { get; set; }
    
        public virtual RigType RigType { get; set; }
        public virtual ICollection<Step> Steps { get; set; }
    }
}
