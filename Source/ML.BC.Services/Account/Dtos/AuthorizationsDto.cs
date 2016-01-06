using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.BCBackData.Model;

namespace ML.BC.Services
{
    public class AuthorizationsDto
    {
        public int RoleID { get; set; }
        public string FunctionID { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool Deleted { get; set; }

        static public implicit operator AuthorizationsDto(RFAAuthorization obj)
        {
            return new AuthorizationsDto
            {
                RoleID = obj.RoleID,
                FunctionID = obj.FunctionID,
                Deleted = obj.Deleted,
                UpdateTime = obj.UpdateTime
            };
        }

        static public implicit operator RFAAuthorization(AuthorizationsDto objDto)
        {
            return new RFAAuthorization()
            {
                RoleID = objDto.RoleID,
                FunctionID = objDto.FunctionID,
                Deleted = objDto.Deleted,
                UpdateTime = objDto.UpdateTime
            };
        }

    }
}
