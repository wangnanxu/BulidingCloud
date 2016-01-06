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
    public class ChatMessageController : APIControllerBase
    {
        private log4net.ILog _ilog = log4net.LogManager.GetLogger(typeof(ChatMessageController));
        private IChatMessageService service;

        public ChatMessageController()
        {
            service = Ioc.GetService<IChatMessageService>();
        }

        [AllowCrossDomainPost]
        public ActionResult Send(ChatMessageModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "SendChatMessage", Serializer.ToJson(model)));

            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var isSuccess = service.ProcessChatMessage(new ChatMessageDto()
                {
                    SendUserID = BCSession.User.UserID,
                    Recipients = model.Recipients,
                    SendTime = model.SendTime,
                    Message = model.Message,
                    MessageID = model.MessageID
                });
                result.Value = new { isSend = isSuccess };
            });
            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "SendChatMessage", Serializer.ToJson(result)));
            return result;
        }

        [AllowCrossDomainPost]
        public ActionResult GetChatMessage(GetChatMessageModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "GetChatMessage", Serializer.ToJson(model)));
            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                int count = 0;
                var list = service.GetChatMessages(BCSession.User.UserID, BCSession.User.Device, model.PageSize,
                    model.QueryTime, out count);
                result.Value = new { Data = list };
            });
            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "GetChatMessage", Serializer.ToJson(result)));
            return result;
        }
    }
}
