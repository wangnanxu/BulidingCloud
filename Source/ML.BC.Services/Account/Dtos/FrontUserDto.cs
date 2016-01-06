using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model;
using ML.BC.Infrastructure;

namespace ML.BC.Services
{
    public class FrontUserDto
    {
        public string UserID { get; set; }
        public string DepartmentName { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string EnterpiseID { get; set; }
        public string Mobile { get; set; }
        public System.DateTime RegistDate { get; set; }
        public Nullable<System.DateTime> LastDate { get; set; }
        public string LastIP { get; set; }
        public bool LoginByDesktop { get; set; }
        public bool LoginByMobile { get; set; }
        public bool Closed { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public int? DepartmentID { get; set; }
        public string Picture { get; set; }
        public List<int> UserRoleIDs { get; set; } 

        static public implicit operator FrontUserDto(ML.BC.EnterpriseData.Model.FrontUser obj)
        {
            var u = new FrontUserDto()
            {
                UserID = obj.UserID,
                Name = obj.Name,
                EnterpiseID = obj.EnterpiseID,
                DepartmentID = obj.DepartmentID,
                Mobile = obj.Mobile,
                LastIP = obj.LastIP,
                LastDate = obj.LastDate,
                Password = obj.Password,
                UpdateTime = obj.UpdateTime,
                Closed = obj.Closed,
                RegistDate = obj.RegistDate,
                LoginByDesktop = obj.LoginByDesktop,
                LoginByMobile = obj.LoginByMobile
            };
            //patched by peng, to avoid throw exception when runing implicit type cast
            try
            {
                u.Picture = UriExtensions.GetFullUrl(obj.Picture);
            }
            catch (Exception)
            {
                u.Picture = obj.Picture;
            }
            return u;
        }

        static public implicit operator ML.BC.EnterpriseData.Model.FrontUser(FrontUserDto obj)
        {
            return new FrontUser()
            {
                UserID = obj.UserID,
                Name = obj.Name,
                EnterpiseID = obj.EnterpiseID,
                DepartmentID = obj.DepartmentID,
                Mobile = obj.Mobile,
                LastIP = obj.LastIP,
                LastDate = obj.LastDate,
                Password = obj.Password,
                UpdateTime = obj.UpdateTime,
                Closed = obj.Closed,
                RegistDate = obj.RegistDate,
                LoginByDesktop = obj.LoginByDesktop,
                LoginByMobile = obj.LoginByMobile

            };
        }
    }
}
