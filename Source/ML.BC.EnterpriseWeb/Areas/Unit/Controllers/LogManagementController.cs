using System.Web.Mvc;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Services.Common;
using ML.BC.EnterpriseWeb.Areas.Unit.Models;
using System.Collections.Generic;
using ML.BC.Services;
using ML.BC.Web.Framework.Security;
using ML.BC.EnterpriseData.Common;

namespace ML.BC.EnterpriseWeb.Areas.Unit.Controllers
{
    [AuthorizeCheck]
    public class LogManagementController : BCControllerBase
    {
        private IOperationLogService _logService = Ioc.GetService<IOperationLogService>();
        private IFrontUserManagementService _userService = Ioc.GetService<IFrontUserManagementService>();
        private string _enterpriseID;
        public LogManagementController()
        {
            _enterpriseID = GetSession().User.EnterpriseID;
        }

        public ActionResult Index()
        {
            return View();
        }

        //获取列表及搜索
        [PermissionControlAttribute(Functions.Root_SystemSetting_OperationLogManagement_List)]
        public ActionResult GetLogList(LogGetModel model)
        {
            var result = new StandardJsonResult<LogViewResultModel>();
            result.Try(() =>
            {

                if (model.page < 1) model.page = 1;
                if (model.rows < 1) model.rows = 10;
                var cond = new SearchLogConditionDto
                {
                    UserID = model.UserID,
                    EnterpriseID = _enterpriseID,
                    EndTime = model.EndTime,
                    StartTime = model.StartTime,
                    OperationID = model.OperationID,
                    UserName = model.UserName,
                };
                int amount;
                var list = _logService.SearchLogsByCondition(cond, model.rows, model.page, out amount);
                var rlist = new List<LogViewModel>();
                foreach (var log in list)
                {
                    var vlog = (LogViewModel)log;
                    //vlog.UserName = _userService.GetFrontUserByUserID(log.UserID).Name;
                    rlist.Add(vlog);
                }
                result.Value = new LogViewResultModel();
                result.Value.total = amount;
                result.Value.rows = rlist;
            });

            

                return new OringinalJsonResult<LogViewResultModel> { Value = result.Value };
           
        }
        //清空日志
        [PermissionControlAttribute(Functions.Root_SystemSetting_OperationLogManagement_Delete)]
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
