using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.Services;

namespace ML.BC.Services
{
    public interface IFrontUserRoleManagementService : IServiceBase
    {
        string AddFrontUserRole(string frontUserID, int roleID);
        bool DeleteFrontUserRole(string frontUserID, int roleID);

        List<int> GetAllEnterpriseRolesByFrontUserId(string frontUserId);
        List<UserRolesDto> SetFrontUserRoles(string frontUserId, List<int> enterpriseRoleIDs);
    }
}
