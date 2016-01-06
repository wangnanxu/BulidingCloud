using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using ML.BC.EnterpriseWeb.Areas.APIs.Models;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Web.Framework;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Web.Framework.Security;

namespace ML.BC.EnterpriseWeb.Areas.APIs.Controllers
{
    [AuthorizeCheck]
    public class KnowlegeBaseController : APIControllerBase
    {
        private log4net.ILog _ilog = log4net.LogManager.GetLogger("LOG");
        private IKnowledgaeList service;

        public KnowlegeBaseController()
        {
            service = Ioc.GetService<IKnowledgaeList>();
        }

        [AllowCrossDomainPost]
        public ActionResult GetKnowlegeList(KnowlegeModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "GetKnowledgeList", Serializer.ToJson(model)));

            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var knowlegelist = service.GetAllKnowlege(BCSession.User.EnterpriseID);

                result.Value = new { KnowlegeList = knowlegelist };
            });
            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "GetKnowledgeList", Serializer.ToJson(result)));
            return result;
        }

        [AllowCrossDomainPost]
        public ActionResult DownLoadKnowlegeList(KnowlegeModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "DownLoadKnowlegeList", Serializer.ToJson(model)));

            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var knowlege = service.MakeUrlWithFileName(model.FileName);

                result.Value = new { FileUrl = knowlege };
            });
            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "DownLoadKnowlegeList", Serializer.ToJson(result)));
            return result;
        }
    }
}
