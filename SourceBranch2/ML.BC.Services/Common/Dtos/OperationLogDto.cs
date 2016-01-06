using ML.BC.EnterpriseData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services.Common
{
    public class OperationLogDto : OperationLog
    {
        public string UserName { get; set; }
        public string OperationName { get; set; }   //  查询出功能的完整名称
    }
}
