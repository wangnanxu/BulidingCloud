using ML.BC.Infrastructure;
using ML.BC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace ML.BC.Web.Framework
{
    public class BCEnterpriseSession : BCSession
    {
        private FormsAuthenticationTicket _ticket;
        public override void Logout()
        {
            if (User != null)
            {
                var accountService = Ioc.GetService<IEnterpriseAccountService>();
                if (_ticket == null)
                {
                    FormsIdentity formIdentity = HttpContext.Current.User.Identity as FormsIdentity;
                    _ticket = formIdentity != null ? formIdentity.Ticket : null;
                }
                string Device = _ticket == null ? "PC" : _ticket.UserData;
                accountService.DeleteUserLoginState(User.UserID, Device);
            }
            base.Logout();
        }
        public override void Login(string userId)
        {
            string device = HttpContext.Current.Request.GetRequestDevice();
            bool RememberMe = HttpContext.Current.Request.GetRequestRememberMe(); ;
            var encryptTiket = HttpContext.Current.Response.SetAuthCookie(userId, device, RememberMe);
            _ticket = System.Web.Security.FormsAuthentication.Decrypt(encryptTiket);
            string loginIP = HttpContext.Current.Request.GetRequestIP();

            var accountService = Ioc.GetService<IEnterpriseAccountService>();
            accountService.UpdateUserLoginState(userId, loginIP, device, encryptTiket);

            base.Login(userId);
        }

        public override void Init()
        {
            if (!IsAuthenticated)
            {
                _user = null;
                return;
            }

            FormsIdentity formIdentity = HttpContext.Current.User.Identity as FormsIdentity;
            _ticket = formIdentity != null ? formIdentity.Ticket : null;
            ReloadAll(_ticket.Name);
        }

        protected override void ReloadAll(string userId)
        {
            if (_ticket == null) return;
            _user = Ioc.GetService<IEnterpriseAccountService>().GetSessionUser(userId, _ticket.UserData);
        }
    }
}
