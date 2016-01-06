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
    public interface IEnterpriseFunctionManagementService : IServiceBase
    {
        List<FunctionDto> GetAllFunctions(string nameKeyword, int psize, int pageidx);
        List<FunctionDto> GetAllFunctions();
        /// <summary>
        /// 获取企业用户拥有的功能
        /// </summary>
        /// <param name="UserID">用户id</param>
        /// <returns></returns>
        List<FunctionDto> GetFunctionsByEnterpriseUserID(string UserID);
        List<FunctionDto> GetFunctionsByFunctionIds(string[] functionIds);

        //        List<FunctionDto> Select(Expression<Func<FunctionDto, bool>> foo);

        int Update(FunctionDto obj);

        bool Delete(string FunctionID);

        int Add(FunctionDto obj);
        //public List<FunctionDto> GetFunctionsByEnterpriseUserID(string UserID)

        int GetAmount();

        bool ExistFunction(string funcId);

        //        List<FunctionDto> GetList(Expression<Func<FunctionDto, bool>> foo,int pagesize,int pagenumber);
    }
}
