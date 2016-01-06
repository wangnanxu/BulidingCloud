using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ML.BC.EnterpriseWeb.Areas.APIs.Models
{
    public class ModelBase
    {
        [Required]
        public string Token { get; set; }
    }
}