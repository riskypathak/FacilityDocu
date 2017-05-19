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
        public string StepID { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsNameWarning { get; set; }

        [DataMember]
        public bool IsDescriptionwarning { get; set; }

        [DataMember]
        public bool IsAnalysis { get; set; }

        [DataMember]
        public string ImportantName { get; set; }

        [DataMember]
        public string ImportantDescription { get; set; }

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

        [DataMember]
        public IList<AttachmentDTO> Attachments { get; set; }

        [DataMember]
        public string People { get; set; }

        [DataMember]
        public string Machines { get; set; }

        [DataMember]
        public string Tools { get; set; }

        [DataMember]
        public IList<RiskAnalysisDTO> RiskAnalysis { get; set; }

        [DataMember]
        public DateTime LastUpdatedAt { get; set; }

        [DataMember]
        public UserDTO LastUpdatedBy { get; set; }

        [DataMember]
        public DateTime PublishedAt { get; set; }

        [DataMember]
        public UserDTO PublishedBy { get; set; }
    }
}
