using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.DTO
{

    [DataContract]
    public class RiskAnalysisDTO
    {
        [DataMember]
        public string RiskAnalysisID { get; set; }

        [DataMember]
        public string Activity { get; set; }

        [DataMember]
        public string Danger { get; set; }

        [DataMember]
        public double Risk { get; set; }

        [DataMember]
        public string Controls { get; set; }

        [DataMember]
        public string L { get; set; }

        [DataMember]
        public int S { get; set; }

        [DataMember]
        public string Responsible { get; set; }
    }
}
