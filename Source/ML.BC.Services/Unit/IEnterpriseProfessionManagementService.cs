using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IEnterpriseProfessionManagementService : IServiceBase
    {
        List<EnterpriseProfessionDto> GetEnterpriseProfessionList(string nameKeyword, int pageSize, int pageIndex, out int amount);
        List<EnterpriseProfessionDto> GetAllEnterpriseProfession();   //获取所有行业信息
        List<EnterpriseProfessionDto> GetAllEnterpriseProfession(int pageSize, int pageIndex, out int count);
        string AddEnterpriseProfession(string enterpriseProfessionKey, string enterpriseProfessionName, bool available);
        bool DeleteEnterpriseProfession(string enterpriseProfessionId);
        bool UpdateEnterpriseProfession(string enterpriseProfessionKey, string enterpriseProfessionName, bool available);
    }
}
