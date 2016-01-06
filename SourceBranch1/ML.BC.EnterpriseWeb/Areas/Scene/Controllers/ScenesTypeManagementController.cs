using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Infrastructure.Model;
using ML.BC.Web.Framework.Controllers;
using ML.BC.EnterpriseWeb.Areas.Scene.Models;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Web.Framework;
using ML.BC.Web.Framework.Security;
using ML.BC.EnterpriseData.Common;
namespace ML.BC.EnterpriseWeb.Areas.Scene.Controllers
{
    [AuthorizeCheck]
    public class ScenesTypeManagementController : BCControllerBase
    {
        ISceneTypeManagementService service = null;
        public ScenesTypeManagementController()
        {
            service = ML.BC.Infrastructure.Ioc.GetService<ISceneTypeManagementService>();
        }
        public ActionResult ScencesTypeIndex()
        {
            return View();
        }
         [PermissionControlAttribute(Functions.Root_SystemSetting_ScenetypeManagement_List)]
        public ActionResult GetList()
        {
            var result = new StandardJsonResult<ScenesTypeResultModel>();
            result.Try(() =>
            {

                List<ScenesTypeModel> ret = new List<ScenesTypeModel>();
                List<SceneTypeDto> list = service.GetAllSceneTypeList(BCSession.User.EnterpriseID);
                foreach (var all in list)
                {
                    ret.Add(new ScenesTypeModel()
                    {
                        id = all.ID,
                        Name = all.Name,
                        _parentId = all.ParentID.Value,
                        Available = all.Available
                    });
                }
                result.Value = new ScenesTypeResultModel();
                result.Value.rows = ret;
                result.Value.total = ret.Count;

            });
            if (!result.Success)
                result.Value = new ScenesTypeResultModel();
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }

        [PermissionControlAttribute(Functions.Root_SystemSetting_ScenetypeManagement_Add)]
        public ActionResult AddType(ScenesTypeModel model)
        {
            var result = new StandardJsonResult<int>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                SceneTypeDto scenetype = new SceneTypeDto
                {
                    ID = model.id,
                    ParentID = model._parentId,
                    Name = model.Name,
                    Available = model.Available,
                    EnterpriseID=BCSession.User.EnterpriseID
                };
                result.Value = service.AddSceneType(scenetype);
                result.Message = "提示：添加成功！";
            });
            if (!result.Success)
            {
                result.Message = "提示：添加失败";
            }
            return result;
        }

        [PermissionControlAttribute(Functions.Root_SystemSetting_ScenetypeManagement_Edit)]
        public ActionResult UpdateType(ScenesTypeModel model)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                SceneTypeDto scenetype = new SceneTypeDto
                {
                    ID = model.id,
                    ParentID = model._parentId,
                    Name = model.Name,
                    Available=model.Available,
                    EnterpriseID=BCSession.User.EnterpriseID
                };
                result.Value = service.UpdateSceneType(scenetype);
                result.Message = "提示：修改成功！";
            });
            if (!result.Success)
            {
                result.Message = "提示：修改失败";
            }
            return result;
        }

        [PermissionControlAttribute(Functions.Root_SystemSetting_ScenetypeManagement_Delete)]
        public ActionResult DeleteType(int TypeID)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                result.Value = service.DeleteSceneType(TypeID);
                result.Message = "提示：删除成功！";
            });
            if (!result.Success)
            {
                result.Message = "提示：删除失败！";
            }
            return result;
        }
    }
}
