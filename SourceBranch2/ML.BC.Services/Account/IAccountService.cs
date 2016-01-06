using ML.BC.BCBackData.Model;
using ML.BC.Services.Account.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IAccountService : IServiceBase
    {
        UserDto Get(string username);
        bool CanLogin(string userId, string password);
        bool UpdateUserLogin(string userId, string lastIP);
        string CreateUser(Account.Dtos.UserDto userDto);
        SessionUserDto GetSessionUser(string userId);
        UserDto GetById(string userId);

        List<UserDto> GetList(int pageNumber, int pageSize, out int amount);

        List<UserDto> SearchUserByName(string nameKeyword, int pageNumber, int pageSize, out int amount);

        int GetAmount();

//        int GetAmount(Expression<Func<BackUser,bool>> filter);
        bool DeleteUser(string userId);

        bool UpdateUser(UserDto user);
        List<UserLoginStateDto> GetUserLoginStateList(string UserName, int pageNumber, int pageSize, out int amount);
        bool DeleteUserLoginState(string userId);
        List<UserLoginLogDto> SearchUserLoginLog(DateTime? beginTime, DateTime? endTime, string userName, string enterpriseName, EnterpriseData.Common.LoginStatus status, int pageNumber, int pageSize, out int amount);
    }
}
