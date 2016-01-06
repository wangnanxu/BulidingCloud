using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Areas.Account.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        public string Error { get; set; }
    }
}