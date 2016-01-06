using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ML.BC.Services;
namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Models
{
    public class ProjectSceneExamineModel
    {
        public int ProjectNum { get; set; }
        public int RectProjectNum { get; set; }
        public int SceneNum { get; set; }
        public int RectSceneNum { get; set; }
        static public implicit operator ProjectSceneExamineModel(ReviewStatisticsDto dto)
        {
            return new ProjectSceneExamineModel()
            {
                ProjectNum = dto.ProjectAll,
                RectProjectNum = dto.ProjectScan,
                SceneNum = dto.SceneAll,
                RectSceneNum = dto.SceneScan
            };
        }
    }

}