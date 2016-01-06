using ML.BC.BCBackData.Common;
using ML.BC.BCBackWeb.Areas.Account.Models;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Web.Framework;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Web.Framework.Security;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ML.BC.BCBackWeb.Areas.Account.Controllers
{
    [Authorize]
    public class BackUserController : BCControllerBase
    {
        private IAccountService _userManageService;
        public BackUserController()
        {
            _userManageService = ML.BC.Infrastructure.Ioc.GetService<IAccountService>();
        }

        [PermissionControlAttribute(Functions.Root_SysManagement_SysUserManagement)]
        public ActionResult Index()
        {
            return View();
        }

        [PermissionControlAttribute(Functions.Root_SysManagement_SysUserManagement)]
        public ActionResult GetList(string keyword,BackUserViewModel model)
        {
            var result = new StandardJsonResult<UserJsonResultModel>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                if (model.page < 1) model.page = 1;
                if (model.rows < 1) model.rows = 10;
                if (keyword == null)
                    keyword = "";
                    int amount;
                    var list = _userManageService.SearchUserByName(keyword, model.page, model.rows, out amount);

                    List<UserJsonItemModel> listr = new List<UserJsonItemModel>();
                    foreach (var dto in list)
                    {
                        UserJsonItemModel item = dto;
                        item.Roles = new List<String>();
                        var service = new UserRolesServerce();
                        var listi = service.GetAllRolesByUserId(dto.UserID);
                        List<string> t = new List<string>();
                        foreach (var iit in listi)
                        {
                            t.Add(iit + "");
                        }
                        item.Roles = t;
                        listr.Add(item);
                    }
                    result.Value = new UserJsonResultModel();
                    result.Value.total = amount;//赋值
                    result.Value.rows = listr;
                

            });
            if (result.Success == true)
            {
                return new OringinalJsonResult<UserJsonResultModel> { Value = result.Value };
            }
            else
            {
                return new OringinalJsonResult<UserJsonResultModel> { Value = new UserJsonResultModel() };
            }
        }

        [PermissionControlAttribute(Functions.Root_SysManagement_SysUserManagement)]
        public ActionResult Search(string keyword)
        {
            if (keyword == null) keyword = "";
            var result = new StandardJsonResult<UserJsonResultModel>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }


                int amount;
                var list = _userManageService.SearchUserByName(keyword, 1, 30, out amount);

                List<UserJsonItemModel> listr = new List<UserJsonItemModel>();
                foreach (var dto in list)
                {
                    UserJsonItemModel item = dto;
                    item.Roles = new List<String>();
                    var service = new UserRolesServerce();
                    var listi = service.GetAllRolesByUserId(dto.UserID);
                    List<string> t = new List<string>();
                    foreach (var iit in listi)
                    {
                        t.Add(iit + "");
                    }
                    item.Roles = t;
                    listr.Add(item);
                }
                result.Value = new UserJsonResultModel();
                result.Value.total = amount;//赋值
                result.Value.rows = listr;
                //var x = new { total = amount, rows = list };

            });
            return result;
        }

        [PermissionControlAttribute(Functions.Root_SysManagement_SysUserManagement_Add)]
        public ActionResult AddUser(NewUserModel model)
        {
            //NewUserModel tempModel = Serializer.FromJson<NewUserModel>(jsonstr);
            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                if (model.Password == null || model.Password.Equals("")) throw new KnownException("密码为空!");
                //UserDto model = tempModel;
                model.Closed = model.Closed;
                model.RegistDate = DateTime.Now;
                model.UpdateTime = DateTime.Now;

                string uid = _userManageService.CreateUser(model);
                var roles = model.Roles;
                processUserRoles(uid, roles);

                result.Value = !uid.Equals("0") + "";
            });
            return result;

        }

        [PermissionControlAttribute(Functions.Root_SysManagement_SysUserManagement_Edit)]
        public ActionResult UpdateUser(NewUserModel model)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                string uid = model.UserID;
                var roles = model.Roles ?? new List<int>();

                processUserRoles(uid, roles);

                model.UpdateTime = DateTime.Now;
                bool value = _userManageService.UpdateUser(model);
                result.Value = value.ToString();
            });
            return result;
        }

        private void processUserRoles(string userID, List<int> listNew)
        {
            if (listNew == null) return;
            var service = new UserRolesServerce();
            if (userID.Equals("0")) return;
            //var rolelist = GetAllRolesByUserId(userID);
            var listOld = service.GetAllRolesByUserId(userID);
            foreach (var role in listOld)
            {
                if (!listNew.Contains(role))
                {
                    //deleteUserRole(userid,role.Roleid)
                    service.DeleteUserRole(userID, role);
                }
            }
            List<int> IlistOld = new List<int>();
            foreach (var role in listOld)
            {
                IlistOld.Add(role);
            }
            foreach (var roleid in listNew)
            {
                if (!IlistOld.Contains(roleid))
                {

                    service.AddUserRole(userID, roleid);
                    //addUserRole(userid,roleid)
                }
            }

        }


        public ActionResult GetRoleList()
        {
            var service = new UserRolesServerce();
            var list = service.GetAllRoles();
            List<RoleViewModelx> rlist = new List<RoleViewModelx>();
            foreach (var role in list)
            {
                var vm = new RoleViewModelx();
                vm.RoleID = role.RoleID + "";
                vm.RoleName = role.Name;
                rlist.Add(vm);
            }
            return new OringinalJsonResult<List<RoleViewModelx>> { Value = rlist };
        }

        [PermissionControlAttribute(Functions.Root_SysManagement_SysUserManagement_Delete)]
        public ActionResult DeleteUser(string UserID)
        {
            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var value = _userManageService.DeleteUser(UserID);
                result.Value = value.ToString();
            });
            return result;
        }

    }
}
