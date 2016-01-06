using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Services;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.EnterpriseWeb.Areas.Account.Models;
using ML.BC.EnterpriseWeb.Areas.Unit.Models;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services.Common;
using ML.BC.EnterpriseData.Common;
using ML.BC.EnterpriseWeb.Areas.ReportForm.Models;
using ML.BC.Web.Framework.Security;
namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Controllers
{
    [AuthorizeCheck]
    public class ProjectSceneExamineCountController : BCControllerBase
    {
        //
        // GET: /ReportForm/ProjectSceneRectCount/
        IReviewStatistics service = null;
        public ProjectSceneExamineCountController()
        {
            service = ML.BC.Infrastructure.Ioc.GetService<IReviewStatistics>();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ExamineIndex()
        {
            return View();
        }
        [PermissionControlAttribute(Functions.Root_ReportManagement_ProjectSceneExamineDataReport_ShowAll)]
        public ActionResult GetList(DateTime? start, DateTime? end, string user)
        {
            var result = new StandardJsonResult<int[]>();
            result.Try(() =>
            {
                ProjectSceneExamineModel list = service.GetReviewStatistics(user, start, end);
                int[] ret = new int[4];
                if (list != null)
                {
                    ret[0] = list.ProjectNum;
                    ret[1] = list.RectProjectNum;
                    ret[2] = list.SceneNum;
                    ret[3] = list.RectSceneNum;
                }
                result.Value = ret;
            });
            if (!result.Success)
            {
                result.Message = "数据获取失败！";
                result.Value = new int[4];
            }
            return result;
        }
        public ActionResult GetUser()
        {
            var result = new StandardJsonResult<List<UserTree>>();
            result.Try(() =>
            {
                List<FrontUserDto> userlist = GetUserList();
                //List< FrontUserDto> my = userlist.Where(u => u.UserID == BCSession.User.UserID).ToList();
                //if (my.Count > 0)
                //{
                //    userlist.Remove(my[0]);
                //}
                List<UserTree> departmentlist = new List<UserTree>();
                departmentlist.Add(new UserTree()
                {
                    iconCls = "icon-user",
                    children = new List<UserTree>(),
                    id = "",
                    text = "所有人",
                    type = 3

                });
                departmentlist = departmentlist.Concat(GetDepartment(userlist)).ToList();
                //所有人标签

                if (departmentlist.Count > 0)
                {
                    result.Value = new List<UserTree>();
                    result.Value = departmentlist;
                }
            });
            if (!result.Success)
                result.Value = new List<UserTree>();
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取所有部门转换成树形结构
        /// </summary>
        /// <returns></returns>
        
        public List<UserTree> GetDepartment(List<FrontUserDto> userlist)
        {

            var service = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseDepartmentManagementService>();
            List<DepartmentDto> list = new List<DepartmentDto>();
                list = service.GetMyDepartment(BCSession.User.EnterpriseID);
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
                List<UserTree> all = new List<UserTree>();

                var noDepartmentUser = userlist.Where(u => u.DepartmentID == null).ToList();
                foreach (var nouser in noDepartmentUser)
                {
                    all.Add(new UserTree()
                    {
                        id = nouser.UserID,
                        text = nouser.Name,
                        iconCls = "icon-user",
                        type = 1,
                        children = new List<UserTree>()
                    });
                }
                foreach (var rot in mylist)
                {
                    if (rot._parentId == 0)
                    {
                        UserTree root = new UserTree();
                        root.id = rot.DepartmentID.ToString();
                        root.text = rot.Name;
                        root.type = 0;
                        root.children = new List<UserTree>();
                        root.children = process2TreeModel(mylist, root, userlist);
                        var res = userlist.Where(u => u.DepartmentID == Convert.ToInt32(root.id));
                        if (res.Count() > 0)
                        {
                            foreach (var u in res)
                            {
                                root.children.Add(new UserTree()
                                {
                                    id = u.UserID,
                                    text = u.Name,
                                    type = 1,
                                    iconCls = "icon-user",
                                    children = new List<UserTree>()
                                });
                            }
                        }
                        all.Add(root);
                    }
                }
                return all;

            }
            else
                return new List<UserTree>();

        }
        /// <summary>
        /// 获取部门下所有用户
        /// </summary>
        /// <returns></returns>
       
        public List<FrontUserDto> GetUserList()
        {
            var service = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseDepartmentManagementService>();
            var userservice = ML.BC.Infrastructure.Ioc.GetService<IFrontUserManagementService>();
            List<DepartmentDto> list = new List<DepartmentDto>();
            List<FrontUserDto> userlist = new List<FrontUserDto>();
            List<int?> departmentlist = new List<int?>();
                list = service.GetMyDepartment(BCSession.User.EnterpriseID);
                foreach (var id in list)
                {
                    departmentlist.Add(id.DepartmentID);
                }
            
                    userlist = userservice.GetScanUsers(Functions.Root_ProjectManagement_SceneListManagement_VerifySceneData, null, BCSession.User.EnterpriseID, departmentlist);

            if (userlist != null && userlist.Count > 0)
                return userlist;
            else
                return new List<FrontUserDto>();

        }
        private List<UserTree> process2TreeModel(List<DepartmentManageModel> source, UserTree curRootNode, List<FrontUserDto> userlist)
        {

            List<UserTree> resultlist = new List<UserTree>();
            if (source != null && source.Count() > 0)//出口1
            {
                if (curRootNode == null)
                {
                    return resultlist;//出口2
                }

                //当前节点的直接子节点list
                IEnumerable<DepartmentManageModel> sublist = source.Where(m => m.ParentID == Convert.ToInt32(curRootNode.id));
                if (sublist == null || sublist.Count() == 0) return resultlist;
                // var pmodel=sublist.FirstOrDefault();

                foreach (var dep in sublist)//出口3
                {
                    UserTree t = new UserTree()
                    {
                        id = dep.DepartmentID.ToString(),
                        text = dep.Name,
                        type = 0
                    };
                    t.children = new List<UserTree>();
                    t.children = process2TreeModel(source, t, userlist);
                    var result = userlist.Where(u => u.DepartmentID == Convert.ToInt32(t.id));
                    if (result.Count() > 0)
                    {
                        foreach (var u in result)
                        {
                            t.children.Add(new UserTree()
                            {
                                id = u.UserID,
                                text = u.Name,
                                type = 1,
                                iconCls = "icon-user",
                                children = new List<UserTree>()
                            });
                        }
                    }
                    curRootNode.children.Add(t);
                }
                return curRootNode.children;
            }
            return new List<UserTree>();
        }


    }
}
