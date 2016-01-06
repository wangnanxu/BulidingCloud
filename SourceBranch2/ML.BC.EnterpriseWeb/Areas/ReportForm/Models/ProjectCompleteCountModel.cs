using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Models
{
    public class CompleteCountResult
    {
        public ProjectCompleteCountModel project{get;set;}
        public ProjectCompleteCountModel scene { get; set; }
    }
    public class ProjectCompleteCountModel
    {
        public int EndCount { get; set; }
        public int ReadyCount { get; set; }
        public int IngCount { get; set; }
    }
    public class Para
    {
        public DateTime? beginTime { get; set; }
        public DateTime? endTime { get; set; }
        public string department { get; set; }
        public string address { get; set; }
    }
}