using ML.BC.Services;
using ML.BC.Services.Account.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Areas.Account.Models
{
    public class FrontUserViewModel
    {
        public int page { get; set; }
        public int rows { get; set; }
    }


    public class NewFrontUserModel
    {

        public string UserID { get; set; }//not null
        [Required]
        public string Name { get; set; }
        public string Password { get; set; }
        [Required]
        public string EnterpriseID { get; set; }//nn
        public string Mobile { get; set; }
        public DateTime RegistDate { get; set; }//nn
        public Nullable<DateTime> LastDate { get; set; }
        public string LastIP { get; set; }
        public bool LoginByDesktop { get; set; }//nn
        public bool LoginByMobile { get; set; }//nn
        [Required]
        public bool Closed { get; set; }
        public DateTime UpdateTime { get; set; }//nn
        public int? DepartmentID { get; set; }
        public List<int> Roles { get; set; }//role id  new!!!
        public static implicit operator FrontUserDto(NewFrontUserModel obj)
        {
            return new FrontUserDto
            {
                DepartmentID = obj.DepartmentID,
                Name = obj.Name,
                UserID = obj.UserID,
                Password = obj.Password,
                Mobile = obj.Mobile,
                Closed = obj.Closed,
                EnterpiseID = obj.EnterpriseID,
                LastDate = obj.LastDate,
                LastIP = obj.LastIP,
                LoginByDesktop = obj.LoginByDesktop,
                RegistDate = obj.RegistDate,
                LoginByMobile = obj.LoginByMobile,
                UpdateTime = obj.UpdateTime,
                UserRoleIDs = obj.Roles
            };
        }
    }

    //per item in json
    public class FrontUserJsonItemModel
    {
        public string UserID { get; set; }//not null
        public string Name { get; set; }
        public string Password { get; set; }
        public string EnterpriseID { get; set; }//nn
        public string Mobile { get; set; }
        public string RegistDate { get; set; }//nn
        public string LastDate { get; set; }
        public string LastIP { get; set; }
        public bool LoginByDesktop { get; set; }//nn
        public bool LoginByMobile { get; set; }//nn
        public bool Closed { get; set; }
        public string UpdateTime { get; set; }//nn
        public int? DepartmentID { get; set; }
        public string DepartName { get; set; }
        //----------------- custom
        public string EnterpriseName { get; set; }
        public List<int> Roles { get; set; }//role id  new!!!
        public static implicit operator FrontUserJsonItemModel(FrontUserDto obj)
        {
            var user = new FrontUserJsonItemModel();

            user.UserID = obj.UserID;
            user.Name = obj.Name;
            user.Mobile = obj.Mobile;
            user.RegistDate = obj.RegistDate.ToString("yyyy-MM-dd HH:mm");
            if (obj.LastDate == null)
            {
                user.LastDate = "";
            }
            else
            {
                user.LastDate = obj.LastDate.Value.ToString("yyyy-MM-dd HH:mm");
            }
            user.LastIP = obj.LastIP;
            user.Closed = obj.Closed;
            user.DepartmentID = obj.DepartmentID;
            user.DepartName = obj.DepartmentName;
            user.EnterpriseID = obj.EnterpiseID;
            user.LoginByDesktop = obj.LoginByDesktop;
            user.LoginByMobile = obj.LoginByMobile;
            if (obj.UpdateTime == null)
            {
                user.UpdateTime = "";
            }
            else
            {
                user.UpdateTime = obj.UpdateTime.ToString("yyyy-MM-dd HH:mm");
            }
            return user;
        }
    }


    //controller to page
    public class FrontUserJsonResultModel
    {
        public int total;
        public List<FrontUserJsonItemModel> rows;
    }




    public class FrontUserRoleViewModel
    {
        public string RoleName;
        public string RoleID;
        public string group;
    }


    public class FUEnterpriseViewModel
    {
        public string EnterpriseName;
        public string EnterpriseID;
        public string group;
    }

}