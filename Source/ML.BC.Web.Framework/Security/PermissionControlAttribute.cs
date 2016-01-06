using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Services;
using ML.BC.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ML.BC.Web.Framework.Security
{
    public class PermissionControlAttribute : ActionFilterAttribute
    {
        private List<string> _permissionItemIds = new List<string>();
        private IDictionary<string, object> _actionParameters;
        
        public PermissionControlAttribute(string permissionId)
        {
            _permissionItemIds.Add(permissionId);
        }
        public PermissionControlAttribute(string[] permissionIds)
        {
            _permissionItemIds = permissionIds.ToList();
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _actionParameters = filterContext.ActionParameters;

            var permissionControlService = ML.BC.Infrastructure.Ioc.GetService<IPermissionControlService>();
            var sessionUser = System.Web.HttpContext.Current.Session.GetBCSession();

            if (sessionUser == null || !sessionUser.IsAuthenticated || sessionUser.User == null || !permissionControlService.HasPermission(sessionUser.User, _permissionItemIds.ToArray()))
            {
                throw new KnownException("您没有权限执行此操作，请联系管理员分配权限。");
            }            
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
            var sessionUser = System.Web.HttpContext.Current.Session.GetBCSession();
            if (sessionUser == null || sessionUser.User == null) return;

            var operationLogService = ML.BC.Infrastructure.Ioc.GetService<IOperationLogService>();
            operationLogService.AddLog(new ML.BC.EnterpriseData.Model.OperationLog
            {
                UserID = sessionUser.User.UserID,
                EnterpriseID = string.IsNullOrEmpty(sessionUser.User.EnterpriseID)? "":sessionUser.User.EnterpriseID,
                OperationID = string.Join("|", _permissionItemIds),
                ClientIP = System.Web.HttpContext.Current.Request.GetRequestIP(),
                OperateTime = DateTime.Now,
                OperationData = _actionParameters == null ? string.Empty : Serializer.ToJson(_actionParameters)
            });
        }
    }
}
