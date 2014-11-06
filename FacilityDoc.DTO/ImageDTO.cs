using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.DTO
{
    [DataContract]
    public class ImageDTO
    {
        [DataMember]
        public string ImageID { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public IList<string> Tags { get; set; }

        [DataMember]
        public IList<CommentDTO> Comments { get; set; }

        [DataMember]
        public byte[] FileByteStream;

    }
}
