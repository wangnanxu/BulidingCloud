using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ML.BC.EnterpriseWeb.Areas.Unit.Models;
using ML.BC.Web.Framework.Security;
using ML.BC.EnterpriseData.Common;
using ML.BC.EnterpriseWeb.Areas.Account.Models;
namespace ML.BC.EnterpriseWeb.Areas.Unit.Controllers
{
    [AuthorizeCheck]
    public class DepartmentManageController : BCControllerBase
    {
        private IEnterpriseDepartmentManagementService service;
        private DepartmentTreeHelper _helper = new DepartmentTreeHelper();
        public DepartmentManageController()
        {
            service = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseDepartmentManagementService>();
        }
        public ActionResult EnterpriseDepartmentIndex()
        {
            return View();
        }
       [PermissionControlAttribute(Functions.Root_SystemSetting_OrganizationManagement_List)]
        public ActionResult GetDepartmentlist()
        {
            #region
            var result = new StandardJsonResult<DepartmentManageResultModel>();
            result.Try(() =>
            {
               var userservice = ML.BC.Infrastructure.Ioc.GetService<IFrontUserManagementService>();
                List<DepartmentDto> list=new List<DepartmentDto>();
                if(HasFunction(Functions.Root_SystemSetting_OrganizationManagement_ShowAll))
               list = service.GetMyDepartment(BCSession.User.EnterpriseID);
                else if(BCSession.User.DepartmentID.HasValue)
                    list = service.GetMyDepartment(BCSession.User.EnterpriseID, BCSession.User.DepartmentID);
                if (list != null && list.Count>0)
                {
                    List<DepartmentList> jsonlist = new List<DepartmentList>();
                    foreach (var d in list)
                    {
                        var dlist = new DepartmentList();
                        dlist.Available = d.Available;
                        dlist.Deleted = d.Deleted;
                        dlist.DepartmentID = d.DepartmentID;
                        dlist.Name = d.Name;
                        dlist.Description = d.Description;
                        dlist.ParentID = d.ParentID;
                        dlist.id = d.DepartmentID;
                        dlist._parentId = d.ParentID;
                        dlist.EnterpriseID = d.EnterpriseID;
                        jsonlist.Add(dlist);
                        //dlist.id = _helper.processSID(fun.FunctionID);
                        //dlist._parentId = _helper.processSID(fun.ParentID);
                    }
                    jsonlist[0]._parentId = 0;
                    result.Value = new DepartmentManageResultModel();
                    result.Value.total = list.Count;
                    result.Value.rows = jsonlist;
                }
                else
                {
                    result.Value = new DepartmentManageResultModel();
                    result.Value.total = 0;
                    result.Value.rows = new List<DepartmentList>();
                }
            });
            if (!result.Success)
            {
                result.Value = new DepartmentManageResultModel();
            }
           
            return Json(result.Value, JsonRequestBehavior.AllowGet);
            #endregion
      
        }

        [PermissionControlAttribute(Functions.Root_SystemSetting_OrganizationManagement_Add)]
        public StandardJsonResult AddDepartment(string funcJsonStr)
        {
            var model = Serializer.FromJson<DepartmentDto>(funcJsonStr);
            var result = new StandardJsonResult<int>();
           
            result.Try(() =>
            {
                if (model.Name == null )
                {
                    result.Success = false;
                    result.Message = "传递到服务器的参数不合法";
                    result.Value = -1;
                }
                else
                {
                    //model.MyID = model.MyID.Replace(" ", "");
                    //model.FunctionID = model.ParentID + "." + model.MyID;
                    //res.Value = service.Add(model);
                    model.EnterpriseID = BCSession.User.EnterpriseID;
                    result.Value = service.Add(model);
                    result.Message = "添加成功！";
                }
            });
            if (!result.Success)
            {
                result.Message = "添加失败！";
                result.Value = -1;
            }
            return result;
        }

        [PermissionControlAttribute(Functions.Root_SystemSetting_OrganizationManagement_Edit)]
        public StandardJsonResult UpdateDepartment(string funcJsonStr)
        {
            var res = new StandardJsonResult<bool>();
            res.Try(() =>
            {
                var model = Serializer.FromJson<DepartmentDto>(funcJsonStr);
                if (  model.Name.Equals(""))
                {
                    res.Success = false;
                    res.Message = "参数不合法";
                    res.Value = false;
                }
                else
                {
                    res.Value = service.Update(model);
                    res.Message = "修改成功！";
                }
            });
            if (!res.Success)
            {
                res.Value = false;
                res.Message = "修改失败";
            }
            return res;
        }

        [PermissionControlAttribute(Functions.Root_SystemSetting_OrganizationManagement_Delete)]
        public StandardJsonResult DeleteDepartment(string funcJsonStr)
        {
            var res = new StandardJsonResult<bool>();
            res.Try(() =>
            {
                var model = Serializer.FromJson<DepartmentDto>(funcJsonStr);
                if (model.DepartmentID==0)
                {
                    res.Value = false;
                    res.Success = false;
                    res.Message = "参数不合法,id为空";
                }
                else
                {
                    res.Value = service.Delete(model.DepartmentID,BCSession.User.EnterpriseID);
                    res.Message = "删除成功！";
                }
            });
          
            return res;
        }

        //返回编辑时的树结构，弃用
        public ActionResult GetMyDepartment(string did)
        {
            var result = new StandardJsonResult<List<DepartmentTreeModel>>();
            result.Try(() => {
                var service = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseDepartmentManagementService>();
                List<DepartmentDto> list = service.GetMyDepartment(BCSession.User.EnterpriseID, BCSession.User.DepartmentID);
                list[0].ParentID = 0;
                if (list == null) throw new ArgumentNullException("func list get from service is null!");
                DepartmentTreeModel root = new DepartmentTreeModel();
                root.id = list[0].DepartmentID;
                root.text = list[0].Name;
                root.children = new List<DepartmentTreeModel>();
                root.children = process2TreeModel(list, root,Convert.ToInt32(did));
                result.Value = new List<DepartmentTreeModel>();
                result.Value.Add(root);
            
            });
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
        //获取当前部门的父部门名字
        [PermissionControlAttribute(Functions.Root_SystemSetting_OrganizationManagement_List)]
        public ActionResult GetParentInfo(string DepartmentID)
        {
            var result = new StandardJsonResult<string>();
            result.Try(() => {
                DepartmentDto par = service.GetParentDepartment(Convert.ToInt32(DepartmentID));
                if (par != null)
                    result.Value = par.Name;
                else
                    result.Value = "空";
            });
            return result;
        }
        private List<DepartmentTreeModel> process2TreeModel(List<DepartmentDto> source, DepartmentTreeModel curRootNode,int did)
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
                    if(did!=dep.DepartmentID)
                    { 
                    DepartmentTreeModel t = new DepartmentTreeModel()
                    {
                        id = dep.DepartmentID,
                        text = dep.Name,
                        children = process2TreeModel(source, dep,did)
                    };
                    curRootNode.children.Add(t);
                    }
                }
                return curRootNode.children;
            }
            return new List<DepartmentTreeModel>();
        }
    }
}
