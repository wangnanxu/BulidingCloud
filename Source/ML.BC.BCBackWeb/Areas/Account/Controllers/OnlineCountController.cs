using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Model;
using ML.BC.Web.ViewModels;
using ML.BC.BCBackWeb.Areas.Account.Models;
using ML.BC.Services.Account.Dtos;
using ML.BC.Web.Framework.Security;
using ML.BC.BCBackData.Common;
namespace ML.BC.BCBackWeb.Areas.Account.Controllers
{
  [Authorize]
    public class OnlineCountController : BCControllerBase
    {
        //
        // GET: /Account/OnlineCountIndex/
        IAccountService services = null;
        public OnlineCountController()
        {
            services=ML.BC.Infrastructure.Ioc.GetService<IAccountService>();
        }
        public ActionResult OnlineCountIndex()
        {
            return View();
        }
       [PermissionControlAttribute(Functions.Root_SysManagement_OnlineUser)]
        public ActionResult GetOnlineList(string UserName,int rows,int page)
        {
            var result = new StandardJsonResult<OnlineUserResult>();
            result.Try(() => {
                result.Value = new OnlineUserResult();
                int amount=0;
                List<UserLoginStateDto> list = services.GetUserLoginStateList(UserName, page, rows,out amount)
                    .OrderByDescending(n=>n.LoginTime).ToList();
                List<OnlineCountModel> mylist = new List<OnlineCountModel>();
                foreach (var user in list)
                {
                    mylist.Add(new OnlineCountModel() { 
                     UserLoginStateID=user.UserLoginStateID,
                     Device=user.Device,
                     LoginIP=user.LoginIP,
                     LoginTime=user.LoginTime.Value.ToString(),
                     LoginToken=user.LoginToken,
                     UserID=user.UserID,
                     UserName=user.UserName
                    });
                }
                result.Value.rows = mylist;
                result.Value.total = amount;
            });
            if (!result.Success)
            {
                result.Value = new OnlineUserResult();
            }
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
      [PermissionControlAttribute(Functions.Root_SysManagement_OnlineUser_ForceUserLogout)]
        public ActionResult DeleteOnlineUser(string UserID)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                result.Value = services.DeleteUserLoginState(UserID);
            });
            return result;
        }
    }
}
