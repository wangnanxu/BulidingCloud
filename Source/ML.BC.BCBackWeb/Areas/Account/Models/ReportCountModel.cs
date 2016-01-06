using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Areas.Account.Models
{
    public class ReportCountModel
    {
        public int Users { get; set; }
        public int Projects { get; set; }
        public int Scenes { get; set; }
        public int Pictures { get; set; }
        public int ItemStatus { get; set; }
    }
}