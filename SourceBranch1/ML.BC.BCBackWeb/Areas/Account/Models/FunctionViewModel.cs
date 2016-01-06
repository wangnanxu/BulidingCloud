using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Areas.Account.Models
{
    public class FunctionViewModel
    {
   
            public string FunctionID { get; set; }
            public string Name { get; set; }
            public string MyID { get; set; }
            public string ParentID { get; set; }
            public string Desription { get; set; }
            public bool Available { get; set; }
            public int page { get; set; }
            public string name { get; set; }
            public int rows { set; get; }
        
    }
}