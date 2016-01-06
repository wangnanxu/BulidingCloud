using ML.BC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IProjectStatisticsService
    {
        ProjectStatisDto GetAllProjectStatisInfo(string enterpriseId, out int amount);
        List<ProjectStatisDto> GetProjectStatisInfo(string enterpriseId,string projName, int pageSize, int pageIndex, out  int amount);
    }
}
