using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.DTO
{
    [DataContract]
    public class ActionDTO
    {
        [DataMember]
        public string ActionID { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Risks { get; set; }

        [DataMember]
        public string Dimensions { get; set; }

        [DataMember]
        public string LiftingGears { get; set; }

        [DataMember]
        public IList<ImageDTO> Images { get; set; }
    }
}
