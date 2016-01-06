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
    public class BCBackSession : BCSession
    {
        public override void Logout()
        {
            base.Logout();
        }
        public override void Login(string userId)
        {
            var accountService = Ioc.GetService<IAccountService>();
            accountService.UpdateUserLogin(userId, HttpContext.Current.Request.GetRequestIP());
            base.Login(userId);
        }

        public override void Init()
        {
            if (!IsAuthenticated)
            {
                _user = null;
                return;
            }
            ReloadAll(HttpContext.Current.User.Identity.Name);
        }

        protected override void ReloadAll(string userId)
        {
            _user = Ioc.GetService<IAccountService>().GetSessionUser(userId);
        }
    }
}
