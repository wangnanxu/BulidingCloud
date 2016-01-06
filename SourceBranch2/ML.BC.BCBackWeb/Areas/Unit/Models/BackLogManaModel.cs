using ML.BC.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Areas.Unit.Models
{
   
        public class BackLogGetModel
        {
            [DefaultValue(1)]
            public int page { get; set; }

            [DefaultValue(15)]
            public int rows { get; set; }
            public string EnterpriseId { get; set; }
            public string UserID { get; set; }
            public string UserName { get; set; }
            public string OperationID { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
        }


        public class BackLogViewModel
        {
            public string OperationLogID { get; set; }
            public string UserID { get; set; }
            public string OperationId { get; set; }
            public string OperationName { get; set; }
            public string OperationData { get; set; }
            public string OperateTime { get; set; }
            public string EnterpriseName { get; set; }
            public string ClientIP { get; set; }
            public string UserName { get; set; }

            static public implicit operator BackLogViewModel(OperationLogDto log)
            {
                return new BackLogViewModel
                {
                    OperateTime = log.OperateTime.ToString(),
                    UserID = log.UserID,
                    OperationId = log.OperationID,
                    OperationData = log.OperationData,
                    OperationLogID = log.Id,
                    ClientIP = log.ClientIP,
                    OperationName = log.OperationName,
                    UserName = log.UserName,

                };
            }
        }


        public class BackLogViewResultModel
        {
            public int total;
            public List<BackLogViewModel> rows;
        }
    }
