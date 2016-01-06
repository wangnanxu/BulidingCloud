using ML.BC.EnterpriseWeb.Areas.Account.Models;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Web.Framework;
using ML.BC.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ML.BC.Web.Framework.Security;
using ML.BC.EnterpriseData.Common;

namespace ML.BC.EnterpriseWeb.Areas.Account.Controllers
{
    [AuthorizeCheck]
    public class EnterpriseUserController : BCControllerBase
    {
        private IEnterpriseUserManagementService _userService;
        private IEnterpriseRoleManagementService _entRoleService;
        private IFrontUserRoleManagementService _entUserRoleService;
        private IEnterpriseDepartmentManagementService _entDepartService;

        public EnterpriseUserController()
        {
            _userService = Ioc.GetService<IEnterpriseUserManagementService>();
            _entRoleService = Ioc.GetService<IEnterpriseRoleManagementService>();
            _entUserRoleService = Ioc.GetService<IFrontUserRoleManagementService>();
            _entDepartService = Ioc.GetService<IEnterpriseDepartmentManagementService>();
            
        }
        [PermissionControlAttribute(Functions.Root_SystemSetting_UserManagement)]
        public ActionResult Index()
        {
            return View();
        }

        //当前企业的用户列表
        [PermissionControlAttribute(Functions.Root_SystemSetting_UserManagement_List)]
        public ActionResult GetUserList(EnterpriseUserViewModel model)
        {
            string _enterpriseID = GetSession().User.EnterpriseID;
            
            if (model.depart == null)
            {
                model.depart = "";
            }
            if (model.name == null)
            {
                model.name = "";
            }
            if (model.page < 1)
            {
                model.page = 1;
            }
            if (model.rows < 1)
            {
                model.rows = 10;
            }
            var result = new StandardJsonResult<EnterpriseUserResultModel>();
            result.Try(() =>
            {

                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var temp = new List<FrontUserDto>();
                int amount;

                //获取所有用户模式 列表
                if (model.name.Equals("") && model.depart.Equals(""))
                {

                    temp = _userService.GetUserList(_enterpriseID, model.page, model.rows, out amount);
                }
                //搜索模式
                else
                {
                    temp = _userService.SearchUser(_enterpriseID, model.depart, model.name, model.page, model.rows, out amount);
                }


                List<EnterpriseUserJsonItemModel> list = new List<EnterpriseUserJsonItemModel>();
                foreach (var udto in temp)
                {
                    var item = (EnterpriseUserJsonItemModel)udto;
                    item.Roles = new List<int>();
                    var name = _entDepartService.GetDepartmentNameById(udto.DepartmentID ?? 0);
                    if (name == null) name = "";
                    item.DepartmentName = name;

                    var rolelist = _entUserRoleService.GetAllEnterpriseRolesByFrontUserId(udto.UserID);
                    if (rolelist == null) rolelist = new List<int>();
                    foreach (var rdto in rolelist)
                    {
                        item.Roles.Add((int)rdto);
                    }

                    list.Add(item);
                }

                result.Value = new EnterpriseUserResultModel();
                result.Value.rows = list;
                result.Value.total = amount;

            });
            if (result.Success == true)
            {
                return new OringinalJsonResult<EnterpriseUserResultModel> { Value = result.Value };
            }
            else
            {
                return new OringinalJsonResult<EnterpriseUserResultModel> { Value = new EnterpriseUserResultModel() };
            }
        }

        [PermissionControlAttribute(Functions.Root_SystemSetting_UserManagement_Add)]
        public ActionResult AddUser(EnterpriseUserNewModel model)
        {
            string _enterpriseID = GetSession().User.EnterpriseID;
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
                model.EnterpiseID = _enterpriseID;
                if (model.Roles == null) model.Roles = new List<int>();
                var dto = _userService.AddUser(model);
                result.Value = dto.UserID;
                var roles = model.Roles;
                //processUserRoles(dto.UserID, roles);

            });
            return  result;

        }
        [PermissionControlAttribute(Functions.Root_SystemSetting_UserManagement_Edit)]
        public ActionResult UpdateUser(EnterpriseUserNewModel model)
        {
            string _enterpriseID = GetSession().User.EnterpriseID;

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                model.UpdateTime = DateTime.Now;
                model.EnterpiseID = _enterpriseID;
                if (model.DepartmentID.Equals("0")) model.DepartmentID = null;
                if (model.Roles == null) model.Roles = new List<int>();
                model.EnterpiseID = _enterpriseID;
                var v = _userService.UpdateUser(model);
                //processUserRoles(model.UserID, model.Roles);
                result.Value = v.ToString();
            });
            return result;
        }

        //legacy method 
        private void processUserRoles(string userID, List<int> listNew)
        {
            _entUserRoleService.SetFrontUserRoles(userID, listNew);
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

        //当前企业的角色列表
        public ActionResult GetRoleList()
        {
            string _enterpriseID = GetSession().User.EnterpriseID;
            var result = new StandardJsonResult<List<EnterpriseRoleViewModel>>();
            result.Try(() =>
            {
                List<EnterpriseRoleDto> list = _entRoleService.GetEnterpriseRoleByEnterpriseID(_enterpriseID);

                List<EnterpriseRoleViewModel> rlist = new List<EnterpriseRoleViewModel>();
                foreach (var role in list)
                {
                    var rvm = new EnterpriseRoleViewModel
                    {
                        RoleID = role.RoleID + "",
                        RoleName = role.Name
                    };
                    rlist.Add(rvm);
                }
                result.Value = rlist;
            });
            if (result.Success)
            {
                return new OringinalJsonResult<List<EnterpriseRoleViewModel>> { Value = result.Value };
            }
            else
            {
                return new OringinalJsonResult<List<EnterpriseRoleViewModel>> { Value = new List<EnterpriseRoleViewModel>() };

            }
        }

        //当前企业的部门列表
        public ActionResult GetDepartList()
        {
            string _enterpriseID = GetSession().User.EnterpriseID;
            var result = new StandardJsonResult<List<DepartmentTreeModel>>();
            result.Try(() =>
            {
                //企业所有部门

                List<DepartmentDto> list = _entDepartService.GetAllAvaliableDepartmentByEnterpriseId(_enterpriseID);
                //DepartmentTreeModel root = list.Where(obj => obj.ParentID == 0).FirstOrDefault();// 根节点
                //virtual root
                DepartmentTreeModel root = new DepartmentTreeModel
                {
                    id = 0,
                    text = "",
                    children = new List<DepartmentTreeModel>()

                };
                //处理成树形结构 递归入口
                root.children = process2TreeModel(list, root);

                result.Value = new List<DepartmentTreeModel>();
                result.Value = root.children;
            });
            if (result.Success)
            {
                return new OringinalJsonResult<List<DepartmentTreeModel>> { Value = result.Value };
            }
            else
            {
                return new OringinalJsonResult<List<DepartmentTreeModel>> { Value = new List<DepartmentTreeModel>() };
            }
        }

        //递归构造树结构
        private List<DepartmentTreeModel> process2TreeModel(List<DepartmentDto> source, DepartmentTreeModel curRootNode)
        {

            List<DepartmentTreeModel> resultlist = new List<DepartmentTreeModel>();
            if (source != null && source.Count() > 0)//出口1
            {
                if (curRootNode == null)
                {
                    return resultlist;//出口2
                }

                //当前节点的直接子节点list
                IEnumerable<DepartmentDto> sublist = source.Where(m => m.ParentID == curRootNode.id);
                if (sublist == null || sublist.Count() == 0) return resultlist;
                // var pmodel=sublist.FirstOrDefault();

                foreach (var dep in sublist)//出口3
                {
                    DepartmentTreeModel t = new DepartmentTreeModel()
                    {
                        id = dep.DepartmentID,
                        text = dep.Name,
                        children = process2TreeModel(source, dep)
                    };
                    curRootNode.children.Add(t);
                }
                return curRootNode.children;
            }
            return new List<DepartmentTreeModel>();
        }
        [PermissionControlAttribute(Functions.Root_SystemSetting_UserManagement_Delete)]
        public ActionResult DeleteUser(string EnterpriseUserID)
        {
            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var value = _userService.DeleteUser(EnterpriseUserID);
                result.Value = value.ToString();
            });
            return result;
        }

    }
}

