using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services.Unit.Dtos
{
    public class EnterpriseSyncDto
    {
        public string EnterpriseID { get; set; }
        public DepartmentSyncDto[] Departments { get; set; }
        public FrontUserSyncDto[] FrontUsers { get; set; }
        public UserRoleSyncDto[] UserRoles { get; set; }
    }

    public class DepartmentSyncDto
    {
        public int DepartmentID { get; set; }
        public string Name { get; set; }
        public string EnterpriseID { get; set; }
        public int ParentID { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
        public bool Deleted { get; set; }

        public static implicit operator DepartmentSyncDto(ML.BC.EnterpriseData.Model.Department obj)
        {
            return new DepartmentSyncDto
            {
                DepartmentID = obj.DepartmentID,
                Name = obj.Name,
                EnterpriseID = obj.EnterpriseID,
                ParentID = obj.ParentID,
                Description = obj.Description,
                Available = obj.Available,
                Deleted = obj.Deleted
            };
        }
    }

    public class FrontUserSyncDto
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public bool Closed { get; set; }
        public string EnterpriseID { get; set; }
    }

    public class RoleSyncDto
    {
        public int RoleID { get; set; }
        public string Name { get; set; }
        public string OwnerID { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }

        public static implicit operator RoleSyncDto(ML.BC.EnterpriseData.Model.RFARole obj)
        {
            return new RoleSyncDto
            {
                RoleID = obj.RoleID,
                Name = obj.Name,
                OwnerID = obj.OwnerID,
                Description = obj.Description,
                Available = obj.Available
            };
        }
    }

    public class UserRoleSyncDto
    {
        public string UserID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string EnterpriseID { get; set; }
        public bool Deleted { get; set; }
    }
}
