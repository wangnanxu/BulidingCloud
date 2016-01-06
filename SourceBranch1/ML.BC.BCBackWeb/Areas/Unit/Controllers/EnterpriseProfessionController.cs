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
using ML.BC.BCBackWeb.Model;
using ML.BC.Web.Framework;
using ML.BC.Web.Framework.Security;
using ML.BC.BCBackData.Common;
namespace ML.BC.BCBackWeb.Areas.Unit.Controllers
{
    /// <summary>
    /// 行业性质
    /// </summary>
    /// 
    [Authorize]
    public class EnterpriseProfessionController : BCControllerBase
    {
        //
        // GET: /Account/EnterpriseController/
        IEnterpriseProfessionManagementService service;
        public EnterpriseProfessionController()
        {
            service = Ioc.GetService<IEnterpriseProfessionManagementService>();
        }
         [PermissionControlAttribute(Functions.Root_EPManagement_EPProfession)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取列表信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
           [PermissionControlAttribute(Functions.Root_EPManagement_EPProfession)]
        public ActionResult GetList(string Name,int rows, int page)
        {
            var result = new StandardJsonResult<EnterpriseProfessionResultModel>();

            result.Try(() => {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                if (page < 1)
                    page = 1;
                if (rows < 10)
                    rows = 10;
                 int amount;
                var list = service.GetEnterpriseProfessionList(Name,rows,page,out amount);
                var viewModel = new EnterpriseProfessionResultModel();
                viewModel.total = amount;
                viewModel.rows = list;

                result.Value = viewModel;
            });
            if (!result.Success)
            {
                result.Value = new EnterpriseProfessionResultModel();
            }
            return new OringinalJsonResult<EnterpriseProfessionResultModel> { Value = result.Value };
        }
        /// <summary>
        /// 获取简单的行业;下拉框用的
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult GetEnterpriseProfessionSimpleList()
        {
            var result = new StandardJsonResult<List<SimpleModel>>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var list = service.GetAllEnterpriseProfession();
                if (list != null && list.Count > 0)
                {
                    var resultlist = list.Select(m => new SimpleModel { Id = m.EnterpriseProfessionID, Text = m.Name });
                    result.Value = resultlist.ToList();
                
                }
                else
                {
                    result.Value = new List<SimpleModel>();
                   
                }
            });
            return new OringinalJsonResult<List<SimpleModel>> { Value = result.Value };
        }

        /// <summary>
        ///删除
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
           [PermissionControlAttribute(Functions.Root_EPManagement_EPProfession_Delete)]
        public ActionResult Delete(string EnterpriseProfessionID)
        {
            var result = new StandardJsonResult();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var service = Ioc.GetService<IEnterpriseProfessionManagementService>();
                bool del = service.DeleteEnterpriseProfession(EnterpriseProfessionID);
                result.Message = "系统提示:删除成功";
            });
               if(!result.Success)
                   result.Message = "系统提示:删除失败";
            return result;
        }
        /// <summary>
        ///新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
           [PermissionControlAttribute(Functions.Root_EPManagement_EPProfession_Add)]
        public ActionResult Add(EnterpriseProfessionModel model)
        {
            var result = new StandardJsonResult();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var service = Ioc.GetService<IEnterpriseProfessionManagementService>();
                var ad = new EnterpriseProfessionDto();
                ad.Available = model.Available;
                ad.EnterpriseProfessionID = model.EnterpriseProfessionID;
                ad.Name = model.Name;
                service.AddEnterpriseProfession(model.EnterpriseProfessionID,model.Name,model.Available);
                result.Message = "系统提示:添加成功！";
            });
           
            return result;
        }

        /// <summary>
        ///更改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
           [PermissionControlAttribute(Functions.Root_EPManagement_EPProfession_Edit)]
        public ActionResult Update(EnterpriseProfessionModel model)
        {
            var result = new StandardJsonResult();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var service = Ioc.GetService<IEnterpriseProfessionManagementService>();
                var ad1 = new EnterpriseProfessionDto();
                ad1.Available = model.Available;
                ad1.EnterpriseProfessionID = model.EnterpriseProfessionID;
                ad1.Name = model.Name;
                service.UpdateEnterpriseProfession(model.EnterpriseProfessionID,model.Name,model.Available);
                result.Message = "系统提示:修改成功！";
            });
               if(!result.Success)
                   result.Message = "系统提示:修改成功！";
            return result;
        }

    }
}
