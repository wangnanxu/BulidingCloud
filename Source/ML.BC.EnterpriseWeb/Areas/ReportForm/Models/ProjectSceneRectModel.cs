using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ML.BC.Services;
namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Models
{
    public class ProjectSceneRectModel
    {
        public int FinishCount { get; set; }
        public int RectCount { get; set; }
        public int AllCount { get; set; }
        public int PictureCount { get; set; }
        static public implicit operator ProjectSceneRectModel(RectificationStatisticalDto dto)
        {
         return new ProjectSceneRectModel(){
          AllCount=dto.AllCount,
         FinishCount=dto.FinishCount,
         PictureCount=dto.PictureCount,
         RectCount=dto.RectificationCount
         };
        }
    }

}