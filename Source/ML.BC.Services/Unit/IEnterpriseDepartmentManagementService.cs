using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IEnterpriseDepartmentManagementService : IServiceBase
    {
        List<DepartmentDto> SearchDepartmentByName(string nameKeyword, int pageSize, int pageNumber);
        List<DepartmentDto> GetMyDepartment(string enterpriseId);
        List<DepartmentDto> GetAllAvaliableDepartmentByEnterpriseId(string enterpriseId);//获取全部可用的
        bool Update(DepartmentDto department);
        List<DepartmentDto> GetMyDepartment(string enterpriseId, int? departmentId);
        DepartmentDto GetParentDepartment(int departmentId);
        bool Delete(int departmentId,string enterpriseId);
        int Add(DepartmentBase department);
        int GetAmount();
        string GetEnterpriseNameById(string enterpriseId);
        string GetDepartmentNameById(int departmentId);//+
        DepartmentDto GetRootDepartmentByEntpriseId(string enterpriseId);//获取某企业的根部门节点
    }
}
