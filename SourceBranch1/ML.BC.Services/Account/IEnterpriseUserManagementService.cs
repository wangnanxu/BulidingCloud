using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{

    public interface IEnterpriseUserManagementService : IServiceBase
    {
        //获取用户列表
        List<FrontUserDto> GetUserList(string enterpriseID, int pageIndex, int pageSize, out int totalAmount);
        //搜索用户
        List<FrontUserDto> SearchUser(string enterpriseID, string departmentKeyword, string nameKeyword, int pageIndex, int pageSize, out int totalAmount);
        //删除用户
        bool DeleteUser(string userID); 
        //禁用用户
        bool DisableUser(string userID);
        //新建用户
        FrontUserDto AddUser(FrontUserDto newUser);
        //更新用户资料
        bool UpdateUser(FrontUserDto user);
        //更新用户密码
        bool UpdateUserPassword(string userId, string oldPass, string newPass);
        //检查用户是否存在
        bool IsExistUser(string userName);
        //检查用户密码
        bool CheckUserPass(string userID, string pass);

        List<FrontUserDto> GetUserByDepartmentIDs(List<int> departIDs, int pageSize, int pageIndex, out int count);
        
        //设置头像 
        bool SetUserAvatar(string userID, string imageUrl);
        
    }
}
