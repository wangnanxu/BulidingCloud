using ML.BC.Services.Enterprise.Dtos;
using ML.BC.Services.Unit.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{

    public interface IEnterpriseManagementService : IServiceBase
    {
        //通过企业ID、行业ID、企业性质ID分页查询企业信息
        List<EnterpriseDto> SearchEnterpriseByCondition(string professionId, string propertyId, string enterpriseName, int pSize, int pNum, out  int count);
        int GetAmount(); // 获取总数 
        List<EnterpriseDto> GetAllEnterpriseList();
        EnterpriseDto GetOneByEnterpriseID(string enterpriseId);
        bool DeleteEnterprise(string enterpriseId);//通过企业ID删除企业
        string AddEnterprise(EnterpriseDto enterpriseDto);//添加企业
        bool UpdateEnterprise(EnterpriseDto enterpriseDto); //更新企业信息

        EnterpriseSyncDto GetEnterpriseForSync(string enterpriseId);
        EnterpriseSyncDto GetEnterpriseForSync(string enterpriseID, string userID, string deviceID);
    }
}
