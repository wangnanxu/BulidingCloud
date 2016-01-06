using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.Services;

namespace ML.BC.Services
{
    public interface IReviewStatistics
    {
        ReviewStatisticsDto GetReviewStatistics(string userID,DateTime? beginTime,DateTime? endTime);
    }
}
