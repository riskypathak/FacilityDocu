using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacilityDocu.DTO
{
    public class AllMasterDTO
    {
        public List<MasterDTO> MasterData { get; set; }
    }

    public class MasterDTO
    {
        public string Type { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
