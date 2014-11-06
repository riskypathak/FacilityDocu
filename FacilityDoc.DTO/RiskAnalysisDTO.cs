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
        public RiskAnalysisTypeDTO RiskAnalysisType { get; set; }

        [DataMember]
        public string Activity { get; set; }

        [DataMember]
        public string Danger { get; set; }

        [DataMember]
        public double K { get; set; }

        [DataMember]
        public double B { get; set; }

        [DataMember]
        public double E { get; set; }

        [DataMember]
        public double Risk { get; set; }

        [DataMember]
        public string Controls { get; set; }

        [DataMember]
        public double K_ { get; set; }

        [DataMember]
        public double B_ { get; set; }

        [DataMember]
        public double E_ { get; set; }

        [DataMember]
        public double Risk_ { get; set; }
    }
}
