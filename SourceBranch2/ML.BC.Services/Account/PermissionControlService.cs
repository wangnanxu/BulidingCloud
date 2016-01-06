using ML.BC.Services.Account;
using ML.BC.Services.Account.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.Infrastructure;

namespace ML.BC.Services
{
    public class PermissionControlService : IPermissionControlService
    {
        public bool HasPermission(SessionUserDto sessionUserDto, string[] permissionIds)
        {
            if (permissionIds.Any(n => n == "功能ID")) return true;
            return sessionUserDto != null && ((string.IsNullOrEmpty(sessionUserDto.EnterpriseID) && sessionUserDto.UserID == "A00001") || sessionUserDto.FunctionIDs.Any(n => permissionIds.Any(m => n.Eq(m))));
        }

        public bool HasToken(string tokenStr)
        {
            throw new NotImplementedException();
        }
    }
}
