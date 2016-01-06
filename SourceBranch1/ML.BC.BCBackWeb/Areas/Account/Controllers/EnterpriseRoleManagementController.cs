using System.Collections.Generic;
using System.Web.Mvc;
using ML.BC.Infrastructure;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Services;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Web.Framework;
using ML.BC.BCBackWeb.Areas.Account.Models;
using ML.BC.BCBackWeb.Model;
using ML.BC.Web.Framework.Security;
using ML.BC.BCBackData.Common;
namespace ML.BC.BCBackWeb.Areas.Account.Controllers
{
    [Authorize]
    public class EnterpriseRoleManagementController : BCControllerBase
    {
        private IEnterpriseRoleManagementService service;
        public EnterpriseRoleManagementController()
        {
            service = Ioc.GetService<IEnterpriseRoleManagementService>();
        }

        [PermissionControlAttribute(Functions.Root_EPManagement_EPRole)]
        public ActionResult EnterpriseRoleIndex()
        {
            return View();
        }

        /// <summary>
        /// 企业角色列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [PermissionControlAttribute(Functions.Root_EPManagement_EPRole)]
        public ActionResult GetEnterpriseRoleByList(EnterpriseRoleViewModel model)
        {

            var result = new StandardJsonResult<DataGridResultModelBase<EnterpriseRoleDto>>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var service = Ioc.GetService<IEnterpriseRoleManagementService>();
                int count = 0;//test
                List<EnterpriseRoleDto> list = new List<EnterpriseRoleDto>();
                list = service.GetEnterpriseRoleList(model.EnterpriseName, model.Name, model.rows, model.page, out count);
                result.Value = new DataGridResultModelBase<EnterpriseRoleDto>();
                result.Value.total = count;//赋值

                // result.Value.rows = Serializer.ToJson(list);
                result.Value.rows = list;

            });
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
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
                List<EnterpriseRoleDto> list = new List<EnterpriseRoleDto>();
                EnterpriseRoleDto m = service.GetEnterpriseRoleByRoleID(RoleID);
                result.Value = m!=null?m:new EnterpriseRoleDto();
            });
            if (result.Success == false) result.Value = new EnterpriseRoleDto();
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }

        [PermissionControlAttribute(Functions.Root_EPManagement_EPRole_Add)]
        public ActionResult AddEnterpriseRole(EnterpriseRoleDto model)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var service = Ioc.GetService<IEnterpriseRoleManagementService>();

                string i = service.AddEnterpriseRole(model);
                result.Value = i;//赋值
            });
            return result;
        }

        [PermissionControlAttribute(Functions.Root_EPManagement_EPRole_Delete)]
        public ActionResult DeleteEnterpriseRole(int RoleID)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                bool f;
                if (RoleID != 1)
                {
                    var service = Ioc.GetService<IEnterpriseRoleManagementService>();
                    f = service.DeleteEnterpriseRole(RoleID);
                }
                else
                {
                    f = false;
                    result.Message = "不允许删除系统内置角色.";
                }

                result.Value = f + "";
            });
            return result;
        }

        [PermissionControlAttribute(Functions.Root_EPManagement_EPRole_Edit)]
        public ActionResult UpdateEnterpriseRole(EnterpriseRoleDto model)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var service = Ioc.GetService<IEnterpriseRoleManagementService>();

                bool i = service.UpdateEnterpriseRole(model);
                result.Value = i + "";
            });
            return result;
        }
    }
}
