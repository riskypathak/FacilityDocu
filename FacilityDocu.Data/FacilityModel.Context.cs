﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TabletApp_DatabaseEntities : DbContext
    {
        public TabletApp_DatabaseEntities()
            : base("name=TabletApp_DatabaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<ImageComment> ImageComments { get; set; }
        public virtual DbSet<LiftingGear> LiftingGears { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectActionAttachment> ProjectActionAttachments { get; set; }
        public virtual DbSet<ProjectActionImage> ProjectActionImages { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<RigType> RigTypes { get; set; }
        public virtual DbSet<RiskAnalysi> RiskAnalysis { get; set; }
        public virtual DbSet<Risk> Risks { get; set; }
        public virtual DbSet<Step> Steps { get; set; }
        public virtual DbSet<Tool> Tools { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ProjectDetail> ProjectDetails { get; set; }
    }
}
