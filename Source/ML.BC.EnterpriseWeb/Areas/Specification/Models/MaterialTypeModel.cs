using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ML.BC.Services;
namespace ML.BC.EnterpriseWeb.Areas.Unit.Models
{
    public class MaterialTypeModel
    {
        public int MaterialTypeID { get; set; }
        public string Name { get; set; }
        public bool Avaliable { get; set; }
    }

    public class MaterialTypeResult
    {
        public int total { get; set; }
        public List<MaterialTypeDto> rows;
    }
}