using ML.BC.Infrastructure.MsmqHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Common;
using ML.BC.EnterpriseData.Model;
using ML.BC.Infrastructure;
using ML.BC.Services;
using ML.BC.Services.Common;

namespace ML.BC.Web.Framework.Schedule
{
    public partial class MsmqSync
    {
        private static readonly IAppSyncService Service = Ioc.GetService<IAppSyncService>();
        public static void DistributeMessage(List<ML.BC.Infrastructure.MsmqHelper.MessageItem> messageItems)
        {
            if (messageItems == null && !messageItems.Any()) return;

            var organizationChanges = messageItems.Where(n => n.Type == TypeEnum.Department || n.Type == TypeEnum.User || n.Type == TypeEnum.Role)
                .OrderBy(n => n.ChangeTime).ToList();
            if (organizationChanges.Any())
            {
                ProcessOrganizationChanges(organizationChanges);
            }

            var projectSceneChanges = messageItems.Where(n => n.Type == TypeEnum.Project || n.Type == TypeEnum.Scene || n.Type == TypeEnum.SceneType)
                .OrderBy(n => n.ChangeTime).ToList();
            if (projectSceneChanges.Any())
            {
                ProcessProjectSceneChanges(projectSceneChanges);
            }

            var sceneDataChanges = messageItems.Where(n => n.Type == TypeEnum.SceneData).OrderBy(n => n.ChangeTime).ToList();
            if (sceneDataChanges.Any())
            {
                ProcessSceneDataChanges(sceneDataChanges);
            }

            var messageChanges = messageItems.Where(n => n.Type == TypeEnum.Message).OrderBy(n => n.ChangeTime).ToList();
            if (messageChanges.Any())
            {
                ProcessMessageChanges(messageChanges);
            }
        }

        #region Process Organization
        private static void ProcessOrganizationChanges(List<ML.BC.Infrastructure.MsmqHelper.MessageItem> messageItems)
        {
            try
            {
                if (null == messageItems || messageItems.Count == 0) return;

                var tempList = new List<OrganizationUserMsgItem2Mongo>();
                foreach (var messageItem in messageItems)
                {
                    switch (messageItem.Type)
                    {
                        case TypeEnum.Department:
                            var temp1 = ProcessDepartmentChanges(messageItem);
                            if (temp1 != null)
                                tempList.Add(temp1);
                            break;
                        case TypeEnum.User:
                            var temp2 = ProcessFrontUserChanges(messageItem);
                            if (temp2 != null)
                                tempList.Add(temp2);
                            break;
                        case TypeEnum.Role:
                            var temp3 = ProcessUserRoleChanges(messageItem);
                            if (temp3 != null)
                                tempList.Add(temp3);
                            break;
                    }
                }

                var list = tempList.GroupBy(n => n).ToList();
                list.ForEach(item =>
                    {
                        var syncInfoList = item.ToList().SelectMany(n => n.SyncInfo).Distinct().ToList();
                        var userNotifi = item.ToList().Select(n => n.UsernoteItem).OrderByDescending(n => n.Time).FirstOrDefault();
                        if (userNotifi != null)
                        {
                            UserMsgItem2Mongo(Service, syncInfoList, userNotifi);
                        }
                    });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static OrganizationUserMsgItem2Mongo ProcessDepartmentChanges(MessageItem msg)
        {
            try
            {
                if (null == msg) return null;
                var usernoteItem = new UserMessageQueueItem();

                List<Services.SyncStateDto> syncInfo;

                if (OperationEnum.Deleted == msg.Operation)
                {
                    //  删除操作
                    syncInfo = msg.Data.Exists(o => o.Key == "EnterpriseID")
                        ? Service.GetExistSyncStateDtosByEnterpriseID(msg.Data.Find(o => o.Key == "EnterpriseID").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "EnterpriseID");
                    var del = new[] { new DeletedEntity() };
                    if (msg.Data.Exists(o => o.Key == "DepartmentID"))
                        del[0] = new DeletedEntity { EntityID = msg.Data.Find(o => o.Key == "DepartmentID").Value, EntityName = msg.EntityName };
                    GeneralSetUserMessageQueueItem(usernoteItem, del, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
                }
                else
                {
                    //  其他操作
                    syncInfo = msg.Data.Exists(o => o.Key == "EnterpriseID")
                        ? Service.GetExistSyncStateDtosByEnterpriseID(msg.Data.Find(o => o.Key == "EnterpriseID").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "EnterpriseID");
                    GeneralSetUserMessageQueueItem(usernoteItem, AppSyncActionEnum.SyncOrganization, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
                }

                return new OrganizationUserMsgItem2Mongo
                {
                    SyncInfo = syncInfo,
                    UsernoteItem = usernoteItem
                };
                //UserMsgItem2Mongo(Service, syncInfo, usernoteItem);
            }
            catch (Exception ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
            }
            return null;
        }
        private static OrganizationUserMsgItem2Mongo ProcessFrontUserChanges(MessageItem msg)
        {
            try
            {
                if (null == msg) return null;
                var usernoteItem = new UserMessageQueueItem();

                List<Services.SyncStateDto> syncInfo;

                if (OperationEnum.Deleted == msg.Operation)
                {
                    //  删除操作
                    syncInfo = msg.Data.Exists(o => o.Key == "EnterpriseID")
                        ? Service.GetExistSyncStateDtosByEnterpriseID(msg.Data.Find(o => o.Key == "EnterpriseID").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "EnterpriseID");
                    var del = new[] { new DeletedEntity() };
                    if (msg.Data.Exists(o => o.Key == "UserID"))
                        del[0] = new DeletedEntity { EntityID = msg.Data.Find(o => o.Key == "UserID").Value, EntityName = msg.EntityName };
                    GeneralSetUserMessageQueueItem(usernoteItem, del, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
                }
                else
                {
                    //  其他操作
                    syncInfo = msg.Data.Exists(o => o.Key == "UserID")
                        ? Service.GetExistSyncStateDtosRelateFrontUserIDAtEnterprise(msg.Data.Find(o => o.Key == "UserID").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "UserID");
                    GeneralSetUserMessageQueueItem(usernoteItem, AppSyncActionEnum.SyncOrganization, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
                }

                return new OrganizationUserMsgItem2Mongo
                {
                    SyncInfo = syncInfo,
                    UsernoteItem = usernoteItem
                };
                //UserMsgItem2Mongo(Service, syncInfo, usernoteItem);
            }
            catch (Exception ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
            }
            return null;
        }
        private static OrganizationUserMsgItem2Mongo ProcessUserRoleChanges(MessageItem msg)
        {
            try
            {
                if (null == msg) return null;
                var usernoteItem = new UserMessageQueueItem();
                List<Services.SyncStateDto> syncInfo;

                if (OperationEnum.Deleted == msg.Operation)
                {
                    //  删除操作
                    var delList = new List<DeletedEntity>();
                    if ("UserRole" == msg.EntityName)
                    {
                        syncInfo = msg.Data.Exists(o => o.Key == "UserID")
                        ? Service.GetExistSyncStateDtosRelateFrontUserIDAtEnterprise(msg.Data.Find(o => o.Key == "UserID").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "UserID");

                        var delete = new DeletedEntity();
                        if (msg.Data.Exists(o => o.Key == "User_RoleID"))
                            delete = new DeletedEntity { EntityID = msg.Data.Find(o => o.Key == "User_RoleID").Value, EntityName = msg.EntityName };
                        delList.Add(delete);
                    }
                    else
                    {
                        syncInfo = msg.Data.Exists(o => o.Key == "EnterpriseID")
                        ? Service.GetExistSyncStateDtosByEnterpriseID(msg.Data.Find(o => o.Key == "EnterpriseID").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "EnterpriseID");
                        var delete = new DeletedEntity();
                        if (msg.Data.Exists(o => o.Key == "RoleID"))
                            delete = new DeletedEntity { EntityID = msg.Data.Find(o => o.Key == "RoleID").Value, EntityName = msg.EntityName };
                        delList.Add(delete);
                    }
                    DeletedEntity[] del = delList.ToArray();
                    GeneralSetUserMessageQueueItem(usernoteItem, del, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
                }
                else
                {
                    //  其他操作
                    if ("UserRole" == msg.EntityName)
                    {
                        syncInfo = msg.Data.Exists(o => o.Key == "UserID")
                        ? Service.GetExistSyncStateDtosRelateFrontUserIDAtEnterprise(msg.Data.Find(o => o.Key == "UserID").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "UserID");
                    }
                    else
                    {
                        var item = msg.Data.Find(o => o.Key == "RoleID");
                        int roleId = 0;
                        if (item != null)
                        {
                            int.TryParse(msg.Data.Find(o => o.Key == "RoleID").Value, out roleId);
                        }

                        syncInfo = Service.GetExistSyncStateDtoByRoleID(roleId);
                    }
                    GeneralSetUserMessageQueueItem(usernoteItem, AppSyncActionEnum.SyncOrganization, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
                }

                return new OrganizationUserMsgItem2Mongo
                {
                    SyncInfo = syncInfo,
                    UsernoteItem = usernoteItem
                };
                //UserMsgItem2Mongo(Service, syncInfo, usernoteItem);
            }
            catch (Exception ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
            }
            return null;
        }
        public class OrganizationUserMsgItem2Mongo
        {
            public List<Services.SyncStateDto> SyncInfo { get; set; }
            public UserMessageQueueItem UsernoteItem { get; set; }

            public override bool Equals(object obj)
            {
                var temp = obj as OrganizationUserMsgItem2Mongo;
                if (temp == null) return false;

                if (temp.UsernoteItem == null && this.UsernoteItem != null) return false;
                if (temp.UsernoteItem != null && this.UsernoteItem == null) return false;
                if (temp.UsernoteItem != null && this.UsernoteItem != null)
                {
                    if (temp.UsernoteItem.Action != this.UsernoteItem.Action ||
                        temp.UsernoteItem.SyncDataType != this.UsernoteItem.SyncDataType ||
                        temp.UsernoteItem.UserID != this.UsernoteItem.UserID ||
                        temp.UsernoteItem.Device != this.UsernoteItem.Device) return false;

                    if (!(temp.UsernoteItem.Data == null && this.UsernoteItem.Data == null) ||
                        !(temp.UsernoteItem.DeletedEntities == null && this.UsernoteItem.DeletedEntities == null)
                        ) return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                var temp = new UserMessageQueueItem();
                if (this.UsernoteItem != null)
                {
                    temp.Action = this.UsernoteItem.Action;
                    temp.SyncDataType = this.UsernoteItem.SyncDataType;
                    temp.UserID = this.UsernoteItem.UserID;
                    temp.Device = this.UsernoteItem.Device;
                    temp.Data = this.UsernoteItem.Data;
                    temp.DeletedEntities = this.UsernoteItem.DeletedEntities;
                }
                var hashCode = Serializer.ToJson(temp).GetHashCode();
                return hashCode;
            }
        }

        #endregion

        #region Process ProjectAndScene
        private static void ProcessProjectSceneChanges(List<ML.BC.Infrastructure.MsmqHelper.MessageItem> messageItems)
        {
            try
            {
                if (null == messageItems || messageItems.Count == 0) return;
                foreach (var messageItem in messageItems)
                {
                    switch (messageItem.Type)
                    {
                        case TypeEnum.Project: ProcessProjectChanges(messageItem); break;
                        case TypeEnum.Scene: ProcessSceneChanges(messageItem); break;
                        case TypeEnum.SceneType: ProcessSceneTypeChanges(messageItem); break;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static void ProcessSceneTypeChanges(MessageItem msg)
        {
            if (null == msg) return;
            var usernoteItem = new UserMessageQueueItem();

            List<Services.SyncStateDto> syncInfo;

            if (OperationEnum.Deleted == msg.Operation)
            {
                syncInfo = msg.Data.Exists(o => o.Key == "EnterpriseID")
                        ? Service.GetExistSyncStateDtosByEnterpriseID(msg.Data.Find(o => o.Key == "EnterpriseID").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "EnterpriseID");
                var del = new[] { new DeletedEntity() };
                if (msg.Data.Exists(o => o.Key == "SceneTypeID"))
                    del[0] = new DeletedEntity { EntityID = msg.Data.Find(o => o.Key == "SceneTypeID").Value, EntityName = msg.EntityName };
                GeneralSetUserMessageQueueItem(usernoteItem, del, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
            }
            else
            {
                syncInfo = msg.Data.Exists(o => o.Key == "EnterpriseID")
                        ? Service.GetExistSyncStateDtosByEnterpriseID(msg.Data.Find(o => o.Key == "EnterpriseID").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "EnterpriseID");
                GeneralSetUserMessageQueueItem(usernoteItem, AppSyncActionEnum.SyncProjectAndScene, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
            }
            UserMsgItem2Mongo(Service, syncInfo, usernoteItem);
        }

        private static void ProcessProjectChanges(MessageItem msg)
        {
            try
            {
                if (null == msg) return;
                var usernoteItem = new UserMessageQueueItem();

                List<Services.SyncStateDto> syncInfo;

                if (OperationEnum.Deleted == msg.Operation)
                {
                    //  删除操作
                    string departmentIDs = "-1";
                    var data = msg.Data.Find(o => o.Key == "DepartMentIDs");
                    if (null != data) departmentIDs = msg.Data.Find(o => o.Key == "DepartMentIDs").Value;
                    var idList = departmentIDs.Split('|').ToList();
                    var ids = idList.Select(id => Convert.ToInt32(id)).ToList();
                    syncInfo = Service.GetExistSyncStateDtoByDepartMentIDs(ids);
                    var del = new[] { new DeletedEntity() };
                    if (msg.Data.Exists(o => o.Key == "ProjectID"))
                        del[0] = new DeletedEntity { EntityID = msg.Data.Find(o => o.Key == "ProjectID").Value, EntityName = msg.EntityName };
                    GeneralSetUserMessageQueueItem(usernoteItem, del, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
                }
                else
                {
                    //  其他操作
                    syncInfo = msg.Data.Exists(o => o.Key == "ProjectID")
                        ? Service.GetExistSyncStateDtoByProjectID(msg.Data.Find(o => o.Key == "ProjectID").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "ProjectID");
                    GeneralSetUserMessageQueueItem(usernoteItem, AppSyncActionEnum.SyncProjectAndScene, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
                }

                UserMsgItem2Mongo(Service, syncInfo, usernoteItem);
            }
            catch (Exception ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
            }
        }
        private static void ProcessSceneChanges(MessageItem msg)
        {
            try
            {
                if (null == msg) return;
                var usernoteItem = new UserMessageQueueItem();

                List<Services.SyncStateDto> syncInfo;

                if (OperationEnum.Deleted == msg.Operation)
                {
                    //  删除操作
                    syncInfo = msg.Data.Exists(o => o.Key == "ProjectID")
                        ? Service.GetExistSyncStateDtoByProjectID(msg.Data.Find(o => o.Key == "ProjectID").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "ProjectID");
                    var del = new[] { new DeletedEntity() };
                    if (msg.Data.Exists(o => o.Key == "SceneID"))
                        del[0] = new DeletedEntity { EntityID = msg.Data.Find(o => o.Key == "SceneID").Value, EntityName = msg.EntityName };
                    GeneralSetUserMessageQueueItem(usernoteItem, del, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
                }
                else
                {
                    //  其他操作
                    syncInfo = msg.Data.Exists(o => o.Key == "ProjectID")
                        ? Service.GetExistSyncStateDtoByProjectID(msg.Data.Find(o => o.Key == "ProjectID").Value)
                        : GetError2Log4Net(msg.EntityName, msg.Operation, "ProjectID");
                    GeneralSetUserMessageQueueItem(usernoteItem, AppSyncActionEnum.SyncProjectAndScene, AppSyncDataTypeEnum.Entity, msg.ChangeTime);
                }

                UserMsgItem2Mongo(Service, syncInfo, usernoteItem);
            }
            catch (Exception ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
            }
        }
        #endregion

        #region Private Method

        private static List<SyncStateDto> GetError2Log4Net(string ent, OperationEnum op, string key)
        {
            var logger = log4net.LogManager.GetLogger(ent);
            logger.Error(string.Format("操作实体：{0},操作类型：{1},关键字：{2}", ent, op, key));
            return new List<SyncStateDto>();
        }
        private static void GeneralSetUserMessageQueueItem(UserMessageQueueItem item, DeletedEntity[] del, AppSyncDataTypeEnum dataType, DateTime changeTime)
        {
            item.DeletedEntities = del;
            item.SyncDataType = dataType;
            item.Time = changeTime;
        }
        private static void GeneralSetUserMessageQueueItem(UserMessageQueueItem item, AppSyncActionEnum action, AppSyncDataTypeEnum dataType, FrontUserDto sendUserInfo, MessageItem msg)
        {
            item.Action = action;
            item.SyncDataType = dataType;
            item.Time = msg.ChangeTime;

            var chatMsg = new ChatMessage();

            chatMsg.SendUserID = sendUserInfo.UserID;
            chatMsg.SendUserName = sendUserInfo.Name;
            chatMsg.SendUserPicture = sendUserInfo.Picture;
            chatMsg.EnterpriseID = sendUserInfo.EnterpiseID;
            chatMsg.IsRead = ReadStatus.NoRead;
            chatMsg.Recipients = msg.Data.Exists(o => o.Key == "Recipients") ? msg.Data.Find(o => o.Key == "Recipients").Value : "noRecipients";
            chatMsg.Message = msg.Data.Exists(o => o.Key == "Text") ? msg.Data.Find(o => o.Key == "Text").Value : "";
            chatMsg.SendTime = DateTime.SpecifyKind(Convert.ToDateTime(msg.Data.Exists(o => o.Key == "SendTime")
                ? msg.Data.Find(o => o.Key == "SendTime").Value
                : DateTime.MaxValue.ToString()), DateTimeKind.Local);
            chatMsg.MessageID = new Guid(msg.Data.Exists(o => o.Key == "MessageID")
                ? msg.Data.Find(o => o.Key == "MessageID").Value
                : Guid.Empty.ToString());
            item.Data = chatMsg;
        }
        private static void GeneralSetUserMessageQueueItem(UserMessageQueueItem item, AppSyncActionEnum action, AppSyncDataTypeEnum dataType, DateTime changeTime)
        {
            item.Action = action;
            item.SyncDataType = dataType;
            item.Time = changeTime;
        }
        private static void UserMsgItem2Mongo(IAppSyncService service, List<Services.SyncStateDto> syncInfos, UserMessageQueueItem msgItem)
        {
            if (null == syncInfos)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqSync));
                logger.Info("没有相关人员需要发送消息！");
            }
            else
            {
                foreach (var info in syncInfos)
                {
                    msgItem.UserID = info.UserID;
                    msgItem.Device = info.DeviceID;
                    service.AddUserMessageQueueItem(msgItem);
                }
            }
        }
        #endregion

    }
}
