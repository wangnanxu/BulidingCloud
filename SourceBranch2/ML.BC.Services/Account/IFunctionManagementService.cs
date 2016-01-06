using ML.BC.Services.Account.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Data.Entity;
using ML.BC.EnterpriseData.Model;

namespace ML.BC.Services
{
    public interface IFunctionManagementService : IServiceBase
    {
        List<FunctionDto> GetAllFunctions(string nameKeyword, int psize, int pageidx);
        List<FunctionDto> GetAllFunctions();

//        List<FunctionDto> Select(Expression<Func<FunctionDto, bool>> foo);

        int Update(FunctionDto obj);
        bool Delete(string FunctionID);

        int Add(FunctionDto obj);

        int GetAmount();

        bool ExistFunction(string funcId);

//        List<FunctionDto> GetList(Expression<Func<FunctionDto, bool>> foo,int pagesize,int pagenumber);
    }
}
