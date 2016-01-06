using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IPersonStatisticsService
    {
        List<PersonStatisDto> GetPersonStatisInfo(string enterpriseId, string userName, DateTime startTime, DateTime endTime, int pageSize, int pageIndex, out int amount);
        PersonStatisDto GetPersonStatisSummaryInfo(string enterpriseId);//合计
    }
}
