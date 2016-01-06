using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ML.BC.EnterpriseWeb.Areas.APIs.Models
{
    public class KnowlegeModel:ModelBase
    {
//        public List<EnterpriseData.Model.KnowledgeBase> KnowlegeList { get; set; }
        public string FileName { get; set; }
//        public byte[] File { get; set; }
    }
}