using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IProjectSceneCompletionStatistics
    {
        ProjectSceneCompletionStatisticsDto GetReportForm(string enterpriseId, DateTime? beginTime, DateTime? endTime, List<int> department, string address);
    }
}
