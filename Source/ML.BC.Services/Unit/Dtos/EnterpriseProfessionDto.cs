using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class EnterpriseProfessionDto
    {
        public string EnterpriseProfessionID { get; set; }
        public string Name { get; set; }
        public bool Available { get; set; }

        static public implicit operator EnterpriseProfessionDto(ML.BC.EnterpriseData.Model.EnterpriseProfession obj)
        {
            return new EnterpriseProfessionDto
            {
                EnterpriseProfessionID = obj.EnterpriseProfessionID,
                Name = obj.Name,
                Available = obj.Available
            };
        }

        static public implicit operator ML.BC.EnterpriseData.Model.EnterpriseProfession(EnterpriseProfessionDto obj)
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
