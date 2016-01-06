using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Infrastructure;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Services;
using ML.BC.BCBackWeb.Areas.Unit.Models;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Services.Enterprise.Dtos;
using ML.BC.Services.Enterprise;
using ML.BC.Web.Framework;
using ML.BC.BCBackWeb.Model;
using ML.BC.Infrastructure.Model;
using ML.BC.Web.Framework.Security;
using ML.BC.BCBackData.Common;
namespace ML.BC.BCBackWeb.Areas.Unit.Controllers
{
    [Authorize]
    public class EnterpriseManagementController : BCControllerBase
    {
        private IEnterpriseManagementService _ieIEnterprise;
        public EnterpriseManagementController()
        {
            _ieIEnterprise = Ioc.GetService<IEnterpriseManagementService>();
        }
        [PermissionControlAttribute(Functions.Root_EPManagement_EPList)]
        public ActionResult EnterpriseIndex()
        {
            return View();
        }
        [PermissionControlAttribute(Functions.Root_EPManagement_EPList)]
        public ActionResult GetEnterpriseList(EnterpriseViewModel model)
        {
            var result = new StandardJsonResult<DataGridResultModelBase<EnterpriseDto>>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                int count = 100;//test
                List<EnterpriseDto> list;//= new List<EnterpriseDto>();
                list = _ieIEnterprise.SearchEnterpriseByCondition(model.ProfessionID, model.PropertyID, model.Name, model.rows, model.page, out count);
                result.Value = new DataGridResultModelBase<EnterpriseDto>();
                result.Value.total = count;//赋值
                result.Value.rows = list;
                //Serializer.ToJson(list);
            });
            return new OringinalJsonResult<DataGridResultModelBase<EnterpriseDto>> { Value = result.Value };
        }
        /// <summary>
        /// 获取简单的企业列表;下拉框用的
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult GetEnterpriseBySimpleList()
        {
            var result = new StandardJsonResult<List<SimpleModel>>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var list = _ieIEnterprise.GetAllEnterpriseList();
                if (list != null && list.Count > 0)
                {
                    var resultlist = list.Select(m => new SimpleModel { Id = m.EnterpriseID, Text = m.Name });
                    result.Value = resultlist.ToList();
                }
                else
                {
                    result.Value = new List<SimpleModel>();
                }
            });
            // return result;
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ///
        [PermissionControlAttribute(Functions.Root_EPManagement_EPList_Delete)]
        public ActionResult DeleteEnterprise(string EnterpriseID)
        {
            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }


                bool f = _ieIEnterprise.DeleteEnterprise(EnterpriseID);
                result.Value = f.ToString();
            });
            return result;
        }
        /// <summary>
        ///新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [PermissionControlAttribute(Functions.Root_EPManagement_EPList_Add)]
        public ActionResult AddEnterprise(EnterpriseDto model)
        {
            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                model.Deleted = false; model.RegistDate = DateTime.Now;
                model.UpdateTime = DateTime.Now;
                model.RegistDate = DateTime.Now;
                string value = _ieIEnterprise.AddEnterprise(model);
                result.Value = value;
            });
            return result;
        }

        /// <summary>
        ///更改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        [PermissionControlAttribute(Functions.Root_EPManagement_EPList_Edit)]
        public ActionResult UpdateEnterprise(EnterpriseDto model)
        {
            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                model.UpdateTime = DateTime.Now;
                bool f = _ieIEnterprise.UpdateEnterprise(model);
                result.Value = f.ToString();
            });
            return result;
        }

    }
}
