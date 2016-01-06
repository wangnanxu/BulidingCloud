using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services.Common
{
    public class SearchLogConditionDto
    {
        public string UserID { get; set; }
        public string EnterpriseID { get; set; }
        public string UserName { get; set; }
        public string OperationID { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }



    }
}
