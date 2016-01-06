using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model;
using ML.BC.Infrastructure.Model;
using ML.BC.EnterpriseData.MongoDb;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Exceptions;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver.Linq;

namespace ML.BC.Services.Common
{
    public class OperationLogService : IOperationLogService
    {
        public bool AddLog(OperationLog log)
        {
            try
            {
                if (null == log) return false;
                if (string.IsNullOrEmpty(log.ClientIP) || string.IsNullOrEmpty(log.UserID) ||
                    string.IsNullOrEmpty(log.EnterpriseID) || string.IsNullOrEmpty(log.EnterpriseID))
                {
                    var logger = log4net.LogManager.GetLogger(typeof(OperationLogService));
                    logger.Error("缺失必要日志信息IP:" + log.ClientIP + ",UserID:" + log.UserID + ",EnterpriseID:" + log.EnterpriseID + ",OperationID:" + log.EnterpriseID + "");
                }

                var db = new MongoDbProvider<OperationLog>();
                db.Insert(log);
                return true;
            }
            catch (Exception ex)
            {
                //  输出到log4net
                var msg = ex.Message;
                var logger = log4net.LogManager.GetLogger(typeof(OperationLogService));
                logger.Error(msg);
                return false;
            }

        }

        public bool ClearLog()
        {
            try
            {
                var db = new MongoDbProvider<OperationLog>();
                db.Remove();
                return true;
            }
            catch (Exception ex)
            {
                //      输出到log4net
                var message = ex.Message;
                var logger = log4net.LogManager.GetLogger(typeof(OperationLogService));
                logger.Error(message);
                return false;
            }

        }

        public List<OperationLogDto> SearchLogsByCondition(SearchLogConditionDto condition, int pageSize, int pageIndex, out int count)
        {
            try
            {
                if (null == condition) throw new KnownException("没有给定任何搜索条件查询！");

                //  从MongoDB查询得到OperationLog
                var reList = SearchOperationLogsByMongo(condition);

                //  分页显示
                count = reList.Count();
                int pageTotal;

                if (pageSize > 0)
                {
                    pageTotal = (count + pageSize - 1) / pageSize;
                }
                else
                {
                    pageSize = 10;
                    pageTotal = (count + pageSize - 1) / pageSize;
                }

                if (pageIndex > pageTotal)
                    pageIndex = pageTotal;
                if (pageIndex < 1)
                    pageIndex = 1;
                #region 由OperationLog相关ID通过EF查出对应Name
                using (var db = new BCEnterpriseContext())
                {
                    //  将从MongoDB查询按照分页ToList需要的结果，根据其结果的ID再通过EF查询对应的Name
                    var l = reList
                            .OrderByDescending(obj => obj.OperateTime)
                            .Skip(pageSize * (pageIndex - 1))
                            .Take(pageSize).ToList();

                    var userIds = l.Select(n => n.UserID);
                    var operationIds = l.Select(n => n.OperationID);

                    var users = (from u in db.FrontUsers
                                 where userIds.Any(n => n == u.UserID)
                                 select new { u.UserID, u.Name }).Distinct().ToList();

                    var operations = (from fun in db.RFAFunctions
                                      where operationIds.Any(n => n.StartsWith(fun.FunctionID))
                                      select new { fun.FunctionID, fun.Name }
                        ).Distinct().ToList();

                    Func<string, string> generateOperationName = (operationId) =>
                    {
                        if (string.IsNullOrEmpty(operationId)) return string.Empty;

                        var result = new StringBuilder();
                        var subIds = operationId.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < subIds.Length; i++)
                        {
                            var tempId = string.Empty;
                            int index = 0;
                            while (index <= i)
                            {
                                tempId = string.Format("{0}.{1}", tempId, subIds[index]);
                                index++;
                            }
                            if (!string.IsNullOrEmpty(tempId))
                            {
                                tempId = tempId.Trim('.');
                                var tempOperation = operations.FirstOrDefault(n => n.FunctionID == tempId);
                                if (tempOperation != null)
                                {
                                    result.AppendFormat("{0}.", tempOperation.Name);
                                }
                            }
                        }
                        var resultName = result.ToString();
                        return string.IsNullOrEmpty(resultName) ? resultName : resultName.Trim('.');
                    };

                    var resultLogs = (from op in l
                                      join u in users on op.UserID equals u.UserID into tempU
                                      from tu in tempU.DefaultIfEmpty()
                                      select new OperationLogDto
                                      {
                                          Id = op.Id,
                                          UserID = op.UserID,
                                          OperationID = op.OperationID,
                                          EnterpriseID = op.EnterpriseID,
                                          ClientIP = op.ClientIP,
                                          OperationData = op.OperationData,
                                          OperateTime = op.OperateTime,
                                          OperationName = generateOperationName(op.OperationID),
                                          UserName = tu.Name,
                                      }).ToList();
                    return resultLogs;
                }
                #endregion
            }
            catch (Exception ex)
            {

                throw new KnownException(ex.Message);
            }

        }

        //  此处需要优化，每次查询Name都需要连接数据库,想办法把这个方法用from where select 方式集成到上一个查询，这样就不用新建连接
        private static string FunctionId2Name(string funID)
        {
            try
            {
                var queryFunID = new List<string>();

                //构建查询语句查出需要用于查询的结果FunIDList
                IQueryable v;

                v = (IQueryable)(from b in Encoding.ASCII.GetBytes(funID)
                                 where 46 == b
                                 select new { s = funID.Substring(0, 8) });


                //var flid = from id in funID.Split('.')
                //    select new {s = id};
                //
                //var r = funID.Split('.').ToList();
                //var reStr = r.Aggregate("", (current, k) => current + k);


                //  构造需要查询的FunID
                var bytArr = Encoding.ASCII.GetBytes(funID);
                var length = bytArr.Length;
                for (int i = 0; i < length; i++)
                    if (46 == bytArr[i]) queryFunID.Add(funID.Substring(0, i));
                queryFunID.Add(funID);

                //  由queryFunID查询出完整的功能名称并return
                using (var db = new BCEnterpriseContext())
                {
                    var fun =
                        db.RFAFunctions.Where(obj => queryFunID.Contains(obj.FunctionID))
                        .OrderBy(obj => obj.FunctionID.Length)
                            .Select(obj => new { fName = obj.Name })
                            .ToList()
                            .Aggregate("", (current, f) => current + f.fName);

                    //foreach (var f in fun)
                    //{
                    //    reStr += f.fName;
                    //}
                    //var re = (from ss in fun.ToList()
                    //          select ss.fName)
                    //       .Aggregate((total, next) =>
                    //       {
                    //           return total + next;
                    //       });

                    //var s = fun.Aggregate(fun.First().fName, (current, f) => current + f.fName);
                    //var fff = fun.ToList().Aggregate(reStr, (current, f) => current + f.fName);
                    //return fun.Aggregate(reStr, (current, f) => current + f.fName);
                    //return re;
                    return fun;
                }
            }
            catch (Exception ex)
            {

                throw new KnownException(ex.Message);
            }

        }

        private static IEnumerable<OperationLog> GetAllOperationLog()
        {
            var mgdb = new MongoDbProvider<OperationLog>();
            return mgdb.GetAll().ToList();
        }

        //  可选优化，IEnumerable查询会加载所有结果，IQueryable只会加载分页结果
        //  controller已确保EnterpriseID的正确使用
        private static IEnumerable<OperationLog> SearchOperationLogsByMongo(SearchLogConditionDto condition)
        {
            try
            {
                IEnumerable<OperationLog> reList;
                List<string> uIDList = new List<string>();
                var mgdb = new MongoDbProvider<OperationLog>();

                //  不存在UserID时候查询过程
                if (string.IsNullOrEmpty(condition.UserID))
                {
                    using (var db = new BCEnterpriseContext())
                    {
                        uIDList = db.FrontUsers.Where(obj => (string.IsNullOrEmpty(condition.EnterpriseID) ? true : obj.EnterpiseID == condition.EnterpriseID) && (string.IsNullOrEmpty(condition.UserName) ? true : obj.Name.Contains(condition.UserName))).Select(o => o.UserID).ToList();
                    }
                    if (0 == uIDList.Count)
                    {
                        if (string.IsNullOrEmpty(condition.OperationID))
                        {
                            // 第一次显示操作日志判断 
                            reList = mgdb.GetAll(obj => (string.IsNullOrEmpty(condition.EnterpriseID) || obj.EnterpriseID == condition.EnterpriseID) && (obj.OperateTime > (condition.StartTime ?? DateTime.MinValue))
                                         && (obj.OperateTime < (condition.EndTime ?? DateTime.Now)));
                        }
                        else
                        {
                            //  UserID和UserName为空，仅根据企业ID和操作ID查询
                            reList = mgdb.GetAll(obj => (string.IsNullOrEmpty(condition.EnterpriseID) || obj.EnterpriseID == condition.EnterpriseID)
                                         && (obj.OperationID.ToLower().Contains(condition.OperationID.ToLower()))
                                         && (obj.OperateTime > (condition.StartTime ?? DateTime.MinValue))
                                         && (obj.OperateTime < (condition.EndTime ?? DateTime.Now)));
                        }

                    }
                    else
                    {
                        //  输入UserName查询得到uIDList模糊查询
                        var tempCondition = string.IsNullOrEmpty(condition.OperationID) ? "" : condition.OperationID.ToLower();
                        reList = mgdb.GetAll(obj => (uIDList.Contains(obj.UserID))
                                                 && (string.IsNullOrEmpty(condition.EnterpriseID) || obj.EnterpriseID == condition.EnterpriseID)
                                                 && (string.IsNullOrEmpty(condition.OperationID) || obj.OperationID.ToLower().Contains(tempCondition))
                                                 && (obj.OperateTime > (condition.StartTime ?? DateTime.MinValue))
                                                 && (obj.OperateTime < (condition.EndTime ?? DateTime.Now)));
                    }
                }

                //  指定UserID查询过程
                else
                {
                    //  指定了UserID查询
                    reList = mgdb.GetAll(obj => (obj.UserID == condition.UserID)
                                             && (string.IsNullOrEmpty(condition.EnterpriseID) || obj.EnterpriseID == condition.EnterpriseID)
                                             && (obj.OperateTime > (condition.StartTime ?? DateTime.MinValue))
                                             && (obj.OperateTime < (condition.EndTime ?? DateTime.Now)));
                }
                return reList;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
