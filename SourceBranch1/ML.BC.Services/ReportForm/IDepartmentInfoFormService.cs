using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IDepartmentInfoFormService
    {
        List<DepartmentInfoFormDto> GetDepartmentInfoFormOfEnterprise(string enterpriseId, DateTime? beginTime, DateTime? endTime);
    }
}
