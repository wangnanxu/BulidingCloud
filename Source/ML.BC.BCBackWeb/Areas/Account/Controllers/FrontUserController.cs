using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Web.Framework;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Infrastructure;
using ML.BC.BCBackWeb.Areas.Account.Models;
using ML.BC.Web.Framework.Security;
using ML.BC.BCBackData.Common;
using System.Linq;
//企业帐号管理
namespace ML.BC.BCBackWeb.Areas.Account.Controllers
{
    [Authorize]
    public class FrontUserController : BCControllerBase
    {
        private IEnterpriseManagementService _entService;//企业管理 查询  enterprises表
        private IEnterpriseRoleManagementService _entRoleServie;//企业角色管理  查询  rfaroles表 

        private IFrontUserRoleManagementService _userRoleService;//企业帐号角色管理  增删改查  userroles表
        private IFrontUserManagementService _userService;//企业帐号管理  增删改查  frontuser表

        public FrontUserController()
        {
            _entService = Ioc.GetService<IEnterpriseManagementService>();
            _entRoleServie = Ioc.GetService<IEnterpriseRoleManagementService>();
            _userRoleService = Ioc.GetService<IFrontUserRoleManagementService>();
            _userService = Ioc.GetService<IFrontUserManagementService>();

        }

        [PermissionControlAttribute(Functions.Root_EPManagement_EPUser)]
        public ActionResult Index()
        {
            return View();
        }

        [PermissionControlAttribute(Functions.Root_EPManagement_EPUser)]
        public ActionResult GetList(string entName, string userName, FrontUserViewModel model)
        {

            var result = new StandardJsonResult<FrontUserJsonResultModel>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                if (model.page < 1) model.page = 1;
                if (model.rows < 1) model.rows = 10;
                //获取所有
                if (entName == null && userName == null)
                {
                    int amount;
                    //查询front用户列表
                    var temp = _userService.GetAllFrontUser("", model.rows, model.page, out amount);
                    List<FrontUserJsonItemModel> list = new List<FrontUserJsonItemModel>();
                    foreach (var dto in temp)
                    {
                        var item = (FrontUserJsonItemModel)dto;
                        item.Roles = _userRoleService.GetAllEnterpriseRolesByFrontUserId(dto.UserID);
                        item.EnterpriseName = _entService.GetOneByEnterpriseID(dto.EnterpiseID).Name;
                        if (dto.DepartmentID == null)
                        {
                            item.DepartName = "";
                        }
                        else
                        {
                            item.DepartName = Ioc.GetService<IEnterpriseDepartmentManagementService>().GetDepartmentNameById(dto.DepartmentID ?? 0);
                        }
                        list.Add(item);
                    }
                    result.Value = new FrontUserJsonResultModel();
                    result.Value.rows = list;
                    result.Value.total = amount;
                }
                else //搜索模式
                {
                    int amount;
                    var rlist = new List<FrontUserJsonItemModel>();
                    var userList = _userService.SearchUserByName(entName, userName, model.page, model.rows, out amount);
                    foreach (var dto in userList)
                    {
                        var item = (FrontUserJsonItemModel)dto;
                        item.Roles = _userRoleService.GetAllEnterpriseRolesByFrontUserId(item.UserID);
                        item.EnterpriseName = _entService.GetOneByEnterpriseID(dto.EnterpiseID).Name;
                        rlist.Add(item);
                    }
                    result.Value = new FrontUserJsonResultModel();
                    result.Value.total = amount;//赋值
                    result.Value.rows = rlist;
                }


            });
            if (result.Success == true)
            {
                return new OringinalJsonResult<FrontUserJsonResultModel> { Value = result.Value };
            }
            else
            {
                return new OringinalJsonResult<FrontUserJsonResultModel> { Value = new FrontUserJsonResultModel() };
            }
        }

        //搜索  弃用，方法合并在getlist
        [PermissionControlAttribute(Functions.Root_EPManagement_EPUser)]
        public ActionResult Search(string entName, string userName, int rows, int page)
        {
            if (entName == null) entName = "";
            if (userName == null) userName = "";
            var result = new StandardJsonResult<FrontUserJsonResultModel>();
            result.Try(() =>
            {
                int amount;
                var rlist = new List<FrontUserJsonItemModel>();
                var userList = _userService.SearchUserByName(entName, userName, page, rows, out amount);
                foreach (var dto in userList)
                {
                    var item = (FrontUserJsonItemModel)dto;
                    item.Roles = _userRoleService.GetAllEnterpriseRolesByFrontUserId(item.UserID);
                    item.EnterpriseName = _entService.GetOneByEnterpriseID(dto.EnterpiseID).Name;
                    rlist.Add(item);
                }
                result.Value = new FrontUserJsonResultModel();
                result.Value.total = amount;//赋值
                result.Value.rows = rlist;
            });
            if (result.Success == true)
            {
                return new OringinalJsonResult<FrontUserJsonResultModel> { Value = result.Value };
            }
            else
            {
                return new OringinalJsonResult<FrontUserJsonResultModel> { Value = new FrontUserJsonResultModel() };
            }
        }

        ////按名称搜索帐号
        //[PermissionControlAttribute(Functions.Root_EPManagement_EPUser)]
        //public JsonResult SearchByName(string keyword)
        //{
        //    if (keyword == null) keyword = "";
        //    var result = new StandardJsonResult<FrontUserJsonResultModel>();
        //    result.Try(() =>
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            throw new KnownException(ModelState.GetFirstError());
        //        }


        //        int amount;
        //        var list = _userService.GetFrontUserByPartialInfo(new FrontUserDto { Name = keyword }, 30, 1, out amount);
        //        List<FrontUserJsonItemModel> rlist = new List<FrontUserJsonItemModel>();
        //        foreach (var dto in list)
        //        {
        //            var item = (FrontUserJsonItemModel)dto;
        //            item.Roles = _userRoleService.GetAllEnterpriseRolesByFrontUserId(item.UserID);
        //            //item.EnterpriseName = _entService.GetByid(ID).NAME;
        //            rlist.Add(item);
        //        }



        //        result.Value = new FrontUserJsonResultModel();
        //        result.Value.total = rlist.Count;//赋值
        //        result.Value.rows = rlist;
        //        //var x = new { total = amount, rows = list };
        //    });

        //}

        [PermissionControlAttribute(Functions.Root_EPManagement_EPRole_Add)]
        public ActionResult AddUser(NewFrontUserModel model)
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

                model.RegistDate = DateTime.Now;
                model.UpdateTime = DateTime.Now;
                model.LoginByDesktop = true;
                model.LoginByMobile = true;


                string uid = _userService.AddFrontUser(model);
                var roles = model.Roles;
                //processUserRoles(uid, roles);

                result.Value = !uid.Equals("0") + "";
            });
            return result;

        }

        [PermissionControlAttribute(Functions.Root_EPManagement_EPRole_Edit)]
        public ActionResult UpdateUser(NewFrontUserModel model)
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
                //processUserRoles(uid, roles);

                model.UpdateTime = DateTime.Now;

                bool value = _userService.UpdateFrontUser(model);
                result.Value = value.ToString();
            });
            return result;
        }

        private void processUserRoles(string userID, List<int> listNew)
        {
            _userRoleService.SetFrontUserRoles(userID, listNew);
            //if (listNew == null) return;
            //var service = new UserRolesServerce();
            //if (userID.Equals("0")) return;
            ////var rolelist = GetAllRolesByUserId(userID);
            //var listOld = service.GetAllRolesByUserId(userID);
            //foreach (var role in listOld)
            //{
            //    if (!listNew.Contains(role))
            //    {
            //        //deleteUserRole(userid,role.Roleid)
            //        service.DeleteUserRole(new UserRolesDto { RoleID = role, UserID = userID });
            //    }
            //}
            //List<int> IlistOld = new List<int>();
            //foreach (var role in listOld)
            //{
            //    IlistOld.Add(role);
            //}
            //foreach (var roleid in listNew)
            //{
            //    if (!IlistOld.Contains(roleid))
            //    {

            //        service.AddUserRole(new UserRolesDto { RoleID = roleid, UserID = userID });
            //        //addUserRole(userid,roleid)
            //    }
            //}

        }


        public ActionResult GetRoleList(string EnterpriseID)
        {
            int amount;
            List<EnterpriseRoleDto> list;
            if (EnterpriseID == null || EnterpriseID.Equals(""))
            {
                list = _entRoleServie.GetEnterpriseRoleList("", "", 999, 1, out amount);
            }
            else
            {
                list = _entRoleServie.GetEnterpriseRoleByEnterpriseID(EnterpriseID);
            }

            List<FrontUserRoleViewModel> rlist = new List<FrontUserRoleViewModel>();
            foreach (var role in list)
            {
                var rvm = new FrontUserRoleViewModel();
                if (role.OwnerID != null)
                {
                    rvm.RoleID = role.RoleID + "";
                    rvm.RoleName = role.Name;
                    rvm.group = "---" + role.EnterpriseName + "---";
                }
                else
                {
                    rvm.RoleID = role.RoleID + "";
                    rvm.RoleName = role.Name;
                    rvm.group = "---通用角色---";
                }


                rlist.Add(rvm);
            };
            rlist = rlist.OrderBy(r => r.group).ToList();



            return new OringinalJsonResult<List<FrontUserRoleViewModel>> { Value = rlist };
        }

        public ActionResult GetEntList()
        {
            var entList = _entService.GetAllEnterpriseList();
            //entList = entList.Where(e=>e.)
            var list = new List<FUEnterpriseViewModel>();
            foreach (var dto in entList)
            {
                var evm = new FUEnterpriseViewModel
                {
                    EnterpriseID = dto.EnterpriseID,
                    EnterpriseName = dto.Name

                };
                list.Add(evm);
            }
            return new OringinalJsonResult<List<FUEnterpriseViewModel>> { Value = list };
        }

        [PermissionControlAttribute(Functions.Root_EPManagement_EPRole_Delete)]
        public ActionResult DeleteUser(string FrontUserID)
        {
            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var value = _userService.DeleteFrontUser(FrontUserID);
                result.Value = value.ToString();
            });
            return result;
        }
    }
}
