using ML.BC.EnterpriseData.Model;
using ML.BC.EnterpriseData.MongoDb;
using ML.BC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using ML.BC.EnterpriseData.Common;
using ML.BC.EnterpriseData.Model.Extend;
using ML.BC.Infrastructure.MsmqHelper;
using ML.BC.Infrastructure;

namespace ML.BC.Services.Common
{
    public class AppSyncService : IAppSyncService
    {
        public bool AddUserMessageQueueItem(UserMessageQueueItem userMessaeQueueItem)
        {
            if (userMessaeQueueItem == null) throw new ArgumentNullException("userMessaeQueueItem");
            if (string.IsNullOrEmpty(userMessaeQueueItem.UserID) || string.IsNullOrEmpty(userMessaeQueueItem.Device))
                throw new KnownException("用户ID及设备不能为空");

            userMessaeQueueItem.Id = string.Empty;
            userMessaeQueueItem.State = AppSyncDataStateEnum.Normal;
            userMessaeQueueItem.UpdateTime = DBTimeHelper.DBNowTime();

            var db = new MongoDbProvider<UserMessageQueueItem>();
            db.Insert(userMessaeQueueItem);
            return true;
        }

        public List<UserMessageQueueItem> PopupUserMessageQueueItem(string userId, string device)
        {
            if (userId == null) throw new ArgumentNullException("userId");
            if (device == null) throw new ArgumentNullException("device");

            var db = new MongoDbProvider<UserMessageQueueItem>();
            var list = db.GetAll(n => n.State == AppSyncDataStateEnum.Normal && n.UserID == userId && n.Device == device).ToList();
            var result = list.ToArray().Clone() as UserMessageQueueItem[];
            foreach (var l in list)
            {
                l.Time = l.Time.ToLocalTime();
                var chatMessage = l.Data as ChatMessage;
                if (chatMessage == null) continue;
                chatMessage.SendTime = chatMessage.SendTime.ToLocalTime();
                chatMessage.IsRead = ReadStatus.Read;
            }
            list.ForEach(n =>
            {
                n.State = AppSyncDataStateEnum.Sent;
                n.UpdateTime = DBTimeHelper.DBNowTime();
                db.Update(n);
            });

            return null != result ? result.ToList() : new List<UserMessageQueueItem>();
        }

        public bool ConfirmSentSuccess(List<string> userMessaeQueueItemIDs)
        {
            if (userMessaeQueueItemIDs == null) throw new ArgumentNullException("userMessaeQueueItemIDs");
            var db = new MongoDbProvider<UserMessageQueueItem>();
            db.Delete(n => userMessaeQueueItemIDs.Any(m => n.Id == m) && n.State == AppSyncDataStateEnum.Sent);
            return true;
        }

        public bool ClearCache(string userID, string deviceID)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    if (!db.SyncStates.Any(o => o.UserID == userID && o.DeviceID == deviceID)) return true;
                    db.SyncStates.RemoveRange(db.SyncStates.Where(o => o.UserID == userID && o.DeviceID == deviceID));

                    var temp = db.UserLoginStates.FirstOrDefault(o => o.UserID == userID && o.Device == deviceID);
                    if (null != temp)
                    {
                        LoginLogWrite(db, temp, LoginStatus.Logout, "APP清除缓存");
                        db.UserLoginStates.Remove(temp);
                    }
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void LoginLogWrite(BCEnterpriseContext db, UserLoginState userLoginState, EnterpriseData.Common.LoginStatus status, string description)
        {
            if (userLoginState == null) return;
            var loginLog = new UserLoginLog
            {
                UserID = userLoginState.UserID,
                UserName = userLoginState.UserName,
                Device = userLoginState.Device,
                IP = userLoginState.LoginIP,
                Status = (int)status,
                Description = description
            };
            if (status == EnterpriseData.Common.LoginStatus.Login && userLoginState.LoginTime.HasValue)
            {
                loginLog.Time = userLoginState.LoginTime.Value;
            }
            else
            {
                loginLog.Time = ML.BC.EnterpriseData.Model.Extend.DBTimeHelper.DBNowTime(db);
            }

            LoginLogWrite(db, loginLog);
        }
        private void LoginLogWrite(BCEnterpriseContext db, UserLoginLog loginLog)
        {
            if (loginLog == null) return;
            db.UserLoginLogs.Add(loginLog);
            db.SaveChanges();
        }

        private const string CONFIRMSYNCSUCCESSLOCK = "ConfirmSyncSuccess";
        public bool ConfirmSyncSuccess(string userId, string device, AppSyncActionEnum action, DateTime syncTime)
        {
            lock (CONFIRMSYNCSUCCESSLOCK)
            {
                try
                {
                    using (var db = new BCEnterpriseContext())
                    {
                        var actionList = new List<int>();
                        switch (action)
                        {
                            case AppSyncActionEnum.SyncOrganization:
                                actionList.Add((int)TypeEnum.Department);
                                actionList.Add((int)TypeEnum.User);
                                actionList.Add((int)TypeEnum.Role);
                                break;
                            case AppSyncActionEnum.SyncProjectAndScene:
                                actionList.Add((int)TypeEnum.Project);
                                actionList.Add((int)TypeEnum.Scene);
                                break;
                            case AppSyncActionEnum.SyncSceneData:
                                actionList.Add((int)TypeEnum.SceneData);
                                break;
                        }

                        var syncStates =
                            db.SyncStates.Where(o => o.UserID == userId && o.DeviceID == device &&
                                                              actionList.Any(n => n == o.ActionType)).ToList();

                        syncStates.ForEach(n =>
                        {
                            if (n.SyncTime < syncTime)
                            {
                                n.SyncTime = syncTime;
                            }
                        });

                        return 0 < db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public FrontUserDto GetFrontUserDtoByID(string frontUserID)
        {
            using (var db = new BCEnterpriseContext())
            {
                var user = db.FrontUsers.Where(o => o.UserID == frontUserID).Select(o => new FrontUserDto()
                {
                    UserID = o.UserID,
                    Name = o.Name,
                    Picture = o.Picture,
                    EnterpiseID = o.EnterpiseID
                }).FirstOrDefault();
                if (user != null)
                {
                    user.Picture = UriExtensions.GetFullUrl(user.Picture);
                }
                return user;
            }
        }

        public List<SyncStateDto> GetExistSyncStateDtosBySceneItemID(string sceneItemID)
        {
            try
            {
                var mgdb = new MongoDbProvider<SceneItem>();
                var sceneID = mgdb.GetById(sceneItemID).SceneID;
                return GetExistSyncStateDtosBySceneID(sceneID);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private List<GroupedUser> SrializerWorkers(string workers)
        {
            try
            {
                return ML.BC.Infrastructure.Serializer.FromJson<List<GroupedUser>>(workers);
            }
            catch
            {
                throw new Exception("解析现场的工作人员出错！");
            }
        }

        #region 现场和现场数据的变动，消息分发逻辑

        //  分发给现场所属的项目下所有部门的所有人员
        public List<SyncStateDto> GetExistSyncStateDtosBySceneID(string sceneID)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var scene = db.Scenes.FirstOrDefault(o => o.SceneID == sceneID);
                    if (null == scene) return new List<SyncStateDto>();
                    var departmentIDs = db.Projects.Where(o => o.ProjectID == scene.ProjectID).Select(o => o.Departments).First();
                    var idList = departmentIDs.Split('|').ToList();
                    var ids = (from id in idList where !string.IsNullOrEmpty(id) select Convert.ToInt32(id)).ToList();
                    return GetExistSyncStateDtoByDepartMentIDs(ids);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //  分发给现场worker和项目manager
        //public List<SyncStateDto> GetExistSyncStateDtosBySceneID(string sceneID)
        //{
        //    try
        //    {
        //        using (var db = new BCEnterpriseContext())
        //        {
        //            var scene = db.Scenes.FirstOrDefault(o => o.SceneID == sceneID);
        //            if (null == scene) return new List<SyncStateDto>();
        //            var uIDs = SrializerWorkers(scene.Woker).SelectMany(n => n.UserID).Distinct().ToList();
        //            var managerIDs = db.Projects.Where(o => o.ProjectID == scene.ProjectID).Select(o => o.Managers).FirstOrDefault();
        //            uIDs.AddRange((managerIDs ?? string.Empty).Split('|'));
        //
        //            var sync = db.SyncStates.Where(obj => uIDs.Contains(obj.UserID)).Select(o => new
        //            {
        //                o.UserID,
        //                o.DeviceID,
        //            }).Distinct().Select(o => new SyncStateDto()
        //            {
        //                UserID = o.UserID,
        //                DeviceID = o.DeviceID
        //            }).ToList();
        //            return sync;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //
        //}
        #endregion

        #region 获取同步状态
        public List<SyncStateDto> GetExistSyncStateDtosByEnterpriseID(string EnterpriseID)
        {
            try
            {
                if (string.IsNullOrEmpty(EnterpriseID))
                {
                    using (var db = new BCEnterpriseContext())
                    {
                        var result = db.SyncStates.Select(n => new
                        {
                            n.UserID,
                            n.DeviceID
                        }).Distinct().Select(n => new SyncStateDto()
                        {
                            UserID = n.UserID,
                            DeviceID = n.DeviceID
                        }).ToList();
                        return result;
                    }
                }
                using (var db = new BCEnterpriseContext())
                {
                    var query = (from user in
                                     (from frontUser in db.FrontUsers
                                      where frontUser.EnterpiseID == EnterpriseID
                                      select frontUser)
                                 join sync in db.SyncStates on user.UserID equals sync.UserID
                                 select new
                                 {
                                     sync.UserID,
                                     sync.DeviceID
                                 }).Distinct().Select(o => new SyncStateDto()
                        {
                            UserID = o.UserID,
                            DeviceID = o.DeviceID
                        }).ToList();
                    return query;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<SyncStateDto> GetExistSyncStateDtosRelateFrontUserIDAtEnterprise(string userID)
        {
            using (var db = new BCEnterpriseContext())
            {
                var enterpriseID = db.FrontUsers.Where(o => o.UserID == userID).Select(o => o.EnterpiseID);
                return null == enterpriseID.FirstOrDefault() ? new List<SyncStateDto>() : GetExistSyncStateDtosByEnterpriseID(enterpriseID.First());
            }
        }

        public List<SyncStateDto> GetExistSyncStateDtosRelateDepartmentIDAtEnterprise(string DepartmentID)
        {
            using (var db = new BCEnterpriseContext())
            {
                var departmentID = Convert.ToInt32(DepartmentID);
                var enterpriseID = db.Departments.Where(o => o.DepartmentID == departmentID).Select(o => o.EnterpriseID);
                return null == enterpriseID.FirstOrDefault() ? new List<SyncStateDto>() : GetExistSyncStateDtosByEnterpriseID(enterpriseID.First());
            }
        }

        public List<SyncStateDto> GetExistSyncStateDtoByUserIDs(string userIDs)
        {
            var uIDs = userIDs.Split('|').ToList();
            using (var db = new BCEnterpriseContext())
            {
                if (string.IsNullOrEmpty(userIDs)) return new List<SyncStateDto>();
                var sync = db.SyncStates.Where(o => uIDs.Contains(o.UserID)).Select(o => new SyncStateDto()
                {
                    UserID = o.UserID,
                    DeviceID = o.DeviceID
                }).Distinct()
                .ToList();

                //  没有用手机端登陆过的人也应该把消息发送给他，默认指定Device为 "PC",以便电脑端能查看消息。
                var syncUiDs = sync.Select(o => o.UserID).ToList();
                var noSync = uIDs.Where(o => !syncUiDs.Contains(o)).ToList();
                sync.AddRange(noSync.Select(n => new SyncStateDto() { UserID = n, DeviceID = "PC" }));

                return sync;
            }
        }

        public List<SyncStateDto> GetExistSyncStateDtoByDepartMentIDs(List<int> departMentIDs)
        {
            using (var db = new BCEnterpriseContext())
            {
                var re = (from sync in db.SyncStates
                          join fu in db.FrontUsers on sync.UserID equals fu.UserID into tfu
                          from f in tfu.DefaultIfEmpty()
                          where departMentIDs.Contains(f.DepartmentID ?? 0)
                          select new SyncStateDto()
                          {
                              UserID = sync.UserID,
                              DeviceID = sync.DeviceID
                          })
                          .Distinct()
                          .ToList();

                return re;
            }
        }

        public List<SyncStateDto> GetExistSyncStateDtoByProjectID(string projectID)
        {
            using (var db = new BCEnterpriseContext())
            {
                var departmentIDs = db.Projects.Where(o => o.ProjectID == projectID).Select(o => o.Departments).First();
                var idList = departmentIDs.Split('|').ToList();
                var ids = (from id in idList where !string.IsNullOrEmpty(id) select Convert.ToInt32(id)).ToList();
                return GetExistSyncStateDtoByDepartMentIDs(ids);
            }
        }

        public List<SyncStateDto> GetExistSyncStateDtoByRoleID(int RoleID)
        {
            using (var db = new BCEnterpriseContext())
            {
                var role = db.RFARoles.FirstOrDefault(n => n.RoleID == RoleID);
                if (role == null) return new List<SyncStateDto>();
                if (string.IsNullOrEmpty(role.OwnerID))
                {
                    var result = db.SyncStates.Select(n => new
                                 {
                                     n.UserID,
                                     n.DeviceID
                                 }).Distinct().Select(n => new SyncStateDto()
                                 {
                                     UserID = n.UserID,
                                     DeviceID = n.DeviceID
                                 }).ToList();
                    return result;
                }
                else
                {
                    return GetExistSyncStateDtosByEnterpriseID(role.OwnerID);
                }
            }
        }

        #endregion

        public bool InitUserSyncState(string enterpiseId, string userId, string device)
        {
            using (var db = new BCEnterpriseContext())
            {
                var query = (from ss in db.SyncStates
                             where ss.DeviceID == device
                             join fu in db.FrontUsers on ss.UserID equals fu.UserID
                             where fu.EnterpiseID == enterpiseId
                             group ss by ss.ActionType into temp
                             select new { ActionType = temp.Key, SyncTime = temp.Max(n => n.SyncTime) }).Distinct()
                             .ToList();

                if (query.Any() && !db.SyncStates.Any(n => n.UserID == userId && n.DeviceID == device &&
                    (n.ActionType == (int)TypeEnum.Department || n.ActionType == (int)TypeEnum.User || n.ActionType == (int)TypeEnum.Role)))
                {
                    var list = query.Where(n => n.ActionType == (int)TypeEnum.Department || n.ActionType == (int)TypeEnum.User || n.ActionType == (int)TypeEnum.Role)
                        .Select(n => new SyncState
                                {
                                    ActionType = n.ActionType,
                                    DeviceID = device,
                                    UserID = userId,
                                    SyncTime = n.SyncTime,
                                });
                    db.SyncStates.AddRange(list);
                }

                if (!db.SyncStates.Any(n => n.UserID == userId && n.DeviceID == device &&
                   (n.ActionType == (int)TypeEnum.Project || n.ActionType == (int)TypeEnum.Scene)))
                {
                    var list = query.Where(n => n.ActionType == (int)TypeEnum.Project || n.ActionType == (int)TypeEnum.Scene)
                        .Select(n => new SyncState
                        {
                            ActionType = n.ActionType,
                            DeviceID = device,
                            UserID = userId,
                            SyncTime = n.SyncTime,
                        });
                    db.SyncStates.AddRange(list);
                }

                db.SaveChanges();
                return true;
            }
        }
    }
}
