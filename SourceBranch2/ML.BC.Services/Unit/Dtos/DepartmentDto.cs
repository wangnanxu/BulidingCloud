using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class DepartmentDto:DepartmentBase
    {
        public int DepartmentID { get; set; }
        static public implicit operator DepartmentDto(ML.BC.EnterpriseData.Model.Department x)
        {
            return new DepartmentDto
            {
                DepartmentID = x.DepartmentID,
                ParentID = x.ParentID,
                Name = x.Name,
                EnterpriseID = x.EnterpriseID,
                Description = x.Description,
                Available = x.Available,
                Deleted = x.Deleted
            };
        }

        static public implicit operator ML.BC.EnterpriseData.Model.Department(DepartmentDto x)
        {
            return new ML.BC.EnterpriseData.Model.Department
            {
                DepartmentID = x.DepartmentID,
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
