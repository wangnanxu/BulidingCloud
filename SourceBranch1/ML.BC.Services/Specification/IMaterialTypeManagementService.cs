using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IMaterialTypeManagementService : IServiceBase
    {
        List<MaterialTypeDto> GetAllMaterialType();
        List<MaterialTypeDto> GetAllMaterialType(string Name,int row, int page, out int amount);
        bool Update(MaterialTypeDto model);
        bool Delete(int ID);
        bool Add(MaterialTypeDto model);
        
    }
}
