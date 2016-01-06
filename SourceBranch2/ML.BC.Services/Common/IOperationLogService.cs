using ML.BC.EnterpriseData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services.Common
{
    public interface IOperationLogService : IServiceBase
    {
        bool AddLog(OperationLog log);

        //  清空记录
        bool ClearLog();
        List<OperationLogDto> SearchLogsByCondition(SearchLogConditionDto condition, int pageSize, int pageIndex, out int count);
    }
}
