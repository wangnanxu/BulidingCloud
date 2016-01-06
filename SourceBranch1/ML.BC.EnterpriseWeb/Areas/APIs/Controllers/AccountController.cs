using ML.BC.Infrastructure;
using ML.BC.Services;
using ML.BC.EnterpriseWeb.Areas.Account.Models;
using ML.BC.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Web.Framework;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using System.Web.Security;
using ML.BC.EnterpriseWeb.Areas.APIs.Models;
using ML.BC.Services.Account.Dtos;
using ML.BC.Web.Framework.Security;

namespace ML.BC.EnterpriseWeb.Areas.APIs.Controllers
{
    public class AccountController : APIControllerBase
    {
        public ActionResult Test()
        {
            return Content("这是一个测试，用于测试API是否可用");
        }

        [AllowCrossDomainPost]
        public ActionResult Login(LoginModel model)
        {
            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var service = Ioc.GetService<IEnterpriseAccountService>();
                if (!service.CanLogin(model.UserName, model.Password))
                {
                    throw new KnownException("用户名/密码错误，登录失败");
                }
                BCSession.Login(model.UserName);

                result.Value = new
                {
                    UserID = BCSession.User.UserID,
                    UserName = BCSession.User.UserName,
                    EnterpriseID = BCSession.User.EnterpriseID,
                    EnterpriseName = BCSession.User.EnterpriseName,
                    DepartmentID = BCSession.User.DepartmentID,
                    HeadImage = BCSession.User.Picture,
                    FunctionIDs = BCSession.User.FunctionIDs.Where(n => n.StartsWith("Root.AppPermission")).ToArray(),
                    RoleIDs = BCSession.User.RoleIDs,
                    Token = BCSession.User.Token
                };
            });
            return result;
        }

        [AuthorizeCheck]
        [AllowCrossDomainPost]
        public ActionResult Logout(ModelBase model)
        {
            var result = new StandardJsonResult();
            result.Try(() =>
            {
                FormsAuthentication.SignOut();
                GetSession().Logout();
                Session.Clear();
                Session.Abandon();
            });
            return result;
        }

        [AuthorizeCheck]
        [AllowCrossDomainPost]
        public ActionResult SetUserPicture(ModelBase model)
        {
            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                if (Request.Files.Count == 0) throw new KnownException("没有图片上传到服务器！");

                var service = Ioc.GetService<IEnterpriseAccountService>();
                var tempUser = service.GetById(BCSession.User.UserID);

                var pictureStream = ML.BC.Infrastructure.Utilities.ImageHelper.PressImage(Request.Files[0].InputStream, 200, 200);
                var filePath = string.Format("/UserAvatar/{0}_{1}.jpg", tempUser.UserID, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                using (var file = System.IO.File.OpenWrite(Server.MapPath(filePath)))
                {
                    pictureStream.WriteTo(file);
                    file.Flush();
                }
                var userSecurService = Ioc.GetService<IEnterpriseUserManagementService>();
                userSecurService.SetUserAvatar(tempUser.UserID, filePath);

                if (System.IO.File.Exists(Server.MapPath(tempUser.Picture)))
                {
                    try
                    {
                        var file = new System.IO.FileInfo(Server.MapPath(tempUser.Picture));
                        file.Attributes = System.IO.FileAttributes.Normal;
                        file.Delete();
                    }
                    catch { }
                }
                result.Value = UriExtensions.GetFullUrl(filePath);
            });
            return result;
        }

        [AuthorizeCheck]
        [AllowCrossDomainPost]
        public ActionResult SetUserPassword(SetUserPasswordModel model)
        {
            var result = new StandardJsonResult();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                var userSecurService = Ioc.GetService<IEnterpriseUserManagementService>();
                userSecurService.UpdateUserPassword(BCSession.User.UserID, model.OldPassword, model.NewPassword);
            });
            return result;
        }
    }
}
