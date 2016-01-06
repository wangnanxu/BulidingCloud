using ML.BC.Services.Account.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Areas.Account.Models
{
    //page to controller
    public class BackUserViewModel
    {



        public int page { get; set; }


        public int rows { get; set; }
    }

    public class NewUserModel
    {
        public string UserID { get; set; }
      
        public List<int> Roles { get; set; }

        [Required]
        public string Name { get; set; }


        public string Password { get; set; }
        public string Mobile { get; set; }

        [DefaultValue(false)]
        public bool Closed { get; set; }

        public DateTime RegistDate { get; set; }
        public DateTime UpdateTime { get; set; }

        public static implicit operator UserDto(NewUserModel obj)
        {
            return new UserDto
            {
                Name = obj.Name,
                UserID = obj.UserID,
                Password = obj.Password,
                Mobile = obj.Mobile,
                Closed = obj.Closed
            };
        }


    }

    //per item in json
    public class UserJsonItemModel
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string RegistDate { get; set; }
        public string LastDate { get; set; }
        public string LastIP { get; set; }
        public bool Closed { get; set; }
        public string UpdateTime { get; set; }

        public List<string> Roles { get; set; }//role id

        public static implicit operator UserJsonItemModel(UserDto obj)
        {
            return new UserJsonItemModel
            {
                UserID = obj.UserID,
                Name = obj.Name,
                Mobile = obj.Mobile,
                RegistDate = obj.RegistDate.ToString("yyyy-MM-dd HH:mm"),
                
                LastDate = obj.LastDate==null?"":obj.LastDate.Value.ToString("yyyy-MM-dd HH:mm"),
                LastIP = obj.LastIP,
                Closed = obj.Closed,
                UpdateTime = obj.UpdateTime.ToString("yyyy-MM-dd HH:mm")
            };
        }
    }


    //controller to page
    public class UserJsonResultModel
    {
        public int total;
        public List<UserJsonItemModel> rows;
    }




    public class RoleViewModelx
    {
        public string RoleName;
        public string RoleID;
    }

   
}