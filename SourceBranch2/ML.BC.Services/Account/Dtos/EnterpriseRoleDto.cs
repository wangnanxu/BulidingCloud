using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class EnterpriseRoleDto
    {
        public int RoleID { get; set; }
        public string Name { get; set; }
        public string OwnerID { get; set; }
        public string EnterpriseName { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
        public string FunctionIDs { get; set; }

        static public implicit operator int(EnterpriseRoleDto dto)
        {
            return dto.RoleID;
        }

    }
}
