using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.EnterpriseWeb.Areas.Scene.Models;
using ML.BC.EnterpriseWeb.Areas.Unit.Models;
using ML.BC.Web.Framework.Security;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.EnterpriseData.Common;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Web.ViewModels;
using ML.BC.EnterpriseWeb.Areas.ReportForm.Models;
namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Controllers
{
   [AuthorizeCheck]
    public class ProjectCompleteCountController : BCControllerBase
    {
        //
        // GET: /ReportForm/ProjectSceneCompleteCount/
       private IProjectSceneCompletionStatistics services;
        public ActionResult CompleteIndex()


        {
            return View();
        }
        public ProjectCompleteCountController()
        {
            services = ML.BC.Infrastructure.Ioc.GetService<IProjectSceneCompletionStatistics>();
        }
       [PermissionControlAttribute(Functions.Root_ReportManagement_ProjectCompleteCountDataReport_ShowAll)]
        public ActionResult GetList(Para model)
        {
            var result = new StandardJsonResult<CompleteCountResult>();
            result.Try(() =>
            {
                List<int> department = new List<int>();
                if(model.department!=null)
                foreach (var de in model.department.Split(','))
                {
                    department.Add(Convert.ToInt32( de));
                }
                ProjectSceneCompletionStatisticsDto list = services.GetReportForm(BCSession.User.EnterpriseID,model.beginTime,model.endTime,department,model.address);
                result.Value = new CompleteCountResult();
                result.Value.project = new ProjectCompleteCountModel() { 
                EndCount=list.ProjectData.EndCount,
                IngCount=list.ProjectData.IngCount,
                ReadyCount=list.ProjectData.ReadyCount
                };
                result.Value.scene = new ProjectCompleteCountModel()
                {
                    EndCount=list.SceneData.EndCount,
                 ReadyCount =list.SceneData.ReadyCount,
                  IngCount=list.SceneData.IngCount
                };
                
           
            });
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult GetDepartment()
        {
            var result = new StandardJsonResult<List<DepartmentTree>>();
            result.Try(() =>
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

    }
}
