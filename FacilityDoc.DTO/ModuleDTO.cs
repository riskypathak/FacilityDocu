using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace FacilityDocu.DTO
{
    [DataContract]
    public class ModuleDTO
    {
        [DataMember]
        public string ModuleID { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public IList<StepDTO> Steps { get; set; }
    }
}
