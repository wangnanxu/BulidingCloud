using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.Services.Unit.Dtos;
using ML.BC.Services.Account.Dtos;

namespace ML.BC.Services
{
    public interface IFrontUserManagementService : IServiceBase
    {
        bool DeleteFrontUser(string frontUserId);
        string AddFrontUser(FrontUserDto frontUserDto);
        bool UpdateFrontUser(FrontUserDto frontUserDto);
        //更新资料 不含密码
        bool UpdateFrontUserInfo(FrontUserDto frontUserDto);
        List<FrontUserDto> GetScanUsers(string sceneFunctionID,string projectFunctionID, string enterpriseID, List<int?> departmentID);
        FrontUserDto GetFrontUserByUserID(string userID);
        List<FrontUserDto> GetFrontUserByPartialInfo(FrontUserDto model, int pageSize, int pageIndex, out int count);
        List<FrontUserDto> GetAllFrontUser(string enterpriseId, int pageSize, int pageIndex, out int count);
        List<FrontUserDto> SearchUserByName(string enterpriseNameKeyword, string userNameKeyword, int pageIndex, int pageSize, out int totalAmount);
        List<FrontUserDto> GetAllUserByDepartmentID(string enterpriseID,int? departmentID, int pageSize, int pageIndex, out int count);

    }
}
