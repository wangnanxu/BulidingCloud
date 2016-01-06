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
    public class RoleManagementController : BCControllerBase
    {
        [PermissionControlAttribute(Functions.Root_SysManagement_SysRoleManagement)]
        public ActionResult RoleIndex()
        {
            return View();
        }

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [PermissionControlAttribute(Functions.Root_SysManagement_SysRoleManagement)]
        public ActionResult GetRoleList(RoleViewModel model)
        {

            var result = new StandardJsonResult<DataGridResultModelBase<RolesDto>>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }


                var service = Ioc.GetService<IRolesManagementService>();
                int count = 0;//test
                List<RolesDto> list = new List<RolesDto>();
                list = service.GetRoleList(new RolesDto { RoleID = model.RoleID??0, Name = model.Name, Description = model.Description }, model.rows, model.page, out count);
                result.Value = new DataGridResultModelBase<RolesDto>();
                result.Value.total = count;//赋值
                result.Value.rows = list;
            });
            if (!result.Success) {
                result.Value = new DataGridResultModelBase<RolesDto>();
            }
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRoleByRoleID(int RoleID)
        {

            var result = new StandardJsonResult<RolesDto>();result.Value=new RolesDto();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }


                var service = Ioc.GetService<IRolesManagementService>();
                int count = 0;//test
                List<RolesDto> list = new List<RolesDto>();
                list = service.GetRoleList(new RolesDto() {RoleID=RoleID },1,1, out count);
                if (list != null && list.Count > 0)
                {
                    result.Value = list[0];
                }
            });
            if (!result.Success) {
                result.Value = new RolesDto();
            }
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
        [PermissionControlAttribute(Functions.Root_SysManagement_SysRoleManagement_Add)]
        public ActionResult AddRole(RolesDto model)
        {

            var result = new StandardJsonResult<string>();
           // model.RoleID =10;
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    //throw new KnownException(ModelState.GetFirstError());
                }

                var service = Ioc.GetService<IRolesManagementService>();
                model.OwnerID = "";
                int i = service.AddRole(model);
                result.Value = i + "";//赋值
            });
            return result;
        }

        [PermissionControlAttribute(Functions.Root_SysManagement_SysRoleManagement_Delete)]
        public ActionResult DeleteRole(int RoleID)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }


                var service = Ioc.GetService<IRolesManagementService>();

                bool f = service.DeleteRole(RoleID);
                result.Value = f + "";
            });
            return result;
        }

        [PermissionControlAttribute(Functions.Root_SysManagement_SysRoleManagement_Edit)]
        public ActionResult UpdateRole(RolesDto model)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var service = Ioc.GetService<IRolesManagementService>();
                model.OwnerID = "";
                bool i = service.UpdateRole(model);
                result.Value = i + "";
            });
            return result;
        }
    }
}
