using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.DTO
{
    [DataContract]
    public class RiskAnalysisTypeDTO
    {
        [DataMember]
        public string RiskTypeID { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
