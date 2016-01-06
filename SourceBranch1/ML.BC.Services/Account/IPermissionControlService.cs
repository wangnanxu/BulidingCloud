using ML.BC.Services.Account.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IPermissionControlService : IServiceBase
    {
        bool HasPermission(SessionUserDto sessionUserDto, string[] permissionIds);
        
        //  移动设备连接服务时，查询数据库是否有此token
        bool HasToken(string tokenStr);
    }
}
