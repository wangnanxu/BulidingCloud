using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ML.BC.EnterpriseWeb.Areas.Account.Models
{
    public class PersonalUpdateModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string newPassword { get; set; }
        public string newPassword2 { get; set; }
    }
    public class AvatarUpdateModel
    {
        
        public string actionx { get; set; }
        
        public string x { get; set; }
        public string y { get; set; }
        public string width { get; set; }
        public string height { get; set; }
        public string imgSrc { get; set; }
    }
}