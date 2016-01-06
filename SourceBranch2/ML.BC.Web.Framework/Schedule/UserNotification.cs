using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Web.Framework.Schedule
{
    public class UserNotification
    {
        public AppSyncActionEnum Action { get; set; }
        public DeletedEntity[] DeletedEntities { get; set; }
        public AppSyncDataTypeEnum SyncDataType { get; set; }
        public DateTime Time { get; set; }
    }

    public class DeletedEntity
    {
        public string EntityName { get; set; }
        public string EntityID { get; set; }
    }
}
