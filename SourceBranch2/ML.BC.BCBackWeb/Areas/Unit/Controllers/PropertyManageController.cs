using ML.BC.BCBackWeb.Areas.Unit.Models;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Services.Unit.Dtos;
using ML.BC.Web.Framework;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using ML.BC.Web.Framework.Security;
using ML.BC.BCBackData.Common;
using ML.BC.Web.Framework.ViewModels;

namespace ML.BC.BCBackWeb.Areas.Unit.Controllers
{
    //企业性质管理控制器
    [Authorize]
    public class PropertyManageController : Controller
    {
        private IEnterprisePropertyManagementService _service;
        public PropertyManageController()
        {
            _service = ML.BC.Infrastructure.Ioc.GetService<IEnterprisePropertyManagementService>();
        }

        [PermissionControlAttribute(Functions.Root_EPManagement_EPProperty)]
        public ActionResult Index()
        {
            return View();
        }

        [PermissionControlAttribute(Functions.Root_EPManagement_EPProperty)]
        public ActionResult GetList(PropertyViewModel model)
        {
            var result = new StandardJsonResult<PropResultModel>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                int page = model.page < 1 ? 1 : model.page;
                int pageSize = model.rows < 1 ? 15 : model.rows;

                int total;
                var list = _service.SearchEnterprisePropertyByName("", pageSize, page, out total);
                result.Value = new PropResultModel();
                result.Value.rows = new List<EnterprisePropertyDto>();
                result.Value.rows = list;
                result.Value.total = total;

            });
            if (result.Success == false) result.Value = new PropResultModel();
            return new OringinalJsonResult<PropResultModel> { Value = result.Value };
        }

        [PermissionControlAttribute(Functions.Root_EPManagement_EPProperty)]
        public ActionResult Search(string keyword)
        {
            var result = new StandardJsonResult<GridDataModelBase<EnterprisePropertyDto>>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                int amount;
                var list = _service.SearchEnterprisePropertyByName(keyword, 1, 30, out amount);
                result.Value = new GridDataModelBase<EnterprisePropertyDto> { rows = list, total = amount };
            });
            if (result.Success == false) result.Value = new GridDataModelBase<EnterprisePropertyDto>();
            return new OringinalJsonResult<GridDataModelBase<EnterprisePropertyDto>> { Value = result.Value };
        }
        [PermissionControlAttribute(Functions.Root_EPManagement_EPProperty_Add)]
        public ActionResult AddProp(PropertyNewModel model)
        {
            //NewUserModel tempModel = Serializer.FromJson<NewUserModel>(jsonstr);
            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var dto = new EnterprisePropertyDto { EnterprisePropertyID = model.EnterprisePropertyID, Name = model.Name, Available = model.Available };

                string uid = _service.AddEnterpriseProperty(dto);
                result.Value = uid;

            });
            return result;

        }

        [PermissionControlAttribute(Functions.Root_EPManagement_EPProperty_Edit)]
        public ActionResult UpdateProp(PropertyNewModel model)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var dto = new EnterprisePropertyDto { EnterprisePropertyID = model.EnterprisePropertyID, Name = model.Name, Available = model.Available };

                bool value = _service.UpdateEntProperty(dto);
                result.Value = value.ToString();
            });
            return result;
        }
        [PermissionControlAttribute(Functions.Root_EPManagement_EPProperty_Delete)]
        public ActionResult DeleteProp(string propID)
        {
            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var value = _service.DeleteEnterpriseProperty(propID);
                result.Value = value.ToString();
            });
            return result;
        }

        /// <summary>
        /// 获取简单的性质;下拉框用的
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult GetEnterprisePropertySimpleList()
        {
            var result = new StandardJsonResult<List<SimpleModel>>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var list = _service.GetAllEnterpriseProperty();
                if (list != null && list.Count > 0)
                {
                    var resultlist = list.Select(m => new SimpleModel { Id = m.EnterprisePropertyID, Text = m.Name });
                    result.Value = resultlist.ToList();
                }
                else
                {
                    result.Value = new List<SimpleModel>();
                }
            });
            if (result.Success == false) result.Value = new List<SimpleModel>();
            return new OringinalJsonResult<List<SimpleModel>> { Value = result.Value };
        }


    }


}

