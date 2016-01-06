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
    public interface IEnterprisePropertyManagementService : IServiceBase
    {
        List<EnterprisePropertyDto> SearchEnterprisePropertyByName(string nameKeyword, int pageSize, int pageNumber, out int amount);
        List<EnterprisePropertyDto> GetAllEnterpriseProperty(int pageSize, int pageNumber, out int amount);   //获取所有行业信息
        string AddEnterpriseProperty(EnterprisePropertyDto enterpriseproperty);
        List<EnterprisePropertyDto> GetAllEnterpriseProperty();
        bool DeleteEnterpriseProperty(string enterprisepropertyId);
        bool UpdateEntProperty(EnterprisePropertyDto enterpriseproperty);
    }
}
