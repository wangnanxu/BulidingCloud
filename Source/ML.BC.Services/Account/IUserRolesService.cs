using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace ML.BC.Services
{
    interface IUserRolesServerce : IServiceBase
    {
        string AddUserRole(string userID, int roleID);
        bool DeleteUserRole(string userID, int roleID);

        List<int> GetAllRolesByUserId(string userId);

        List<RolesDto> GetAllRoles();
        //List<UserRolesDto> SetRole(string userId, List<int> roleID);

        //int UpdateUserRole(UserRolesDto userole);
        //List<AuthorizationsDto> SelectUserRole(Expression<Func<UserRolesDto, bool>> foo);

    }
}
