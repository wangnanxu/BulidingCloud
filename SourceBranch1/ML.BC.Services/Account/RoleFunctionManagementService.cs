using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.BCBackData.Model;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Exceptions;

namespace ML.BC.Services
{
    public class RoleFunctionManagementService : IRoleFunctionManagementService
    {
        public string AddRoleFunction(int roleID, string functionID)
        {
            try
            {
                if (string.IsNullOrEmpty(functionID))
                {
                    throw new KnownException("参数无效！");
                }

                using (var db = new BCBackContext())
                {
                    db.RFAAuthorizations.Add(new RFAAuthorization() { RoleID = roleID, FunctionID = functionID, UpdateTime = DateTime.Now });
                    if (1 == db.SaveChanges())
                    {
                        return functionID;
                    }
                    else
                    {
                        throw new KnownException();
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public bool DeleteRoleFunction(int roleID, string functionID)
        {
            try
            {
                if (string.IsNullOrEmpty(functionID))
                {
                    throw new KnownException("参数无效！");
                }
                using (var db = new BCBackContext())
                {
                    RFAAuthorization au = new RFAAuthorization { RoleID = roleID, FunctionID = functionID };
                    db.RFAAuthorizations.Attach(au);
                    db.RFAAuthorizations.Remove(au);
                    if (1 == db.SaveChanges())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public bool UpdateRoleFunction(int roleID, string functionID, bool available)
        {
            if (string.IsNullOrEmpty(functionID))
            {
                throw new KnownException("更新信息不全！");
            }
            using (var db = new BCBackContext())
            {
                var q = db.RFAAuthorizations.FirstOrDefault(obj => obj.RoleID == roleID && obj.FunctionID == functionID);
                if (null == q)
                {
                    throw new KnownException("该记录不存在！");
                }

                db.RFAAuthorizations.AddOrUpdate(new RFAAuthorization() { RoleID = roleID, FunctionID = functionID, UpdateTime = DateTime.Now });

                return 1 == db.SaveChanges();
            }
        }

        public List<string> GetAllFunctionIDByRoleID(int roleID)
        {
            try
            {
                if (0 > roleID)
                {
                    throw new KnownException("参数无效！");
                }

                var list = new List<string>();
                using (var db = new BCBackContext())
                {
                    list = db.RFAAuthorizations.Where(obj => obj.RoleID == roleID).Select(obj => obj.FunctionID).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<AuthorizationsDto> SetFunction(int roleID, string functionIDs)
        {
            try
            {
                if (0 > roleID || string.IsNullOrEmpty(functionIDs))
                {
                    throw new KnownException("参数无效！");
                }
                //  返回设置完成后的角色功能信息
                var list = new List<AuthorizationsDto>();

                var strS = functionIDs.Split('|');
                var listFuc = strS.ToList();

                using (var db = new BCBackContext())
                {
                    foreach (var funId in listFuc)
                    {
                        //  更新或者添加功能
                        var au = new RFAAuthorization() { RoleID = roleID, FunctionID = funId, UpdateTime = DateTime.Now };
                        db.RFAAuthorizations.AddOrUpdate(au);
                    }

                    ////  删除不再具有的功能
                    //var query = db.RFAAuthorizations.Where(obj => obj.RoleID == roleID).Select(obj => obj.FunctionID).ToList();
                    //foreach (var f in query)
                    //{
                    //    if (!listFuc.Contains(f))
                    //    {
                    //        var del = db.RFAAuthorizations.FirstOrDefault(obj => obj.RoleID == roleID && obj.FunctionID == f);
                    //        db.RFAAuthorizations.Remove(del);
                    //    }
                    //
                    //}

                    //  删除不再具有的功能
                    var del = db.RFAAuthorizations.Where(obj => (obj.RoleID == roleID) && (!functionIDs.Contains(obj.FunctionID))).ToList();
                    foreach (var d in del) db.RFAAuthorizations.Remove(d);

                    if (0 < db.SaveChanges())
                    {
                        list = db.RFAAuthorizations.Where(obj => (obj.RoleID == roleID) && (functionIDs.Contains(obj.FunctionID)))
                                                        .Select(obj => new AuthorizationsDto()
                                                                            {
                                                                                RoleID = obj.RoleID,
                                                                                FunctionID = obj.FunctionID,
                                                                                UpdateTime = obj.UpdateTime
                                                                            }).ToList();
                    }
                }
                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
