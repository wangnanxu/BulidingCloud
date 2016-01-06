using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ML.BC.Services;
namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Models
{
    public class ScanCountModel
    {
        public int ProjectNum { get; set; }
        public int CheckProjectNum { get;set;}
        public int SceneNum { get; set; }
        public int CheckSceneNum { get; set; }
        static public implicit operator ScanCountModel(ScanReportDto sdto)
        {
            return new ScanCountModel()
            {
                ProjectNum = sdto.ProjectAll,
                CheckProjectNum = sdto.ProjectScan,
                SceneNum = sdto.SceneAll,
                CheckSceneNum = sdto.SceneScan
            };
        }
    }
}