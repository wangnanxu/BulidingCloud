using ML.BC.EnterpriseData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.Infrastructure;

namespace ML.BC.Services.Account.Dtos
{
    public class UserDto
    {
        public List<int> Roles { get; set; }//role id
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Mobile { get; set; }
        public System.DateTime RegistDate { get; set; }
        public Nullable<System.DateTime> LastDate { get; set; }
        public string LastIP { get; set; }
        public bool Closed { get; set; }
        public System.DateTime UpdateTime { get; set; }
        static public implicit operator UserDto(ML.BC.BCBackData.Model.BackUser obj)
        {
            return new UserDto
            {
                UserID = obj.UserID,
                Name = obj.Name,
                Mobile = obj.Mobile,
                LastIP = obj.LastIP,
                LastDate = obj.LastDate,
                Password = obj.Password,
                UpdateTime = obj.UpdateTime,
                Closed = obj.Closed,
                RegistDate = obj.RegistDate
            };
        }

        static public implicit operator ML.BC.BCBackData.Model.BackUser(UserDto obj)
        {
            return new ML.BC.BCBackData.Model.BackUser
            {
                UserID = obj.UserID,
                Name = obj.Name,
                Mobile = obj.Mobile,
                LastIP = obj.LastIP,
                LastDate = obj.LastDate,
                Password = obj.Password,
                UpdateTime = obj.UpdateTime,
                Closed = obj.Closed,
                RegistDate = obj.RegistDate
            };
        }
    }
}
