using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Services.Common;
using ML.BC.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.BCBackWeb.Areas.Unit.Models;
using ML.BC.BCBackWeb.Model;
using ML.BC.Web.Framework.Security;
using ML.BC.BCBackData.Common;

namespace ML.BC.BCBackWeb.Areas.Unit.Controllers
{
    [Authorize]
    public class BackLogManagementController : BCControllerBase
    {
        private IOperationLogService _logService = Ioc.GetService<IOperationLogService>();
        private IFrontUserManagementService _userService = Ioc.GetService<IFrontUserManagementService>();
        private IEnterpriseManagementService _entService = Ioc.GetService<IEnterpriseManagementService>();
        public BackLogManagementController()
        {

        }

        public ActionResult BackLogIndex()
        {
            return View();
        }

        //获取列表及搜索
        [PermissionControlAttribute(Functions.Root_EPManagement_EPLog)]
        public ActionResult GetLogList(BackLogGetModel model)
        {
            var result = new StandardJsonResult<BackLogViewResultModel>();
            result.Try(() =>
            {

                if (model.page < 1) model.page = 1;
                if (model.rows < 1) model.rows = 10;
                SearchLogConditionDto cond;
                if (model.EnterpriseId == null || model.EnterpriseId.Equals(""))
                {
                    cond = new SearchLogConditionDto
                    {
                        UserID = model.UserID,
                        EnterpriseID = "",
                        EndTime = model.EndTime,
                        StartTime = model.StartTime,
                        OperationID = model.OperationID,
                        UserName = model.UserName
                    };
                }
                else
                {
                    cond = new SearchLogConditionDto
                    {
                        UserID = model.UserID,
                        EnterpriseID = model.EnterpriseId,
                        EndTime = model.EndTime,
                        StartTime = model.StartTime,
                        OperationID = model.OperationID,
                        UserName = model.UserName
                    };
                }
                int amount;
                var list = _logService.SearchLogsByCondition(cond, model.rows, model.page, out amount);
                var rlist = new List<BackLogViewModel>();
                foreach (var log in list)
                {
                    var vlog = (BackLogViewModel)log;
                    try
                    {
                        if (log.EnterpriseID == null)
                        {
                            var user = _userService.GetFrontUserByUserID(log.UserID);
                            if (user != null && user.EnterpiseID != null) 
                            {                                 
                                vlog.EnterpriseName = _entService.GetOneByEnterpriseID(user.EnterpiseID).Name;
                            }
                            else
                            {
                                vlog.EnterpriseName = "无企业ID和用户所属企业信息";
                            }
                        }
                        else
                        {
                            vlog.EnterpriseName = _entService.GetOneByEnterpriseID(log.EnterpriseID).Name;
                        }
                        
                    }
                    catch (Exception )
                    {
                        
                        vlog.EnterpriseName = "(" + log.EnterpriseID + ")企业名获取失败";
                    }
                    rlist.Add(vlog);
                }
                result.Value = new BackLogViewResultModel();
                result.Value.total = amount;
                result.Value.rows = rlist;
            });

            if (result.Success == true)
            {
                return new OringinalJsonResult<BackLogViewResultModel> { Value = result.Value };
            }
            else
            {
                return new OringinalJsonResult<BackLogViewResultModel> { Value = new BackLogViewResultModel() };
            }
        }
        public ActionResult GetEnterpriseList()
        {
            var result = new StandardJsonResult<List<SimpleModel>>();
            result.Try(() =>
            {
                result.Value = new List<SimpleModel>();
                try
                {
                    result.Value = _entService.GetAllEnterpriseList().Select(e => new SimpleModel
                    {
                        Id = e.EnterpriseID,
                        Text = e.Name
                    }).ToList();
                }
                catch (Exception )
                {
                    result.Value = new List<SimpleModel>();
                }
            });
            return new OringinalJsonResult<List<SimpleModel>> { Value = result.Value };
        }
        //清空日志
        [PermissionControlAttribute(Functions.Root_EPManagement_EPLog_ClearLogs)]
        public ActionResult ClearLog()
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                result.Value = _logService.ClearLog();
            });
            return result;
        }




    }
}
