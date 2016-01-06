using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IRoleFunctionManagementService : IServiceBase
    {
        string AddRoleFunction(int roleID, string functionID);
        bool DeleteRoleFunction(int roleID, string functionID);
        bool UpdateRoleFunction(int roleID, string functionID, bool available);
        List<string> GetAllFunctionIDByRoleID(int roleID);
        List<AuthorizationsDto> SetFunction(int roleID, string functionIDs);
    }
}
