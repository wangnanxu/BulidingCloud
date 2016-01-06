using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services.Enterprise.Dtos
{
    public class EntPropertyDto
    {
        public string EnterprisePropertyID { get; set; }
        public string Name { get; set; }
        public bool Available { get; set; }

        public static implicit operator ML.BC.EnterpriseData.Model.EnterpriseProperty (EntPropertyDto dto)
        {
            return new EnterpriseData.Model.EnterpriseProperty
            {
                EnterprisePropertyID = dto.EnterprisePropertyID,
                Name = dto.Name,
                Available = dto.Available
            };
        }
        public static implicit operator EntPropertyDto(ML.BC.EnterpriseData.Model.EnterpriseProperty orig)
        {
            return new EntPropertyDto{
                EnterprisePropertyID = orig.EnterprisePropertyID,
                Name = orig.Name,
                Available = orig.Available
            };
        }
    }
}
