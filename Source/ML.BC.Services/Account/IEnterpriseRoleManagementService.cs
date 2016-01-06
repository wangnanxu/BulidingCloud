using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.Services;

namespace ML.BC.Services
{
    public interface IEnterpriseRoleManagementService : IServiceBase
    {
            
        List<EnterpriseRoleDto> GetEnterpriseRoleList(string enterpriseName,string roleName,int pageSize,int pageNumber,out int amount);
//        string AddRoleFunction(int roleID, string functionID);
        string AddEnterpriseRole(EnterpriseRoleDto enterpriseRole);
        EnterpriseRoleDto GetEnterpriseRoleByRoleID(int RoleID);
        List<EnterpriseRoleDto> GetEnterpriseRoleByEnterpriseID(string enterpriseId);
        bool DeleteEnterpriseRole(int roleId);
        bool UpdateEnterpriseRole(EnterpriseRoleDto enterpriseRole);

        //企业端 
        /// <summary>
        /// 获取企业角色功能 ,授权
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        List<string> GetFunctions(int roleID);
       List<EnterpriseRoleDto> GetListToEP(string roleName,string enterpriseId, int pageSize, int pageNumber, out int amount);
       // string AddEnterpriseRoleToEP(EnterpriseRoleDto enterpriseRole);
       // EnterpriseRoleDto GetEnterpriseRoleByRoleIDToEP(int RoleID);
      //  List<EnterpriseRoleDto> GetEnterpriseRoleByEnterpriseIDToEP(string enterpriseId);
        bool DeleteEnterpriseRoleToEP(int roleId);
      //  bool UpdateEnterpriseRoleToEP(EnterpriseRoleDto enterpriseRole);
    }
}
