using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.DTO
{

    [DataContract]
    public class ProjectDTO
    {
        [DataMember]
        public string ProjectID { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public UserDTO CreatedBy { get; set; }

        [DataMember]
        public DateTime LastUpdatedAt { get; set; }

        [DataMember]
        public UserDTO LastUpdatedBy { get; set; }

        [DataMember]
        public bool Template { get; set; }

        [DataMember]
        public IList<RigTypeDTO> RigTypes { get; set; }
    }
}
