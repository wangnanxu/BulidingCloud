using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace ML.BC.Services
{
    public interface IRolesManagementService : IServiceBase
    {
        int AddRole(RolesDto role);
        bool DeleteRole(int roleId);
        bool UpdateRole(RolesDto role);
        List<RolesDto> GetRoleList(RolesDto model, int pageSize, int pageIndex, out int count);

    }
}
