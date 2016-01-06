using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Services.Account.Dtos;
using ML.BC.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using ML.BC.Web.Framework.Security;
using ML.BC.EnterpriseData.Common;
using ML.BC.Web.Framework.ViewModels;
using ML.BC.EnterpriseWeb.Areas.Account.Models;
using ML.BC.Infrastructure.Exceptions;


namespace ML.BC.EnterpriseWeb.Areas.Account.Controllers
{
    [AuthorizeCheck]
    public class EnterpriseFunctionManagementController : BCControllerBase
    {
        private IEnterpriseFunctionManagementService _functionManagementService;
        // private FunctreeHelper _helper = new FunctreeHelper();

        public EnterpriseFunctionManagementController()
        {
            _functionManagementService = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseFunctionManagementService>();
        }

        //初始化页面
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult EntFunctionTree()
        {
            return View();
        }


        private bool HaveFuncction(FunctionDto fun, IEnumerable<string> auths)
        {
            bool f = false;
            if (fun == null || auths == null || auths.Count() == 0) return f;
            var list = auths.Where(m => m == fun.FunctionID);
            var model = list.FirstOrDefault();
            if (model != null)
            {
                if (!string.IsNullOrEmpty(model)) f = true;
            }
            return f;
        }
        //IEnumerable<FunctionDto> AllFunctionDto = null;
        bool isfirst = true;
        private IEnumerable<TreeDataModelBase> FunctionDtoToTreeResultModelBase(IEnumerable<FunctionDto> source, FunctionDto parentModel)
        {

            List<TreeDataModelBase> resultlist = new List<TreeDataModelBase>();
            if (source != null && source.Count() > 0)
            {
                if (parentModel == null)
                {
                    return resultlist;
                }

                IEnumerable<FunctionDto> sublist = source.Where(m => m.ParentID == parentModel.FunctionID);
                // var pmodel=sublist.FirstOrDefault();
                if (isfirst)
                {
                    isfirst = false;
                    sublist = source.Where(m => m.ParentID == "");
                }
                foreach (FunctionDto fun in sublist)
                {
                    resultlist.Add(new TreeDataModelBase()
                    {
                        id = fun.FunctionID,
                        text = fun.Name,
                        state = "open",
                        iconCls = "",
                        @checked = false,
                        children = FunctionDtoToTreeResultModelBase(source, fun)
                    });
                }
            }
            return resultlist;
        }
        private IEnumerable<TreeDataModelBase> FunctionDtoToTreeResultModelBase(IEnumerable<FunctionDto> source, IEnumerable<string> auths, FunctionDto parentModel)
        {

            List<TreeDataModelBase> resultlist = new List<TreeDataModelBase>();
            if (source != null && source.Count() > 0)
            {
                if (parentModel == null)
                {
                    return resultlist;
                }

                IEnumerable<FunctionDto> sublist = source.Where(m => m.ParentID == parentModel.FunctionID);
                // var pmodel=sublist.FirstOrDefault();
                if (isfirst)
                {
                    isfirst = false;
                    sublist = source.Where(m => m.ParentID == "");
                }
                foreach (FunctionDto fun in sublist)
                {
                    resultlist.Add(new TreeDataModelBase()
                    {
                        id = fun.FunctionID,
                        text = fun.Name,
                        state = "open",
                        iconCls = "",
                        @checked = HaveFuncction(fun, auths),
                        children = FunctionDtoToTreeResultModelBase(source, fun)
                    });
                }
            }
            return resultlist;
        }
        //获取所有功能列表并转换成需要tree的json格式
        //[PermissionControlAttribute(Functions.Root_SysManagement_EPFunMangement)]
        public ActionResult GetFunclistByTreeJson(int? RoleID)
        {
            var result = new StandardJsonResult<List<TreeDataModelBase>>();
            result.Try(() =>
            {
                string UserID = GetSession().User.UserID;
                List<FunctionDto> list = _functionManagementService.GetFunctionsByFunctionIds(GetSession().User.FunctionIDs);
                var rootModel = list.Where(m => m.ParentID == "" || m.FunctionID.ToLower() == "root").FirstOrDefault();
                //if (!_functionManagementService.ExistFunction("Root"))   { } 
                if (list == null) throw new ArgumentNullException("func list get from service is null!");
                List<TreeDataModelBase> listjson = new List<TreeDataModelBase>();

                if (rootModel == null || string.IsNullOrEmpty(rootModel.FunctionID))
                {
                    //没有根节点
                    throw new KnownException("无权限");
                }
                if (TryParse.intTryParse(RoleID) <= 0)
                {
                    //新增角色的时候
                    listjson = FunctionDtoToTreeResultModelBase(list, rootModel).ToList();
                }
                else
                {
                    var IRoleFunctionMG = Ioc.GetService<IRoleFunctionManagementService>();
                    List<string> auths = IRoleFunctionMG.GetAllFunctionIDByRoleID((int)RoleID);
                    listjson = FunctionDtoToTreeResultModelBase(list, auths, rootModel).ToList();
                }
                result.Value = listjson;
            });
            if (result.Success == false) { result.Value = new List<TreeDataModelBase>(); }
            return new OringinalJsonResult<List<TreeDataModelBase>> { Value = result.Value };
        }

    }
}
