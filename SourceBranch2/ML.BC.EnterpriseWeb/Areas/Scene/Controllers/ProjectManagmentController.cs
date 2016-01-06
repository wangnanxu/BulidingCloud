using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Services;
using ML.BC.Infrastructure;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Infrastructure.Mvc;
using ML.BC.EnterpriseWeb.Areas.Scene.Models;
using ML.BC.EnterpriseWeb.Areas.Unit.Models;
using ML.BC.Web.Framework.ViewModels;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Web.Framework;
using ML.BC.EnterpriseData.Common;
using ML.BC.Web.Framework.Security;
namespace ML.BC.EnterpriseWeb.Areas.Scene.Controllers
{
    [AuthorizeCheck]
    public class ProjectManagmentController : BCControllerBase
    {
        private IProjectSceneManagementService service;
        public ActionResult ProjectListIndex()
        {
            return View();
        }
        public ProjectManagmentController()
        {
            service = ML.BC.Infrastructure.Ioc.GetService<IProjectSceneManagementService>();
        }

        [PermissionControlAttribute(Functions.Root_ProjectManagement_ProjectListManagement_List)]
        public ActionResult GetList(string projectid, string projectname, string manager, string searchstatus, int rows, int page)
        {
            var result = new StandardJsonResult<ProjectListResult>();
            result.Try(() =>
            {
                int amount = 0;
                Status status;
                if (string.IsNullOrEmpty(searchstatus))
                    status = Status.All;
                else
                    status = (Status)Convert.ToInt32(searchstatus);
                List<ProjectDto> list = new List<ProjectDto>();
                if (HasFunction(Functions.Root_ProjectManagement_ProjectListManagement_ShowAll))
                    list = service.SearchProjectOnEnterprise(projectid, projectname, manager, status, BCSession.User.EnterpriseID, rows, page, out amount);
                else if (BCSession.User.DepartmentID.HasValue)
                    list = service.SearchProjectOnDepartment(projectid, projectname, manager, status, BCSession.User.DepartmentID, BCSession.User.EnterpriseID, rows, page, out amount);

                result.Value = new ProjectListResult();
                result.Value.rows = list;
                result.Value.total = amount;
            });
            if (!result.Success)
                result.Value = new ProjectListResult();
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }

        [PermissionControlAttribute(Functions.Root_ProjectManagement_ProjectListManagement_Add)]
        public ActionResult AddProject(ProjectModel model)
        {
            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                ProjectDto project = new ProjectDto();
                project.BeginDate = model.BeginDate;
                project.Status = (Status)model.Status;
                project.Deleted = false;
                project.ProjectID = model.ProjectID;
                foreach (var a in model.Departments)
                {
                    project.Departments += a + "|";
                }
                project.Departments = project.Departments.Substring(0, project.Departments.Length - 1);
                foreach (var a in model.Roles)
                {
                    project.Roles += a + "|";
                }
                project.Roles = project.Roles.Substring(0, project.Roles.Length - 1);
                foreach (var a in model.Managers)
                {
                    project.Managers += a + "|";
                }
                project.Managers = project.Managers.Substring(0, project.Managers.Length - 1);
                project.Name = model.Name;
                project.EnterpriseID = BCSession.User.EnterpriseID;
                project.RegistDate = model.RegistDate;
                project.EndDate = model.EndDate;
                project.ProjectCode = model.ProjectCode;
                result.Value = service.AddProject(project);
            });
            if (!result.Success)
            {
                result.Value = "";
            }
            else
                result.Message = "添加成功";

            return result;

        }

        [PermissionControlAttribute(Functions.Root_ProjectManagement_ProjectListManagement_Edit)]
        public ActionResult UpdateProject(ProjectModel model)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                ProjectDto project = new ProjectDto();
                project.BeginDate = model.BeginDate;
                project.Status = (Status)model.Status;
                project.Deleted = false;
                project.ProjectID = model.ProjectID;
                project.Roles = model.Roles[0];
                foreach (var a in model.Departments)
                {
                    project.Departments += a + "|";
                }
                project.Departments = project.Departments.Substring(0, project.Departments.Length - 1);
                foreach (var a in model.Managers)
                {
                    project.Managers += a + "|";
                }
                project.Managers = project.Managers.Substring(0, project.Managers.Length - 1);
                project.Name = model.Name;
                project.EnterpriseID = BCSession.User.EnterpriseID;
                project.RegistDate = model.RegistDate;
                project.EndDate = model.EndDate;
                project.ProjectCode = model.ProjectCode;
                result.Value = service.UpdateProject(project);
                result.Message = "修改成功";
            });

            return result;
        }

        [PermissionControlAttribute(Functions.Root_ProjectManagement_ProjectListManagement_Delete)]
        public ActionResult DeleteProject(string projectid)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                if (string.IsNullOrEmpty(projectid))
                {
                    throw new KnownException("格式不正确");
                }
                result.Value = service.DeleteProject(projectid);
                result.Message = "删除成功";
            });

            return result;
        }

        [PermissionControlAttribute(Functions.Root_SystemSetting_RoleManagement_List)]
        public ActionResult GetRoleList()
        {
            var roleservice = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseRoleManagementService>();
            List<EnterpriseRoleDto> rolelist = new List<EnterpriseRoleDto>();
            rolelist = roleservice.GetEnterpriseRoleByEnterpriseID(BCSession.User.EnterpriseID);
            if (rolelist != null)
                return Json(rolelist, JsonRequestBehavior.AllowGet);
            else
                return Json(new List<EnterpriseRoleDto>(), JsonRequestBehavior.AllowGet);
        }
        [PermissionControlAttribute(Functions.Root_SystemSetting_OrganizationManagement_List)]
        public ActionResult GetDepartment()
        {
            var result = new StandardJsonResult<List<DepartmentTree>>();
            result.Try(() =>
            {
                var service = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseDepartmentManagementService>();
                List<DepartmentDto> list = new List<DepartmentDto>();
                if (HasFunction(Functions.Root_SystemSetting_OrganizationManagement_ShowAll))
                    list = service.GetMyDepartment(BCSession.User.EnterpriseID);
                else if (BCSession.User.DepartmentID.HasValue)
                    list = service.GetMyDepartment(BCSession.User.EnterpriseID, BCSession.User.DepartmentID);
                if (list != null && list.Count > 0)
                {
                    List<DepartmentManageModel> mylist = new List<DepartmentManageModel>();
                    foreach (var l in list)
                    {
                        if (l.Available)
                        {
                            DepartmentManageModel m = new DepartmentManageModel()
                            {
                                DepartmentID = l.DepartmentID,
                                Description = l.Description,
                                _parentId = l.ParentID,
                                Deleted = l.Deleted,
                                Available = l.Available,
                                EnterpriseID = l.EnterpriseID,
                                Name = l.Name,
                                ParentID = l.ParentID
                            };
                            mylist.Add(m);
                        }
                    }
                    mylist[0]._parentId = 0;//不在根节点部门下的管理者不能显示
                    List<DepartmentTree> all = new List<DepartmentTree>();
                    foreach (var rot in mylist)
                    {
                        if (rot._parentId == 0)
                        {
                            DepartmentTree root = new DepartmentTree();
                            root.id = rot.DepartmentID;
                            root.text = rot.Name;
                            root.children = new List<DepartmentTree>();
                            root.children = process2TreeModel(mylist, root);
                            all.Add(root);
                        }
                    }

                    result.Value = new List<DepartmentTree>();
                    result.Value = all;
                }
            });

            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
        [PermissionControlAttribute(Functions.Root_SystemSetting_UserManagement_List)]
        public ActionResult GetManager(string departmentlist)
        {
            var departservice = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseDepartmentManagementService>();
            List<DepartmentDto> departlist = new List<DepartmentDto>();
            List<int> searchlist = new List<int>();//根据部门ID获取用户
            if (!string.IsNullOrEmpty(departmentlist))
                foreach (var id in departmentlist.Split(','))
                {
                    searchlist.Add(Convert.ToInt32(id));
                }
            else
                return Json(new List<FrontUserDto>(), JsonRequestBehavior.AllowGet);
            var userservice = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseUserManagementService>();
            int amount;
            List<FrontUserDto> userlist = userservice.GetUserByDepartmentIDs(searchlist, 1000, 1, out amount);
            if (userlist != null)
                return Json(userlist, JsonRequestBehavior.AllowGet);
            else
                return Json(new List<FrontUserDto>(), JsonRequestBehavior.AllowGet);
        }
        private List<DepartmentTree> process2TreeModel(List<DepartmentManageModel> source, DepartmentTree curRootNode)
        {

            List<DepartmentTree> resultlist = new List<DepartmentTree>();
            if (source != null && source.Count() > 0)//出口1
            {
                if (curRootNode == null)
                {
                    return resultlist;//出口2
                }

                //当前节点的直接子节点list
                IEnumerable<DepartmentManageModel> sublist = source.Where(m => m.ParentID == curRootNode.id);
                if (sublist == null || sublist.Count() == 0) return resultlist;
                // var pmodel=sublist.FirstOrDefault();

                foreach (var dep in sublist)//出口3
                {
                    DepartmentTree t = new DepartmentTree()
                    {
                        id = dep.DepartmentID,
                        text = dep.Name,
                        children = process2TreeModel(source, dep)
                    };
                    curRootNode.children.Add(t);
                }
                return curRootNode.children;
            }
            return new List<DepartmentTree>();
        }

        // private List<DepartmentTreeModel> process2tree()

        public ActionResult AddScan(string projectId,int type)
        {
            var scanservice = ML.BC.Infrastructure.Ioc.GetService<IScan>();
            var result = new StandardJsonResult();
            result.Try(() =>
            {
                ScanDto para = new ScanDto()
                {
                    ObjectID = projectId,
                    Type = (ScanType)type,
                    UserID = BCSession.User.UserID
                };
                scanservice.AddScan(para);
            });
            if (!result.Success)
            {
                result.Message = "计数统计失败！";
            }
            return result;
        
        }
    }
}
