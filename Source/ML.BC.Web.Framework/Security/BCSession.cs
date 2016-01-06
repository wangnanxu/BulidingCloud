using System.Web;
using System.Web.SessionState;
using ML.BC.Services;
using ML.BC.Infrastructure;
using ML.BC.Services.Account.Dtos;
using System;

namespace ML.BC.Web.Framework
{
    public class BCSession : IBCSession
    {
        protected SessionUserDto _user;

        public SessionUserDto User
        {
            get { return _user; }
        }
        public virtual void Logout()
        {
            _user = null;
        }

        public virtual void Login(string userId)
        {
            ReloadAll(userId);
        }

        public virtual void Init()
        {
            if (!IsAuthenticated)
            {
                _user = null;
                return;
            }
            ReloadAll(HttpContext.Current.User.Identity.Name);
        }

        protected virtual void ReloadAll(string userId)
        {

        }

        public virtual bool IsAuthenticated
        {
            get { return HttpContext.Current.Request.IsAuthenticated; }
        }

    }
}

namespace ML.BC.Web.Framework
{
    public static class SessionExtensions
    {
        private const string MvcSolutionSessionKey = "BCSession";

        public static IBCSession GetBCSession(this HttpSessionState session)
        {
            if (session[MvcSolutionSessionKey] == null)
            {
                var mvcSession = Ioc.GetService<IBCSession>();
                mvcSession.Init();
                session[MvcSolutionSessionKey] = mvcSession;
                return mvcSession;
            }
            return session[MvcSolutionSessionKey] as IBCSession;
        }
    }
}