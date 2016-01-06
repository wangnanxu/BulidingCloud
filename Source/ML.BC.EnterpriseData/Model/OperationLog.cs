using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.MongoDb;

namespace ML.BC.EnterpriseData.Model
{
    /// <summary>
    /// 操作日志类
    /// </summary>
    public class OperationLog : MongoDBEntity
    {
        public string UserID;
        public string EnterpriseID;
        public string OperationID;
        public string OperationData;

        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime OperateTime;
        public string ClientIP;
    }
}
