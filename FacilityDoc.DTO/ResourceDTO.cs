using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.DTO
{

    [DataContract]
    public class ResourceDTO
    {
        [DataMember]
        public string ResourceID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ResourceCount { get; set; }
    }
}
