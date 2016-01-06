using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.BCBackData.Model;

namespace ML.BC.Services
{
    public class RolesDto
    {
        public int RoleID { get; set; }
        public string Name { get; set; }
        public string OwnerID { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
        public string FunctionIDs { get; set; }

        static public implicit operator RFARole(RolesDto roleDto)
        {
            return new RFARole()
            {
                RoleID = roleDto.RoleID,
                Name = roleDto.Name,
                OwnerID = roleDto.OwnerID,
                Description = roleDto.Description,
                Available = roleDto.Available
            };
        }

        static public implicit operator RolesDto(RFARole role)
        {
            return new RolesDto()
            {
                RoleID = role.RoleID,
                Name = role.Name,
                OwnerID = role.OwnerID,
                Description = role.Description,
                Available = role.Available
            };
        }
    }

}
