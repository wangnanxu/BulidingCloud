using ML.BC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Web.Framework
{
    public class BCViewBase<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private IPermissionControlService _permissionControlService = null;
        private IPermissionControlService PermissionControlService
        {
            get
            {
                _permissionControlService = _permissionControlService ?? ML.BC.Infrastructure.Ioc.GetService<IPermissionControlService>();
                return _permissionControlService;
            }
        }

        private IBCSession _bcSession = null;
        protected IBCSession BCSession
        {
            get {
                _bcSession = _bcSession??System.Web.HttpContext.Current.Session.GetBCSession();
                return _bcSession;
            }
        }
        protected bool HasFunction(string functionID)
        {
            return PermissionControlService.HasPermission(BCSession.User, new string[] { functionID });
        }
        protected bool HasFunction(string[] functionIDs)
        {
            return PermissionControlService.HasPermission(BCSession.User, functionIDs);
        }
        public override void Execute()
        {
        }
    }

    public class BCViewBase : BCViewBase<dynamic>
    {
    }
}
