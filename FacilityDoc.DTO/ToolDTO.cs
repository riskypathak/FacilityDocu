using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.DTO
{

    [DataContract]
    public class ToolDTO
    {
        [DataMember]
        public string ToolID { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
