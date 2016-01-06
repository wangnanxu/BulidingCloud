using ML.BC.Services.Unit.Dtos;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ML.BC.BCBackWeb.Areas.Unit.Models
{
    public class PropertyViewModel
    {



        public int page { set; get; }

        public int rows { set; get; }
    }


    public class PropertyNewModel
    {
        [Required]
        [MaxLength(1)]
        public string EnterprisePropertyID { get; set; }

        [Required]
        public string Name { get; set; }

        [DefaultValue(true)]
        public bool Available { get; set; }
    }

    public class PropResultModel{
        public int total { set; get; }
        public List<EnterprisePropertyDto> rows { set; get; }
    }
}