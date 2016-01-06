using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IInfoStatistics
    {
        InfoStatisticsDto GetBackInfoStatistics();
        InfoStatisticsDto GetEnterpriseInfoStatistics(string enterpriseId);
    }
}
