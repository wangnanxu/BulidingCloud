using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services.Account.Dtos
{
    public class SessionUserDto
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public DateTime LastLoginTime { get; set; }
        public string LastIP { get; set; }
        public string Device { get; set; }
        public string Token { get; set; }
        public int[] RoleIDs { get; set; }
        public string[] FunctionIDs { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public int? DepartmentID { get; set; }
        public string Picture { get; set; }
    }
}
