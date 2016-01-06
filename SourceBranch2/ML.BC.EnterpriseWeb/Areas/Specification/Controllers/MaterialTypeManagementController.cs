using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Services.Account.Dtos;
using ML.BC.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ML.BC.EnterpriseWeb.Areas.Unit.Models;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Web.Framework;
using ML.BC.Web.Framework.Security;
using ML.BC.EnterpriseData.Common;
namespace ML.BC.EnterpriseWeb.Areas.Specification.Controllers
{
    [AuthorizeCheck]
    public class MaterialTypeManagementController : BCControllerBase
    {
        public ActionResult MaterialTypeIndex()
        {
            return View();
        }

        [PermissionControlAttribute(Functions.Root_DataManagement_DataTypeManagement_List)]
        public ActionResult GetMaterialList(string Name, int rows, int page)
        {
            var result = new StandardJsonResult<MaterialTypeResult>();
            result.Try(() =>
            {
                var mservice = ML.BC.Infrastructure.Ioc.GetService<IMaterialTypeManagementService>();
                result.Value = new MaterialTypeResult();
                int amount = 0;
                result.Value.rows = mservice.GetAllMaterialType(Name, rows, page, out amount);
                result.Value.total = amount;
            });
            if (!result.Success)
            {
                result.Value = new MaterialTypeResult();
            }
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }

        [PermissionControlAttribute(Functions.Root_DataManagement_DataTypeManagement_Add)]
        public ActionResult AddMaterialType(MaterialTypeModel model)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var mservice = ML.BC.Infrastructure.Ioc.GetService<IMaterialTypeManagementService>();
                MaterialTypeDto m = new MaterialTypeDto();
                m.Avaliable = model.Avaliable;
                m.MaterialTypeID = model.MaterialTypeID;
                m.Name = model.Name;
                mservice.Add(m);

            });
            if (result.Success)
            {
                result.Value = true;
                result.Message = "添加成功";
            }
            return result;
        }

        [PermissionControlAttribute(Functions.Root_DataManagement_DataTypeManagement_Edit)]
        public ActionResult UpdateMaterialType(MaterialTypeModel model)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                MaterialTypeDto m = new MaterialTypeDto();
                m.Name = model.Name;
                m.MaterialTypeID = model.MaterialTypeID;
                m.Avaliable = model.Avaliable;
                var mservice = ML.BC.Infrastructure.Ioc.GetService<IMaterialTypeManagementService>();
                result.Value = mservice.Update(m);
            });
            if (result.Success)
            {

                result.Message = "修改成功";
            }
            return result;
        }

        [PermissionControlAttribute(Functions.Root_DataManagement_DataTypeManagement_Delete)]
        public ActionResult DeleteMaterialType(int MaterialTypeID)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                var mservice = ML.BC.Infrastructure.Ioc.GetService<IMaterialTypeManagementService>();
                result.Value = mservice.Delete(MaterialTypeID);
            });
            if (result.Success)
            {

                result.Message = "删除成功";
            }
            return result;
        }

    }
}
