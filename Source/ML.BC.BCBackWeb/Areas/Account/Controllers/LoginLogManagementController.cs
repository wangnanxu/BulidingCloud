using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ML.BC.BCBackWeb.Areas.Account.Models;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Web.Framework.Controllers;
using ML.BC.BCBackWeb.Model;
using ML.BC.Web.Framework.Security;
using ML.BC.BCBackData.Common;
using ML.BC.EnterpriseData.Common;

namespace ML.BC.BCBackWeb.Areas.Account.Controllers
{
    [Authorize]
    public class LoginLogManagementController : BCControllerBase
    {
        private IEnterpriseManagementService _entService = Ioc.GetService<IEnterpriseManagementService>();
        private IAccountService _logService = Ioc.GetService<IAccountService>();

        public ActionResult LoginLogIndex()
        {
            return View();
        }

        //获取列表及搜索
        [PermissionControlAttribute(ML.BC.BCBackData.Common.Functions.Root_EPManagement_LoginLog)]
        public ActionResult GetLogList(LoginLogQueryModel model)
        {
            var result = new StandardJsonResult<ResultModel>();
            result.Try(() =>
            {
                LoginStatus _status;
                if (model.Status == null)
                {
                    _status = LoginStatus.GetAll;
                }
                else
                {
                    _status = (LoginStatus)model.Status;
                }
                if (model.rows < 1) model.rows = 10;
                if (model.page < 1) model.page = 1;
                result.Value = new ResultModel();
                result.Value.rows = new List<LoginLogShowModel>();
                int amount;
                List<UserLoginLogDto> tempList;
                try
                {
                    tempList = _logService.SearchUserLoginLog(model.BeginTime, model.EndTime, model.UserName ?? "", model.EnterpriseName ?? "", _status, model.page, model.rows, out amount);
                }
                catch (Exception e)
                {
                    //tempList = new List<UserLoginLogDto>();
                    //result.Fail("Service发生异常:" + e.Message ?? "" + ".");
                    throw e;
                }
                result.Value.total = amount;

                foreach (var dto in tempList)
                {
                    var r = (LoginLogShowModel)dto;
                    result.Value.rows.Add(r);
                }
                //result.Value.rows.OrderBy(l => l.Time).Reverse();
            });


            return result;

        }
        //public ActionResult GetEnterpriseList()
        //{
        //    var result = new StandardJsonResult<List<SimpleModel>>();
        //    result.Try(() =>
        //    {
        //        result.Value = new List<SimpleModel>();
        //        try
        //        {
        //            result.Value = _entService.GetAllEnterpriseList().Select(e => new SimpleModel
        //            {
        //                Id = e.EnterpriseID,
        //                Text = e.Name
        //            }).ToList();
        //        }
        //        catch (Exception)
        //        {
        //            result.Value = new List<SimpleModel>();
        //        }
        //    });

        //}
        
    }
}
