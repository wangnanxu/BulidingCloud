using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.BCBackData.Model;
using ML.BC.Infrastructure.Exceptions;
using System.Linq.Expressions;
using System.Transactions;
using ML.BC.Infrastructure;
using ML.BC.Services.Account.Dtos;

namespace ML.BC.Services
{
    public class RolesManagementService : IRolesManagementService
    {
        public int AddRole(RolesDto role)
        {
            try
            {
                if (null == role)
                {
                    throw new KnownException();
                }

                if (!string.IsNullOrEmpty(role.FunctionIDs))
                {
                    using (var db = new BCBackContext())
                    {
                        //  事务处理RoleFunction表添加成功和RFARole表添加成功需要同时满足。
                        using (TransactionScope transaction = new TransactionScope())
                        {
                            db.RFARoles.Add(role);
                            db.SaveChanges();
                            var roleId = db.RFARoles.Where(obj => obj.Name == role.Name).Select(obj => obj.RoleID).ToList();
                            if (0 >= roleId.First())
                            {
                                throw new KnownException("保存角色失败!");
                            }

                            //  db.SaveChange()已在SetFunctin函数执行
                            var result = SetFunction(roleId.First(), role.FunctionIDs);
                            if (null == result)
                            {
                                throw new KnownException("保存角色功能失败！");
                            }
                            else
                            {
                                transaction.Complete();
                                return role.RoleID;
                            }
                        }
                    }
                }

                using (var db = new BCBackContext())
                {
                    db.RFARoles.Add(role);
                    if (1 == db.SaveChanges())
                    {
                        return role.RoleID;
                    }
                    else
                    {
                        throw new KnownException("角色保存失败！");
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private List<AuthorizationsDto> SetFunction(int roleID, string functionIDs)
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

                    //  删除不再具有的功能
                    var query = db.RFAAuthorizations.Where(obj => obj.RoleID == roleID).Select(obj => obj.FunctionID).ToList();
                    foreach (var f in query)
                    {
                        if (!listFuc.Contains(f))
                        {
                            var del = db.RFAAuthorizations.FirstOrDefault(obj => obj.RoleID == roleID && obj.FunctionID == f);
                            db.RFAAuthorizations.Remove(del);
                        }

                    }

                    if (0 < db.SaveChanges())
                    {
                        foreach (var fun in listFuc)
                        {
                            list.Add(db.RFAAuthorizations.FirstOrDefault(obj => obj.RoleID == roleID && obj.FunctionID == fun));
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public bool DeleteRole(int roleId)
        {
            try
            {
                if (0 > roleId)
                {
                    throw new KnownException();
                }

                using (var db = new BCBackContext())
                {
                    var fun = db.RFAAuthorizations.Where(x=>x.RoleID == roleId && !x.Deleted);
                    var role = db.UserRoles.FirstOrDefault(x=>x.RoleID == roleId && !x.Deleted);
                    if (role != null)
                        throw new KnownException("该角色被占用，无法删除");
                    else
                    {
                        foreach(var f in fun)
                        {
                            f.Deleted = true;
                        }
                        role.Deleted = true;
                    }
                    //db.RFARoles.Attach(del);
                    //db.RFARoles.Remove(del);
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public bool UpdateRole(RolesDto role)
        {
            try
            {
                if (null == role || 0 > role.RoleID)
                {
                    throw new KnownException("关键信息缺失，无法更新！");
                }

                using (var db = new BCBackContext())
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        var temp = db.RFARoles.FirstOrDefault(obj => obj.RoleID == role.RoleID);
                        if (null == temp) throw new KnownException("不存在该记录，无法更新");

                        temp.Name = role.Name;
                        temp.OwnerID = role.OwnerID;
                        temp.Description = role.Description;
                        temp.Available = role.Available;
                        db.SaveChanges();

                        var auths = SetFunction(role.RoleID, role.FunctionIDs);
                        if (auths == null || auths.Count <= 0) throw new KnownException("角色功能信息更新失败！");

                        transaction.Complete();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public List<RolesDto> GetRoleList(RolesDto model, int pageSize, int pageIndex, out int count)
        {
            try
            {
                RolesDto m;

                //  为空时，查询所有结果
                if (null == model)
                {
                    m = new RolesDto() { RoleID = 0, Name = "", Description = "" };
                    model = m;

                }

                using (var db = new BCBackContext())
                {
                    var query = db.RFARoles.Where(r =>
                        (model.RoleID > 0 ? r.RoleID == model.RoleID : true) &&
                        (!string.IsNullOrEmpty(model.Name) ? r.Name.Contains(model.Name) : true) &&
                        (!string.IsNullOrEmpty(model.Description) ? r.Description.Contains(model.Description) : true));

                    count = query.Count();
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

                    var list = query
                            .OrderBy(obj => obj.RoleID)
                            .Skip(pageSize * (pageIndex - 1))
                            .Take(pageSize)
                            .Select(obj => new RolesDto()
                            {
                                RoleID = obj.RoleID,
                                Name = obj.Name,
                                OwnerID = obj.OwnerID,
                                Description = obj.Description,
                                Available = obj.Available
                            }).ToList();

                    return list;

                    #region 注释无用代码


                    //  转为List<RoleDto>并返回
                    //int len = list.Count();
                    //List<RolesDto> listDto = new List<RolesDto>();
                    //
                    //
                    //for (int i = 0; i < len; i++)
                    //{
                    //
                    //    listDto[i] = list[i];
                    //}
                    //return listDto;

                    //List<RolesDto> listDto = new List<RolesDto>();
                    //foreach (var rfaRole in list)
                    //{
                    //    listDto[outCount] = list[outCount];
                    //    outCount++;
                    //}
                    //
                    //count = outCount;
                    //return listDto;
                    #endregion
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }
}
