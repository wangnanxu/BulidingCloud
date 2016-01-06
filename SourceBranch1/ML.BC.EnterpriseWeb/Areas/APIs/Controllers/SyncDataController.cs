using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.EnterpriseWeb.Areas.APIs.Models;
using ML.BC.Services;
using ML.BC.Infrastructure;
using ML.BC.EnterpriseData.Model;
using ML.BC.Web.Framework.BaiduAPI;
using ML.BC.Web.Framework.ViewModels;
using ML.BC.Services.Common;
using ML.BC.Infrastructure.MsmqHelper;
using ML.BC.EnterpriseData.Common;
using ML.BC.Web.Framework.Security;


namespace ML.BC.EnterpriseWeb.Areas.APIs.Controllers
{
    [AuthorizeCheck]
    public class SyncDataController : APIControllerBase
    {
        private log4net.ILog _ilog = log4net.LogManager.GetLogger("LOG");
        private IAppSyncService service;
        public SyncDataController()
        {
            service = Ioc.GetService<IAppSyncService>();
        }

        [AllowCrossDomainPost]
        public ActionResult GetUserSyncMessages(ModelBase model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "GetUserSyncMessages", Serializer.ToJson(model)));

            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var messagelist = service.PopupUserMessageQueueItem(BCSession.User.UserID, BCSession.User.Device)
                    .Select(n => (UserNotification)n);
                var resultList = messagelist.GroupBy(n => n, new UserNotification_Comparer())
                    .Select(n => n.OrderByDescending(m => m.Time).First())
                    .ToList();

                result.Value = resultList;
            });

            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "GetUserSyncMessages", Serializer.ToJson(result)));
            return result;
        }

        #region common api
        [AllowCrossDomainPost]
        public ActionResult SyncConfirm(ModelBase model, AppSyncActionEnum syncAction, DateTime syncTime)
        {
            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var appSyncService = Ioc.GetService<IAppSyncService>();
                appSyncService.ConfirmSyncSuccess(BCSession.User.UserID, BCSession.User.Device, syncAction, syncTime);
            });
            return result;
        }

        /// <summary>
        /// clear app local cache
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowCrossDomainPost]
        public ActionResult ClearCache(ModelBase model)
        {
            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var appSyncService = Ioc.GetService<IAppSyncService>();
                appSyncService.ClearCache(BCSession.User.UserID, BCSession.User.Device);
            });
            return result;
        }

        [AllowCrossDomainPost]
        public ActionResult InitSyncState(ModelBase model)
        {
            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var appSyncService = Ioc.GetService<IAppSyncService>();
                appSyncService.InitUserSyncState(BCSession.User.EnterpriseID, BCSession.User.UserID, BCSession.User.Device);
            });
            return result;
        }
        #endregion

        #region sync organization
        [AllowCrossDomainPost]
        public ActionResult SyncOrganization(ModelBase model)
        {
            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var service = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseManagementService>();
                result.Value = service.GetEnterpriseForSync(BCSession.User.EnterpriseID, BCSession.User.UserID, BCSession.User.Device);
            });
            return result;
        }

        #endregion

        #region sync project and scene
        [AllowCrossDomainPost]
        public ActionResult SyncProjectAndScene(ModelBase model)
        {
            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var service = ML.BC.Infrastructure.Ioc.GetService<IProjectSceneManagementService>();
                result.Value = service.GetProjectAndSceneForSync(BCSession.User.UserID, BCSession.User.Device,
                    BCSession.User.EnterpriseID);

            });
            return result;
        }

        #endregion

        #region sync scene data
        [AllowCrossDomainPost]
        public ActionResult SyncSceneData(ModelBase model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "SyncSceneData", Serializer.ToJson(model)));
            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var service = ML.BC.Infrastructure.Ioc.GetService<ISceneItemManagementService>();
                result.Value = service.GetSceneItemForSync(BCSession.User.UserID, BCSession.User.Device);

                //var synservice = Ioc.GetService<ISyncStateManagementService>();
                //synservice.SetSyncState(new SyncStateDto() { ActionType = (byte)TypeEnum.SceneData, DeviceID = BCSession.User.Device, UserID = BCSession.User.UserID });
            });

            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "SyncSceneData", Serializer.ToJson(result)));
            return result;
        }
        #endregion
    }

    internal class UserNotification_Comparer : IEqualityComparer<UserNotification>
    {
        public bool Equals(UserNotification x, UserNotification y)
        {
            if (x == null && y != null) return false;
            if (x != null && y == null) return false;
            if (x != null && x != null)
            {
                if (x.Action != y.Action ||
                    x.SyncDataType != x.SyncDataType) return false;

                if (!(x.Data == null && y.Data == null) ||
                    !(x.DeletedEntities == null && y.DeletedEntities == null)
                    ) return false;
            }
            return true;
        }

        public int GetHashCode(UserNotification obj)
        {
            var temp = new UserNotification
            {
                Action = obj.Action,
                SyncDataType = obj.SyncDataType,
                Data = obj.Data,
                DeletedEntities = obj.DeletedEntities
            };

            var hashCode = Serializer.ToJson(temp).GetHashCode();
            return hashCode;
        }
    }
}