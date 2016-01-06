using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services.Enterprise.Dtos
{
    public class EntProfessionDto
    {
        public string EnterpriseProfessionID { get; set; }
        public string Name { get; set; }
        public bool Available { get; set; }

        static public implicit operator EntProfessionDto(ML.BC.EnterpriseData.Model.EnterpriseProfession obj)
        {
            return new EntProfessionDto
            {
                EnterpriseProfessionID = obj.EnterpriseProfessionID,
                Name = obj.Name,
                Available = obj.Available
            };
        }

        static public implicit operator ML.BC.EnterpriseData.Model.EnterpriseProfession(EntProfessionDto obj)
        {
            return new ML.BC.EnterpriseData.Model.EnterpriseProfession
            {
                EnterpriseProfessionID = obj.EnterpriseProfessionID,
                Name = obj.Name,
                Available = obj.Available
            };
        }
    }
}
