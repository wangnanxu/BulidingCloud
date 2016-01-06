using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ML.BC.Services;
namespace ML.BC.EnterpriseWeb.Areas.Scene.Models
{
    public class ScenesTypeModel
    {
        public int id { get; set; }
        public int _parentId { get; set; }
        public string Name { get; set; }
        public bool Available { get; set; }
    }
    public class ScenesTypeResultModel
    {
        public int total { get; set; }
        public List<ScenesTypeModel> rows { get; set; }
    }
}