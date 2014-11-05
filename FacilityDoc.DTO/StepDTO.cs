using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.DTO
{

    [DataContract]
    public class StepDTO
    {
        [DataMember]
        public int StepID { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public IList<ActionDTO> Actions { get; set; }
    }
}
