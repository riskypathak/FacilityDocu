
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
    
public partial class Image
{

    public Image()
    {

        this.ImageDetailComments = new HashSet<ImageDetailComment>();

        this.ProjectActionImages = new HashSet<ProjectActionImage>();

    }


    public int ImageID { get; set; }

    public string Description { get; set; }

    public string ImagePath { get; set; }

    public string Tags { get; set; }

    public Nullable<System.DateTime> CreationDate { get; set; }



    public virtual ICollection<ImageDetailComment> ImageDetailComments { get; set; }

    public virtual ICollection<ProjectActionImage> ProjectActionImages { get; set; }

}

}
