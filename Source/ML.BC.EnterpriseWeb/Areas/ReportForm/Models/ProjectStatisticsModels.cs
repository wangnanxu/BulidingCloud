using ML.BC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Models
{
    public class ProjectStatisticsShowModel
    {
        public string projectId;
        public string projectName;
        public int typeCount1;
        public int typeCount2;
        public int typeCount3;
        public int typeCount4;
        public int typeCount5;
        public int typeCount6;
        public int typeCount7;
        public int typeCountTotal;

        static public implicit operator ProjectStatisticsShowModel(ML.BC.Services.ProjectStatisDto dto)
        {
            return new ProjectStatisticsShowModel
            {
                projectId = dto.projectId,
                projectName = dto.projectName,
                typeCount1 = dto.typeCount[0],
                typeCount2 = dto.typeCount[1],
                typeCount3 = dto.typeCount[2],
                typeCount4 = dto.typeCount[3],
                typeCount5 = dto.typeCount[4],
                typeCount6 = dto.typeCount[5],
                typeCount7 = dto.typeCount[6],
                typeCountTotal = dto.typeCount.Sum(x=>x==-1?0:x)
            };
        }
    }
    public class ProjectStatisticsQueryModel
    {
        public int page { get; set; }
        public int rows { get; set; }
        public string ProjectName { get; set; }
    }
    public class ProjectStatisticsResultModel
    {
        public int total;
        public List<ProjectStatisticsShowModel> rows;
        public List<ProjectStatisticsShowModel> footer;
    }
}