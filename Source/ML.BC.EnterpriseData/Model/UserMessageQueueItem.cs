using ML.BC.EnterpriseData.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Common;

namespace ML.BC.EnterpriseData.Model
{
    public enum AppSyncActionEnum
    {
        SyncOrganization = 1,
        SyncProjectAndScene = 2,
        SyncSceneData = 4,
        SyncMessage = 8
    }
    public enum AppSyncDataTypeEnum
    {
        Entity = 1,
        Text = 2,
        Image = 3,
        Vedio = 4
    }
    public enum AppSyncDataStateEnum
    {
        Normal = 1,
        Sent = 2,
        Overdue = 3
    }

    public class UserMessageQueueItem : UserNotification
    {
        public string UserID { get; set; }
        public string Device { get; set; }
        public AppSyncDataStateEnum State { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; }
    }

    public class UserNotification : MongoDBEntity
    {
        public AppSyncActionEnum Action { get; set; }
        public DeletedEntity[] DeletedEntities { get; set; }
        public Object Data { get; set; }
        public AppSyncDataTypeEnum SyncDataType { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }
    }

    public class ChatMessage
    {
        public string SendUserID { get; set; }
        public string Recipients { get; set; }
        public string SendUserName { get; set; }
        public string SendUserPicture { get; set; }
        public string EnterpriseID { get; set; }
        public string Message { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime SendTime { get; set; }
        public Guid MessageID { get; set; }
        public ReadStatus IsRead { get; set; }
    }

    public class DeletedEntity
    {
        public string EntityName { get; set; }
        public string EntityID { get; set; }
    }
}
