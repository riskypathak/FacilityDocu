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
    
    public partial class ImageComment
    {
        public int ImageCommentID { get; set; }
        public Nullable<int> ImageID { get; set; }
        public string Text { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
    
        public virtual Image Image { get; set; }
    }
}
