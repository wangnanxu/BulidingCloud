using ML.BC.EnterpriseData.Model;
using ML.BC.Services.Account.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IEnterpriseAccountService : IServiceBase
    {
        FrontUserDto Get(string username);
        bool CanLogin(string username, string password);
        bool CheckUserIsLogin(string userCode, string device);
        bool UpdateUserLoginState(string userId, string lastIP, string Device, string LoginToken);
        string CreateUser(FrontUserDto userDto);
        SessionUserDto GetSessionUser(string userId,string device);
        FrontUserDto GetById(string userId);

        List<FrontUserDto> GetList(int pageNumber, int pageSize, out int amount);

        List<FrontUserDto> SearchUserByName(string nameKeyword, int pageNumber, int pageSize, out int amount);

        int GetAmount();

        int GetAmount(Expression<Func<FrontUser, bool>> filter);

        bool DeleteUser(string userId);

        bool UpdateUser(FrontUserDto user);


        //-------------用户登陆状态-------------
        List<UserLoginStateDto> GetUserLoginStateList(SessionUserDto User, bool ShowAll, string UserName, int pageNumber, int pageSize, out int amount);
        bool DeleteUserLoginState(long UserLoginStateID, string reason = "");
        bool DeleteUserLoginState(string UserID, string Device = "", string reason = "");
        bool RefreshUserLoginState(string userId, string device = "");
    }
}
