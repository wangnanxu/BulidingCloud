using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class DepartmentBase
    {
        public string Name { get; set; }
        public string EnterpriseID { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
        public int ParentID { get; set; }
        public bool Deleted { get; set; }
        static public implicit operator DepartmentBase(ML.BC.EnterpriseData.Model.Department x)
        {
            return new DepartmentBase
            {
                ParentID = x.ParentID,
                Name = x.Name,
                EnterpriseID = x.EnterpriseID,
                Description = x.Description,
                Available = x.Available,
                Deleted = x.Deleted
            };
        }

        static public implicit operator ML.BC.EnterpriseData.Model.Department(DepartmentBase x)
        {
            return new ML.BC.EnterpriseData.Model.Department
            {
                ParentID = x.ParentID,
                Name = x.Name,
                EnterpriseID = x.EnterpriseID,
                Description = x.Description,
                Available = x.Available,
                Deleted = x.Deleted
            };
        }
    }
}
