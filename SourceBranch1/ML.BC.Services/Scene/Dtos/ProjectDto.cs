using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class ProjectDto
    {
        public string ProjectID { get; set; }
        public string Name { get; set; }
        public string EnterpriseID { get; set; }
        public string Departments { get; set; }
        public List<string> RoleName { get; set; }
        public List<string> ManagerName { get; set; }
        public List<string> DepartmentName { get; set; }
        public string Managers { get; set; }
        public string Roles { get; set; }
        public System.DateTime RegistDate { get; set; }
        public Nullable<System.DateTime> BeginDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public ML.BC.EnterpriseData.Common.Status Status { get; set; }
        public bool Deleted { get; set; }
    }
}
