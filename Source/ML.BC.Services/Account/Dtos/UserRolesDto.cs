using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ML.BC.Services
{
    public class UserRolesDto
    {
        public string UserID { get; set; }
        public int RoleID { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool Delete { get; set; }

        static public implicit operator BCBackData.Model.UserRole(UserRolesDto objDto)
        {
            return new BCBackData.Model.UserRole()
            {
                UserID = objDto.UserID,
                RoleID = objDto.RoleID,
                UpdateTime = objDto.UpdateTime,
                Deleted = objDto.Delete
            };
        }

        static public implicit operator UserRolesDto(BCBackData.Model.UserRole obj)
        {
            return new UserRolesDto()
            {
                UserID = obj.UserID,
                RoleID = obj.RoleID,
                UpdateTime = obj.UpdateTime,
                Delete = obj.Deleted
            };
        }

        static public implicit operator EnterpriseData.Model.UserRole(UserRolesDto objDto)
        {
            return new EnterpriseData.Model.UserRole()
            {
                UserID = objDto.UserID,
                RoleID = objDto.RoleID,
                UpdateTime = objDto.UpdateTime,
                Deleted = objDto.Delete
            };
        }

        static public implicit operator UserRolesDto(EnterpriseData.Model.UserRole obj)
        {
            return new UserRolesDto()
            {
                UserID = obj.UserID,
                RoleID = obj.RoleID,
                UpdateTime = obj.UpdateTime,
                Delete = obj.Deleted
            };
        }

    }
}
