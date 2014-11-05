using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.DTO
{
    [DataContract]
    public class CommentDTO
    {
        [DataMember]
        public string CommentID { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public DateTime CommentedAt { get; set; }
    }
}
