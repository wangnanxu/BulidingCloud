using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.EnterpriseWeb.Areas.APIs.Models;
using ML.BC.Services;
using ML.BC.Infrastructure;
using ML.BC.EnterpriseData.Model;
using ML.BC.Web.Framework.BaiduAPI;
using ML.BC.Web.Framework.ViewModels;
using ML.BC.Web.Framework.Security;

namespace ML.BC.EnterpriseWeb.Areas.APIs.Controllers
{
    [AuthorizeCheck]
    public class SceneController : APIControllerBase
    {
        private log4net.ILog _ilog = log4net.LogManager.GetLogger(typeof(SceneController));
        private IProjectSceneManagementService service;
        public SceneController()
        {
            service = Ioc.GetService<IProjectSceneManagementService>();
        }
        [AllowCrossDomainPost]
        public ActionResult AddScene(SceneModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "AddScene", Serializer.ToJson(model)));

            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var sceneID = service.AddScene(model);

                result.Value = new { SceneID = sceneID };
            });
            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "AddScene", Serializer.ToJson(result)));
            return result;
            //return result;

        }

        [AllowCrossDomainPost]
        public ActionResult SetSceneStatus(ModelBase model, string sceneID, ML.BC.EnterpriseData.Common.Status status)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "SetSceneStatus", Serializer.ToJson(model)));

            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var resultBool = service.SetSceneStatus(sceneID, status);
                result.Value = new { IsUpdate = resultBool };
            });
            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "SetSceneStatus", Serializer.ToJson(result)));
            return result;
        }

        [AllowCrossDomainPost]
        public ActionResult UpdateScene(SceneModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "UpdateScene", Serializer.ToJson(model)));

            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var resultBool = service.UpdateScene(model);
                result.Value = new { IsUpdate = resultBool };
            });
            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "UpdateScene", Serializer.ToJson(result)));
            return result;

        }
    }
}
