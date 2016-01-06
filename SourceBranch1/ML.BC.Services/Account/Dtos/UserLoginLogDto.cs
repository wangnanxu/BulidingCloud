using ML.BC.EnterpriseData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class UserLoginLogDto
    {
        public long UserLoginLogID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string EnterpriseName { get; set; }
        public string DepartmentName { get; set; }
        public string IP { get; set; }
        public System.DateTime Time { get; set; }
        public string Device { get; set; }
        public LoginStatus Status { get; set; }
        public string Description { get; set; }
    }
    public class EnterpriseUsers
    {
        public string UserID { get; set; }
        public string EnterpriseName { get; set; }
        public string DepartmentName { get; set; }
        public string UserName { get; set; }
        public int? DepartmentID { get; set; }
        public string EnterpriseID { get; set; }
    }
}
