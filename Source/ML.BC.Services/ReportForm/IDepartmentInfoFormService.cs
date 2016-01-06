using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IDepartmentInfoFormService
    {
        List<DepartmentInfoFormDto> GetDepartmentInfoFormOfDepartment(int? departmentId, string EnterpriseId, DateTime? beginTime, DateTime? endTime);
        List<DepartmentInfoFormDto> GetDepartmentInfoFormOfEnterprise(string EnterpriseId, DateTime? beginTime, DateTime? endTime);
    }
}
