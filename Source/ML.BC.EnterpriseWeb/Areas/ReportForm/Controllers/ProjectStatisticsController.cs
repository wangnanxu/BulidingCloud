using ML.BC.EnterpriseData.Common;
using ML.BC.EnterpriseWeb.Areas.ReportForm.Models;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Services.Account.Dtos;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Web.Framework.Security;
using System.Collections.Generic;
using System.Web.Mvc;
//项目统计
namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Controllers
{
    [AuthorizeCheck]
    public class ProjectStatisticsController : BCControllerBase
    {
        private IProjectStatisticsService _projStatService = Ioc.GetService<IProjectStatisticsService>();
        private SessionUserDto _user;


        public ProjectStatisticsController()
        {
            _user = GetSession().User;
        }
        public ActionResult ProjStatisticsIndex()
        {
            return View();
        }
        //todo
        [PermissionControlAttribute(Functions.Root_ReportManagement_PersonStatisticsDataReport)]
        public ActionResult GetProjStatData(ProjectStatisticsQueryModel model)
        {
            var result = new StandardJsonResult<ProjectStatisticsResultModel>();
            result.Try(() =>
            {

                if (model.page < 1) model.page = 1;
                if (model.rows < 1) model.rows = 10;
                if (model.ProjectName == null) model.ProjectName = "";
                int amount;
                var r = _projStatService.GetProjectStatisInfo(_user.EnterpriseID, _user.DepartmentID, model.ProjectName, model.rows, model.page, out amount);
                result.Value = new ProjectStatisticsResultModel();
                result.Value.rows = new List<ProjectStatisticsShowModel>();

                result.Value.footer = new List<ProjectStatisticsShowModel>();
                ProjectStatisticsShowModel footer = new ProjectStatisticsShowModel();

                result.Value.total = amount;
                foreach (var dto in r)
                {
                    ProjectStatisticsShowModel sm = (ProjectStatisticsShowModel)dto;
                    result.Value.rows.Add(sm);
                    footer.typeCount1 += sm.typeCount1;
                    footer.typeCount2 += sm.typeCount2;
                    footer.typeCount3 += sm.typeCount3;
                    footer.typeCount4 += sm.typeCount4;
                    footer.typeCount5 += sm.typeCount5;
                    footer.typeCount6 += sm.typeCount6;
                    footer.typeCount7 += sm.typeCount7;
                    footer.typeCountTotal += sm.typeCountTotal;
                }
                footer.projectName = "当前页合计";
                //所有项目合计footer
                int projAmount;
                ProjectStatisticsShowModel footer2 = _projStatService.GetAllProjectStatisInfo(_user.EnterpriseID, _user.DepartmentID, out projAmount);
                footer2.projectName = "所有项目合计";
                result.Value.footer.Add(footer);
                result.Value.footer.Add(footer2);
            });
            return result;
        }

    }
}
