using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Web.ViewModels;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Web.Framework.ViewModels;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Infrastructure.Model;
using ML.BC.EnterpriseWeb.Areas.ReportForm.Models;
using ML.BC.Services;
using ML.BC.EnterpriseData.Common;
using ML.BC.Web.Framework.Security;
namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Controllers
{
    [AuthorizeCheck]
    public class DepartmentTableController : BCControllerBase
    {
        public ActionResult DepartmentTableIndex()
        {

            return View();
        }
        private IDepartmentInfoFormService service = null;
        public DepartmentTableController()
        {
            service = ML.BC.Infrastructure.Ioc.GetService<IDepartmentInfoFormService>();
        }
        public ActionResult report()
        {
            return View();
        }
        [PermissionControlAttribute(Functions.Root_ReportManagement_DepartmentDataReport_ShowAll)]
        public ActionResult GetList(DateTime? starttime, DateTime? endtime)
        {
            var result = new StandardJsonResult<DepartmentTableResult>();
            result.Try(() =>
            {

                List<DepartmentInfoFormDto> list = new List<DepartmentInfoFormDto>();
                list = service.GetDepartmentInfoFormOfEnterprise(BCSession.User.EnterpriseID, starttime, endtime);
                List<DepartmentTableModel> mylist = new List<DepartmentTableModel>();
                foreach (var a in list)
                {
                    double pspace = a.PictureByte / Math.Pow(1024, 2);
                    mylist.Add(new DepartmentTableModel()
                    {
                        Department = a.DepartmentName,
                        People = a.UsersCount,
                        Picture = a.PictureCount,
                        Project = a.ProjectCount,
                        Scene = a.SceneCount,
                        ParentID = a.ParentID,
                        PictureMb = Convert.ToSingle(pspace.ToString("f2")),
                        id = a.DepartmentID,
                        _parentId = a.ParentID
                    });
                }
                List<DepartmentTableModel> footer = new List<DepartmentTableModel>();
                footer.Add(new DepartmentTableModel()
                {
                    Department = "总计：",
                    People = mylist.Sum(s => s.People),
                    Scene = mylist.Sum(s => s.Scene),
                    Project = mylist.Sum(s => s.Project),
                    Picture = mylist.Sum(s => s.Picture),
                    PictureMb = mylist.Sum(s => s.PictureMb),
                });
                result.Value = new DepartmentTableResult();
                result.Value.total = 10;
                result.Value.rows = mylist;
                result.Value.footer = footer;

            });
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
    }
}
