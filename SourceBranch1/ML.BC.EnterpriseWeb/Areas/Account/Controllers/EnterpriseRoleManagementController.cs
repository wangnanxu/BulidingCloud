using System.Collections.Generic;
using System.Web.Mvc;
using ML.BC.Infrastructure;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Services;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Web.Framework;
using ML.BC.EnterpriseWeb.Areas.Account.Models;
using ML.BC.Web.Framework.Security;
using ML.BC.EnterpriseData.Common;
using ML.BC.Web.Framework.ViewModels;
namespace ML.BC.EnterpriseWeb.Areas.Account.Controllers
{
    ///企业后端

    [AuthorizeCheck]
    public class EnterpriseRoleManagementController : BCControllerBase
    {
        private IEnterpriseRoleManagementService service;
        public EnterpriseRoleManagementController()
        {
            service = Ioc.GetService<IEnterpriseRoleManagementService>();
        }

        [PermissionControlAttribute(Functions.Root_SystemSetting_RoleManagement)]
        public ActionResult EnterpriseRoleIndex()
        {
            return View();
        }

        /// <summary>
        ///企业后端 企业角色列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [PermissionControlAttribute(Functions.Root_SystemSetting_RoleManagement_List)]
        public ActionResult GetEnterpriseRoleList(EnterpriseRoleSearchModel model)
        {

            var result = new StandardJsonResult<GridDataModelBase<EnterpriseRoleDto>>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                if (model.page < 1) model.page = 1;
                if (model.rows < 1) model.rows = 10;

                int count;
                // List<EnterpriseRoleDataModel> list = new List<EnterpriseRoleDataModel>();
                string _enterpriseID = GetSession().User.EnterpriseID;
                List<EnterpriseRoleDto> listdto = service.GetListToEP(model.Name, _enterpriseID, model.rows, model.page, out count);
                result.Value = new GridDataModelBase<EnterpriseRoleDto>();
                result.Value.total = count;
                result.Value.rows = listdto;

            });
            if (result.Success == false) result.Value = new GridDataModelBase<EnterpriseRoleDto>();
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
        [PermissionControlAttribute(Functions.Root_SystemSetting_RoleManagement_List)]
        public ActionResult GetEnterpriseRoleByRoleID(int RoleID)
        {

            var result = new StandardJsonResult<EnterpriseRoleDto>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var service = Ioc.GetService<IEnterpriseRoleManagementService>();
                EnterpriseRoleDto m = service.GetEnterpriseRoleByRoleID(RoleID);
                result.Value = m != null ? m : new EnterpriseRoleDto();
            });
            if (result.Success == false) result.Value = new EnterpriseRoleDto();
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }

        [PermissionControlAttribute(Functions.Root_SystemSetting_RoleManagement_Add)]
        public ActionResult AddEnterpriseRole(EnterpriseRoleDataModel model)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                model.OwnerID = GetSession().User.EnterpriseID;
                string i = service.AddEnterpriseRole(model);
                result.Value = i;
            });
            return result;
        }

        [PermissionControlAttribute(Functions.Root_SystemSetting_RoleManagement_Delete)]
        public ActionResult DeleteEnterpriseRole(int RoleID)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                if (RoleID != 1)
                {
                    var service = Ioc.GetService<IEnterpriseRoleManagementService>();
                    bool f = service.DeleteEnterpriseRoleToEP(RoleID);
                    result.Value = f + "";
                }
                else
                {
                    result.Value = "false";
                    result.Message = "不允许删除系统内置角色";
                    result.Success = false;
                }
            });
            return result;
        }

        [PermissionControlAttribute(Functions.Root_SystemSetting_RoleManagement_Edit)]
        public ActionResult UpdateEnterpriseRole(EnterpriseRoleDataModel model)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                model.OwnerID = GetSession().User.EnterpriseID;
                bool i = service.UpdateEnterpriseRole(model);
                result.Value = i + "";
            });
            return result;
        }
    }
}
