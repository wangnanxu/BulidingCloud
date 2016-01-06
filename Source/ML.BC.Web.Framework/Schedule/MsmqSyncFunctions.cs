using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using ML.BC.EnterpriseData.Model;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.MsmqHelper;
using ML.BC.Services;
using ML.BC.Services.Common;

namespace ML.BC.Web.Framework.Schedule
{
    public partial class MsmqSync
    {
        private static void ProcessSceneDataChanges(List<ML.BC.Infrastructure.MsmqHelper.MessageItem> messageItems)
        {
            try
            {
                if (null == messageItems || messageItems.Count == 0) return;
                foreach (var msg in messageItems)
                {
                    var usernoteItem = new UserMessageQueueItem();
                    List<Services.SyncStateDto> syncInfos;

                    if (OperationEnum.Deleted == msg.Operation)
                    {
                        //  删除操作
                        syncInfos = msg.Data.Exists(o => o.Key == "SceneID")
                        ? Service.GetExistSyncStateDtosBySceneID(msg.Data.Find(o => o.Key == "SceneID").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "SceneID");
                        var del = new[] { new DeletedEntity() };
                        if (msg.Data.Exists(o => o.Key == "Id"))
                            del[0] = new DeletedEntity { EntityID = msg.Data.Find(o => o.Key == "Id").Value, EntityName = msg.EntityName };
                        GeneralSetUserMessageQueueItem(usernoteItem, del, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
                    }
                    else
                    {
                        //  其他操作
                        syncInfos = msg.Data.Exists(o => o.Key == "Id")
                        ? Service.GetExistSyncStateDtosBySceneItemID(msg.Data.Find(o => o.Key == "Id").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "Id");
                        GeneralSetUserMessageQueueItem(usernoteItem, AppSyncActionEnum.SyncSceneData, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
                    }

                    UserMsgItem2Mongo(Service, syncInfos, usernoteItem);
                }
            }
            catch (Exception ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqSync));
                logger.Error(ex.Message);
            }

        }

        private static void ProcessMessageChanges(List<ML.BC.Infrastructure.MsmqHelper.MessageItem> messageItems)
        {
            try
            {
                if (null == messageItems || messageItems.Count == 0) return;
                foreach (var msg in messageItems)
                {
                    var usernoteItem = new UserMessageQueueItem();
                    if (OperationEnum.Added == msg.Operation)
                    {
                        FrontUserDto sendUserDto = new FrontUserDto();
                        var data = msg.Data.Find(o => o.Key == "SendUserID");
                        if (null != data) sendUserDto = Service.GetFrontUserDtoByID(msg.Data.Find(o => o.Key == "SendUserID").Value);
                        var userIDs = "";
                        if (msg.Data.Any(o => o.Key == "Recipients"))
                            userIDs = msg.Data.Find(o => o.Key == "Recipients").Value;
                        var syncInfos = Service.GetExistSyncStateDtoByUserIDs(userIDs);

                        GeneralSetUserMessageQueueItem(usernoteItem, AppSyncActionEnum.SyncMessage, AppSyncDataTypeEnum.Text, sendUserDto, msg);

                        UserMsgItem2Mongo(Service, syncInfos, usernoteItem);
                    }
                    else
                    {
                        var logger = log4net.LogManager.GetLogger(typeof(MsmqSync));
                        logger.Info("发送消息失败！" + msg.Type + (msg.Data.Exists(o => o.Key == "Recipients")
                            ? msg.Data.Find(o => o.Key == "Recipients").Value
                            : "获取相关信息失败"));
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqSync));
                logger.Error(ex.Message);
            }
        }

        //  为了以后会发送公告而单独为消息分发写的私有方法，目前只用到了Recipients
        private static List<Services.SyncStateDto> GetSyncUserInfos(List<CustomKeyValue> data)
        {
            var syncInfos = new List<SyncStateDto>();
            if (data.Any(o => o.Key == "EnterpriseID"))
                return Service.GetExistSyncStateDtosByEnterpriseID(data.Find(o => o.Key == "EnterpriseID").Value);
            if (data.Any(o => o.Key == "Recipients"))
                return Service.GetExistSyncStateDtoByUserIDs(data.Find(o => o.Key == "Recipients").Value);
            if (data.Any(o => o.Key == "SceneID"))
                return Service.GetExistSyncStateDtosBySceneID(data.Find(o => o.Key == "SceneID").Value);
            if (data.Any(o => o.Key == "DepartMentID"))
            {
                var idsString = data.FindAll(o => o.Key == "DepartMentID").Select(o => o.Value).ToList();
                var idsInt = new List<int>();
                idsInt.AddRange(idsString.Select(o => Convert.ToInt32(o)));
                return Service.GetExistSyncStateDtoByDepartMentIDs(idsInt);
            }
            return syncInfos;
        }
    }
}
