using ML.BC.EnterpriseWeb.Areas.APIs.Models;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Web.Framework.Controllers;
using System.Web.Mvc;
using ML.BC.Web.Framework;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using System.Collections.Generic;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.MsmqHelper;
using ML.BC.Web.Framework.Security;

namespace ML.BC.EnterpriseWeb.Areas.APIs.Controllers
{
    [AuthorizeCheck]
    public class InitDataController : APIControllerBase
    {
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
                result.Value = service.GetEnterpriseForSync(BCSession.User.EnterpriseID);

                SyncStateDto model_Department = new SyncStateDto() { ActionType = (byte)TypeEnum.Department, DeviceID = BCSession.User.Device, UserID = BCSession.User.UserID };
                SyncStateDto model_FrontUser = new SyncStateDto() { ActionType = (byte)TypeEnum.User, DeviceID = BCSession.User.Device, UserID = BCSession.User.UserID };
                SyncStateDto model_UserRole = new SyncStateDto() { ActionType = (byte)TypeEnum.Role, DeviceID = BCSession.User.Device, UserID = BCSession.User.UserID };
                var synservice = Ioc.GetService<ISyncStateManagementService>();
                synservice.SetSyncState(model_Department, model_FrontUser, model_UserRole);
            });
            return result;
        }

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
                var synservice = Ioc.GetService<ISyncStateManagementService>();
                var SN = GetSession();
                SyncStateDto model_Project = new SyncStateDto() { ActionType = (byte)TypeEnum.Project, DeviceID = SN.User.Device, UserID = SN.User.UserID };
                SyncStateDto model_Scene = new SyncStateDto() { ActionType = (byte)TypeEnum.Scene, DeviceID = SN.User.Device, UserID = SN.User.UserID };
                synservice.SetSyncState(model_Project, model_Scene);

                result.Value = service.GetProjectAndSceneForSync(BCSession.User.EnterpriseID);
            });
            return result;
        }

        //  统一用同步数据的方法
        //[AllowCrossDomainPost]
        //public ActionResult SyncChatMessage(ModelBase model)
        //{
        //    return null;
        //}
    }
}
