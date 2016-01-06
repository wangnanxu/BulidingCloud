using ML.BC.BCBackWeb.Areas.Account.Models;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Services.Account.Dtos;
using ML.BC.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ML.BC.BCBackWeb.Model;
using ML.BC.Web.Framework.Security;
using ML.BC.BCBackData.Common;


namespace ML.BC.BCBackWeb.Areas.Account.Controllers
{
    [Authorize]
    public class FunctionManagementController : BCControllerBase
    {
        private IFunctionManagementService _functionManagementService;
        private FunctreeHelper _helper = new FunctreeHelper();

        public FunctionManagementController()
        {
            _functionManagementService = ML.BC.Infrastructure.Ioc.GetService<IFunctionManagementService>();
        }

        //初始化页面
        [PermissionControlAttribute(Functions.Root_SysManagement_SysFunManagement)]
        public ActionResult FunctionTree()
        {
            return View();
        }



        //获取所有功能列表并转换成需要的json格式
        [PermissionControlAttribute(Functions.Root_SysManagement_SysFunManagement)]
        public ActionResult GetFunclistByJson()
        {
            //分页查询所需参数 暂不分页
            //int page = fvmodel.page;
            //int pagesz = fvmodel.rows;
            //string name = fvmodel.name; 


            //初始化 添加根
            if (!_functionManagementService.ExistFunction("Root"))
            {
                var Root = new FunctionDto
                {

                    ParentID = "",
                    Name = "Root",
                    MyID = "",
                    Available = true,
                    Desription = "这是功能表的根，不能编辑",
                    FunctionID = "Root"

                };
                _functionManagementService.Add(Root);

            }
            //不分页查询
            List<FunctionDto> list = _functionManagementService.GetAllFunctions();

            if (!_functionManagementService.ExistFunction("Root")) { throw new ArgumentNullException("add Root fail"); }
            if (list == null) throw new ArgumentNullException("func list get from service is null!");

            List<JsonItem> listjson = new List<JsonItem>();
            foreach (var fun in list)
            {
                var jfun = new JsonItem();
                jfun.Available = fun.Available;
                jfun.Desription = fun.Desription;
                jfun.FunctionID = fun.FunctionID;
                jfun.MyID = fun.MyID;
                jfun.Name = fun.Name;
                jfun.text = fun.Name;
                jfun.ParentID = fun.ParentID;
                jfun.id = _helper.processSID(fun.FunctionID);
                jfun._parentId = _helper.processSID(fun.ParentID);
                // if ( fun.FunctionID.Count(chara=>chara.Equals('.') )>=2 )  {jfun.state = "closed"; }
                listjson.Add(jfun);
            }

            int size = listjson.Count();
            var result = new JsonResults();
            result.total = _functionManagementService.GetAmount();
            result.rows = listjson;
            //var resStr = Serializer.ToJson(result);
            return new OringinalJsonResult<JsonResults> { Value = result };
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
        private IEnumerable<TreeResultModelBase> FunctionDtoToTreeResultModelBase(IEnumerable<FunctionDto> source, FunctionDto parentModel)
        {

            List<TreeResultModelBase> resultlist = new List<TreeResultModelBase>();
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
                    resultlist.Add(new TreeResultModelBase()
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
        private IEnumerable<TreeResultModelBase> FunctionDtoToTreeResultModelBase(IEnumerable<FunctionDto> source, IEnumerable<string> auths, FunctionDto parentModel)
        {

            List<TreeResultModelBase> resultlist = new List<TreeResultModelBase>();
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
                    resultlist.Add(new TreeResultModelBase()
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
        public ActionResult GetFunclistByTreeJson(int RoleID = 0)
        {


            //不分页查询
            List<FunctionDto> list = _functionManagementService.GetAllFunctions();
            var rootModel = list.Where(m => m.ParentID == "" || m.FunctionID.ToLower() == "root").FirstOrDefault();
            if (!_functionManagementService.ExistFunction("Root")) { throw new ArgumentNullException("add Root fail"); }
            if (list == null) throw new ArgumentNullException("func list get from service is null!");

            List<TreeResultModelBase> listjson = new List<TreeResultModelBase>();
            if (RoleID <= 0)
            {
                listjson = FunctionDtoToTreeResultModelBase(list, rootModel).ToList();
            }
            else
            {
                var IRoleFunctionMG = Ioc.GetService<IRoleFunctionManagementService>();
                List<string> auths = IRoleFunctionMG.GetAllFunctionIDByRoleID(RoleID);
                listjson = FunctionDtoToTreeResultModelBase(list, auths, rootModel).ToList();
            }

            //if (list != null && list.Count() > 0)
            //{
            //    foreach (FunctionDto fun in list)
            //    {
            //        listjson.Add(new TreeResultModelBase() {
            //            id = fun.FunctionID,
            //            text = fun.Name,
            //            state = "open",
            //            iconCls = "",
            //            ischecked = false,//??
            //            children = FunctionDtoToTreeResultModelBase(list.Where(m => m.FunctionID == fun.ParentID))
            //        });
            //    }
            //}
            //var resStr = Serializer.ToJson(result);
            return new OringinalJsonResult<List<TreeResultModelBase>> { Value = listjson };
        }

        [PermissionControlAttribute(Functions.Root_SysManagement_SysFunManagement_EditFun)]
        public StandardJsonResult UpdateFunc(FunctionDto model)
        {
            var res = new StandardJsonResult<int>();
            res.Try(() =>
            {

                if (model.FunctionID == null || model.FunctionID.Equals(""))
                {
                    res.Success = false;
                    res.Message = "参数不合法";
                    res.Value = -1;
                }
                else
                {
                    res.Value = _functionManagementService.Update(model);
                }
            });
            return res;


        }

        [PermissionControlAttribute(Functions.Root_SysManagement_SysFunManagement_DeleteFun)]
        public StandardJsonResult DeleteFunc(string funcJsonStr)
        {
            var res = new StandardJsonResult<bool>();
            res.Try(() =>
            {
                var model = Serializer.FromJson<FunctionViewModel>(funcJsonStr);
                if (model.FunctionID == null || model.FunctionID.Equals(""))
                {
                    res.Value = false;
                    res.Success = false;
                    res.Message = "参数不合法,id为空";
                }
                else
                {
                    res.Value = _functionManagementService.Delete(model.FunctionID);
                }
            });
            return res;
        }

        [PermissionControlAttribute(Functions.Root_SysManagement_SysFunManagement_AddFun)]
        public StandardJsonResult AddFunc(FunctionDto model)
        {

            var res = new StandardJsonResult<int>();
            res.Try(() =>
            {
                if (model.MyID == null || model.Name == null || model.ParentID == null)
                {
                    res.Success = false;
                    res.Message = "传递到服务器的参数不合法";
                    res.Value = -1;
                }
                else
                {
                    model.MyID = model.MyID.Replace(" ", "");
                    model.FunctionID = model.ParentID + "." + model.MyID;
                    res.Value = _functionManagementService.Add(model);
                }
            });
            return res;
        }
    }
}
