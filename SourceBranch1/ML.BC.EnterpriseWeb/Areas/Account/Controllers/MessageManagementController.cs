using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Services;
using ML.BC.Infrastructure.Model;
using ML.BC.Infrastructure.Mvc;
using ML.BC.EnterpriseWeb.Areas.Account.Models;
using ML.BC.EnterpriseData.Common;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Web.Framework.Security;
namespace ML.BC.EnterpriseWeb.Areas.Account.Controllers
{
    [AuthorizeCheck]
    public class MessageManagementController : BCControllerBase
    {
        //
        // GET: /Account/MessageManagement/
        private IChatMessageService serrvice;
        public ActionResult MessageIndex()
        {
            return View();
        }
        public MessageManagementController()
        {
            serrvice = ML.BC.Infrastructure.Ioc.GetService<IChatMessageService>();
        }
        public ActionResult GetList(string jsonstring, int rows, int page)
        {
            var result = new StandardJsonResult<MessageResult>();
            result.Try(() =>
            {
                List<MessageModel> mylist = new List<MessageModel>();
                List<ChatDto> list = new List<ChatDto>();
                int amount = 0;
                //初始化模式
                if (jsonstring == null)
                {
                    list = serrvice.SearchChatMessages(BCSession.User.UserID, null, null, "", ReadStatus.All, rows, page, out amount);
                  
                }
                else//搜索模式
                {
                    MessageSearchModel model = ML.BC.Infrastructure.Serializer.FromJson<MessageSearchModel>(jsonstring);
                    if (model.Status == null)
                    {
                        model.Status = 4;
                    }
                    list = serrvice.SearchChatMessages(BCSession.User.UserID, model.BeginDate, model.EndDate, model.Sender, (ReadStatus)model.Status, rows, page, out amount);
                }
                foreach (var a in list)
                {
                    mylist.Add(new MessageModel()
                    {
                        Message = a.Message,
                        Sender = a.SenderName,
                        SendTime = a.SendTime.ToString(),
                        Status = (int)a.IsRead,
                     MessageID=a.MessageID
                    });
                }
                result.Value = new MessageResult();
                result.Value.rows = mylist;
                result.Value.total = amount;
            });
            if (!result.Success)
            {
                result.Value = new MessageResult();
            }
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadMessage(string messageid)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() => {
                Guid myguid = new Guid();
                if (Guid.TryParse(messageid, out myguid))
                {
                    result.Value = serrvice.ChangeReadStatus(myguid,BCSession.User.UserID);
                    result.Message = "已成功标记已读！";
                }
                else
                {
                    throw new KnownException("消息编号错误，标记失败");
                }
            });
            if (!result.Success)
                result.Message = "标记失败！";
            return result;
        }
    }
}
