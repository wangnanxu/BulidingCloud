using ML.BC.EnterpriseWeb.Areas.ReportForm.Models;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Web.Framework.Security;
using ML.BC.EnterpriseData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Controllers
{
    [AuthorizeCheck]
    public class PersonStatisticsController : BCControllerBase
    {
        IPersonStatisticsService _personServie = Ioc.GetService<IPersonStatisticsService>();

        public ActionResult PersonStatisIndex()
        {
            return View();
        }
        [PermissionControlAttribute(Functions.Root_ReportManagement_PersonStatisticsDataReport_ShowAll)]
        public ActionResult GetPersonStatisData(PersonStatisGetModel model)
        {
            if (model.page < 1) model.page = 1;
            if (model.rows < 1) model.rows = 10;
            if (model.startTime == null) model.startTime = DateTime.MinValue;
            if (model.endTime == null) model.endTime = DateTime.MaxValue;
            if (model.UserName == null) model.UserName = "";
            var result = new StandardJsonResult<PersonStatisResultModel>();
            int amount;
            result.Try(() =>
            {
                result.Value = new PersonStatisResultModel();
                string _enterpriseID = BCSession.User.EnterpriseID;
                result.Value.rows = _personServie.GetPersonStatisInfo(_enterpriseID, model.UserName, model.startTime.Value, model.endTime.Value, model.rows, model.page, out amount);
                result.Value.total = amount;
                var summary = _personServie.GetPersonStatisSummaryInfo(_enterpriseID);
                result.Value.footer = new List<PersonStatisDto>();
                result.Value.footer.Add(summary);
               
            });
            return result;
        }
    }
}
