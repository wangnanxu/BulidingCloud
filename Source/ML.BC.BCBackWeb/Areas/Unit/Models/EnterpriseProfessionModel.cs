using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ML.BC.Services;
namespace ML.BC.BCBackWeb.Areas.Unit.Models
{
    public class EnterpriseProfessionModel
    {
        [Required]
        [MaxLength(1)]
        public string EnterpriseProfessionID { get; set; }
        public string Name { get; set; }
        public bool Available { get; set; }
        [DefaultValue(1)]
        public int page;//use for pagination
        [DefaultValue(10)]
        public int rows;//use for pagination
    }
    public class EnterpriseProfessionResultModel
    {
        public int total { set; get; }
        public List<EnterpriseProfessionDto> rows { set; get; }
    }
}