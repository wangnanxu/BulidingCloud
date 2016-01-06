using ML.BC.EnterpriseData.Model;
using ML.BC.BCBackData.Model;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ML.BC.Services
{
    public class EnterpriseRoleManagementService:IEnterpriseRoleManagementService
    {
        public List<EnterpriseRoleDto> GetEnterpriseRoleList(string enterpriseName, string roleName, int pageSize, int pageNumber, out int amount)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    if (string.IsNullOrEmpty(enterpriseName))
                    {
                        enterpriseName = "";
                    }
                    if (string.IsNullOrEmpty(roleName))
                    {
                        roleName = "";
                    }
                    var entlistId = db.Enterprises.Where(x => x.Name.Contains(enterpriseName)).Select(n=>n.EnterpriseID);
                    var temp = db.RFARoles.Where(x => entlistId.Contains(x.OwnerID)&&x.OwnerID != null || string.IsNullOrEmpty(enterpriseName));
                    var list = temp.Where(obj => obj.Name.Contains(roleName));
//                    list = list.Union(db.RFARoles.Where(x=>string.IsNullOrEmpty(x.OwnerID)));
                    //var rolelist = db.RFARoles.Where(x => x.Name.Contains(roleName));
                    //var entlist = db.
                    int pagecount;
                    amount = list.Count();
                    if (pageSize > 0)
                    {
                        // 获取总共页数
                        pagecount = (list.Count() + pageSize - 1) / pageSize;
                    }
                    else
                    {
                        pagecount = 0;
                    }
                    //页码判断，小于1则为1，大于最大页码则为最大页码
                    if (pageNumber > pagecount)
                        pageNumber = pagecount;
                    if (pageNumber < 1)
                        pageNumber = 1;
                    var newlist = list.Select(obj => new EnterpriseRoleDto
                    {
                        RoleID = obj.RoleID,
                        Name = obj.Name,
                        OwnerID = obj.OwnerID,
                        Description = obj.Description,
                        Available = obj.Available,
                        EnterpriseName = ""
                    }).OrderBy(x => x.RoleID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                    foreach(var v in newlist)
                    {
                        var ntemp = db.Enterprises.FirstOrDefault(x => x.EnterpriseID.Equals(v.OwnerID));
                        if (null != ntemp)
                        {
                            v.EnterpriseName = ntemp.Name;
                        }
                    }
                    return newlist;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public EnterpriseRoleDto GetEnterpriseRoleByRoleID(int RoleID)
        { 
            try
            {
                using(var db = new BCEnterpriseContext())
                {
                    return db.RFARoles.Where(x => x.RoleID == RoleID).Select(obj=>new EnterpriseRoleDto
                    {
                        RoleID = obj.RoleID,
                        Name = obj.Name,
                        OwnerID = obj.OwnerID,
                        Description = obj.Description,
                        Available = obj.Available
                    }).ToList().First();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private bool SetEnterpriseRoleFunction(int roleId,string functionId)
        {
            try
            {
                if (string.IsNullOrEmpty(functionId))
                {
                    return false;
                }
                using(var db = new BCEnterpriseContext())
                {
                    var strS = functionId.Split('|');
                    var list = strS.ToList();
                    int count = list.Count();
                    var temp = db.RFAAuthorizations.Where(x => x.RoleID == roleId);
                    var query = temp.Where(n=>!n.Deleted).Select(x=>x.FunctionID);
                    var tempcount = temp.Count();
                    //数据库有0条关于该角色的记录
                    if(0==tempcount)
                    {
                        foreach(var funId in list)
                        {
                            //添加角色功能
                            var ao = new ML.BC.EnterpriseData.Model.RFAAuthorization() { RoleID = roleId, FunctionID = funId, UpdateTime = DateTime.Now ,Deleted = false};
                            db.RFAAuthorizations.Add(ao);
                        }
                    }
                    //数据库关于该角色的记录大于0条
                    if(0<tempcount)
                    {
                        foreach(var funid in query)
                        {
                            //判断是否已经存在相同记录
                           if(!list.Contains(funid))
                           {
                               //删除已经更改后已经不存在的角色功能记录
                               var temp1 = db.RFAAuthorizations.FirstOrDefault(obj => obj.RoleID == roleId && obj.FunctionID.Equals(funid));
                               temp1.Deleted = true;
                           }
                        }
                        foreach(var funid in list)
                        {
                            var delflag = db.RFAAuthorizations.FirstOrDefault(x => x.RoleID == roleId && x.FunctionID == funid);
                            if(!query.Contains(funid))
                            {
                                //添加更改后新增的功能项
                                if(null == delflag)
                                {
                                    var ao = new ML.BC.EnterpriseData.Model.RFAAuthorization() { RoleID = roleId, FunctionID = funid, UpdateTime = DateTime.Now, Deleted = false };
                                    db.RFAAuthorizations.Add(ao);
                                }
                                else
                                {
                                    delflag.Deleted = false;
                                }
                            }
                        }
                    }
                    if(db.SaveChanges()>0)
                    {
                        ForceLogout(db, roleId, EnterpriseData.Common.LoginStatus.Logout, "用户权限更改，系统强制登出");
                    }
                }
               
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        private void ForceLogout(BCEnterpriseContext db, int roleId, EnterpriseData.Common.LoginStatus status, string description)
        {
            var loginStateList = (from ur in db.UserRoles
                                  where ur.RoleID == roleId && !ur.Deleted
                                  join uls in db.UserLoginStates on ur.UserID equals uls.UserID
                                  select uls).ToList();
            var dbNowTime = ML.BC.EnterpriseData.Model.Extend.DBTimeHelper.DBNowTime(db);
            loginStateList.ForEach(n =>
            {
                var loginLog = new UserLoginLog
                {
                    UserID = n.UserID,
                    UserName = n.UserName,
                    IP = n.LoginIP,
                    Device = n.Device,
                    Time = dbNowTime,
                    Description = description,
                    Status = (int)status
                };
                db.UserLoginLogs.Add(loginLog);
                db.UserLoginStates.Remove(n);
            });
            db.SaveChanges();
        }

        public string AddEnterpriseRole(EnterpriseRoleDto enterpriseRole)
        {
            using (var db = new BCEnterpriseContext())
            {
                if (db.RFARoles.Any(n => n.Name == enterpriseRole.Name&&(n.OwnerID??"").Equals(enterpriseRole.OwnerID??"")&&n.RoleID != enterpriseRole.RoleID))
                throw new KnownException("角色名冲突，请更改角色名后重试");
                var temp = db.Enterprises.FirstOrDefault(x => x.EnterpriseID.Equals(enterpriseRole.OwnerID)||string.IsNullOrEmpty(enterpriseRole.OwnerID));
                if(null == temp)
                        throw new KnownException("企业ID不合法");
                var role = new ML.BC.EnterpriseData.Model.RFARole
                {
                    Name = enterpriseRole.Name,
                    OwnerID = enterpriseRole.OwnerID,
                    Description = enterpriseRole.Description,
                    Available = enterpriseRole.Available
                };
                //  事务处理Authorizations表添加成功和RFARole表添加成功需要同时满足。
                using (TransactionScope transaction = new TransactionScope())
                {
                    db.RFARoles.Add(role);
                    db.SaveChanges();
                    var roleId = db.RFARoles.Where(x => x.OwnerID.Equals(enterpriseRole.OwnerID)&&x.Name.Equals(enterpriseRole.Name)).Select(n => n.RoleID);
                    if(!string.IsNullOrEmpty(enterpriseRole.FunctionIDs))
                    {
                        SetEnterpriseRoleFunction(roleId.First(), enterpriseRole.FunctionIDs);
                    }
                    if(0<=db.SaveChanges())
                    {
                        transaction.Complete();
                        return role.Name;
                    }
                    else
                    {
                        throw new KnownException("保存角色功能失败");
                    }
                }
            }
        }
        public bool DeleteEnterpriseRole(int roleId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.RFARoles.FirstOrDefault(obj => obj.RoleID.Equals(roleId));
                    if (null == temp)
                        throw new KnownException("该角色不存在");
                    var fun = db.RFAAuthorizations.Where(x => x.RoleID == roleId && !x.Deleted);
                    var userrole = db.UserRoles.Where(x=>x.RoleID == roleId && !x.Deleted);
                    var project = db.Projects.FirstOrDefault(x => x.Roles.Contains(roleId.ToString()) && !x.Deleted);
                    if (0 < userrole.Count() || project != null)
                        throw new KnownException("该角色已被占用，无法删除");
                    else
                    {
                        db.RFARoles.Remove(temp);
                        foreach (var f in fun)
                        {
                            db.RFAAuthorizations.Attach(f);
                            f.Deleted = true;
                        }
                    }
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            } 
        }
        public bool UpdateEnterpriseRole(EnterpriseRoleDto enterpriseRole)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        var temp = db.RFARoles.First(x => x.RoleID == enterpriseRole.RoleID);
                        if (null == temp)
                            throw new KnownException("该对象不存在");
                        if (db.RFARoles.Any(n => n.Name == enterpriseRole.Name && (n.OwnerID??"").Equals(enterpriseRole.OwnerID??"")&&enterpriseRole.RoleID != n.RoleID))
                            throw new KnownException("角色名冲突，请更改角色名后重试");
                        temp.Name = enterpriseRole.Name;
                        temp.Available = enterpriseRole.Available;
                        temp.Description = enterpriseRole.Description;
                        temp.OwnerID = enterpriseRole.OwnerID;
                        if (!string.IsNullOrEmpty(enterpriseRole.FunctionIDs))
                        {
                            SetEnterpriseRoleFunction(enterpriseRole.RoleID, enterpriseRole.FunctionIDs);
                        }
                        if (0<=db.SaveChanges())
                        {
                            transaction.Complete();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //企业端
        public List<EnterpriseRoleDto> GetEnterpriseRoleByEnterpriseID(string enterpriseId)
        {
            try
            {
                using(var db = new BCEnterpriseContext())
                {
                    if (string.IsNullOrEmpty(enterpriseId))
                        {
                            throw new KnownException("企业ID不允许为空");
                        }
                        var name = db.Enterprises.FirstOrDefault(x => x.EnterpriseID.Equals(enterpriseId)).Name;
                        return db.RFARoles.Where(x => (x.OwnerID.Equals(enterpriseId) || string.IsNullOrEmpty(x.OwnerID))&&x.Available).Select(obj => new EnterpriseRoleDto
                        {
                            RoleID = obj.RoleID,
                            Name = obj.Name,
                            OwnerID = obj.OwnerID,
                            Description = obj.Description,
                            Available = obj.Available,
                            EnterpriseName = name
                        }).ToList();
                    
                }
            }
            catch(Exception e)
            {
                throw e;
            }

        }


        public List<EnterpriseRoleDto> GetListToEP(string roleName,string enterpriseId, int pageSize, int pageNumber, out int amount)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    if (string.IsNullOrEmpty(roleName))
                    {
                        roleName = "";
                    }
                    var list = db.RFARoles.Where(obj => obj.Name.Contains(roleName)&&(obj.OwnerID.Equals(enterpriseId) || string.IsNullOrEmpty(obj.OwnerID)));
                    int pagecount;
                    amount = list.Count();
                    if (pageSize > 0)
                    {
                        // 获取总共页数
                        pagecount = (list.Count() + pageSize - 1) / pageSize;
                    }
                    else
                    {
                        pagecount = 0;
                    }
                    //页码判断，小于1则为1，大于最大页码则为最大页码
                    if (pageNumber > pagecount)
                        pageNumber = pagecount;
                    if (pageNumber < 1)
                        pageNumber = 1;
                    var newlist = list.Select(obj => new EnterpriseRoleDto
                    {
                        RoleID = obj.RoleID,
                        Name = obj.Name,
                        OwnerID = obj.OwnerID,
                        Description = obj.Description,
                        Available = obj.Available,
                        EnterpriseName = ""
                    }).OrderBy(x => x.RoleID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                    foreach (var v in newlist)
                    {
                        var temp = db.Enterprises.FirstOrDefault(x => x.EnterpriseID.Equals(v.OwnerID));
                        if (null != temp)
                        {
                            v.EnterpriseName = temp.Name;
                        }
                    }
                    return newlist;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string AddEnterpriseRoleToEP(EnterpriseRoleDto enterpriseRole)
        {
            using (var db = new BCEnterpriseContext())
            {
                if (db.RFARoles.Any(n => n.Name == enterpriseRole.Name&& (n.OwnerID??"").Equals(enterpriseRole.OwnerID??"")&&n.RoleID != enterpriseRole.RoleID))
                    throw new KnownException("角色名冲突，请更改角色名后重试");
               var temp = db.Enterprises.FirstOrDefault(x => x.EnterpriseID.Equals(enterpriseRole.OwnerID));
               if (null == temp && !string.IsNullOrEmpty(enterpriseRole.OwnerID))
                    throw new KnownException("企业ID不合法");
               if (string.IsNullOrEmpty(enterpriseRole.OwnerID))
                   throw new KnownException("没有权限添加公用角色");
                var role = new ML.BC.EnterpriseData.Model.RFARole
                {
                    Name = enterpriseRole.Name,
                    OwnerID = enterpriseRole.OwnerID,
                    Description = enterpriseRole.Description,
                    Available = enterpriseRole.Available
                };
                //  事务处理Authorizations表添加成功和RFARole表添加成功需要同时满足。
                using (TransactionScope transaction = new TransactionScope())
                {
                    db.RFARoles.Add(role);
                    db.SaveChanges();
                    var roleId = db.RFARoles.Where(x => x.Name.Equals(enterpriseRole.Name)).Select(n => n.RoleID);
                    if (SetEnterpriseRoleFunction(roleId.First(), enterpriseRole.FunctionIDs))
                    {
                        transaction.Complete();
                        return role.Name;
                    }
                    else
                    {
                        throw new KnownException("保存角色功能失败");
                    }
                }
            }
        }

        public EnterpriseRoleDto GetEnterpriseRoleByRoleIDToEP(int RoleID)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    return db.RFARoles.Where(x => x.RoleID == RoleID).Select(obj => new EnterpriseRoleDto
                    {
                        RoleID = obj.RoleID,
                        Name = obj.Name,
                        OwnerID = obj.OwnerID,
                        Description = obj.Description,
                        Available = obj.Available
                    }).ToList().First();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<EnterpriseRoleDto> GetEnterpriseRoleByEnterpriseIDToEP(string enterpriseId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    if (string.IsNullOrEmpty(enterpriseId))
                    {
                        throw new KnownException("企业ID不允许为空");
                    }
                    var name = db.Enterprises.FirstOrDefault(x => x.EnterpriseID.Equals(enterpriseId)).Name;
                    return db.RFARoles.Where(x => (x.OwnerID.Equals(enterpriseId)||string.IsNullOrEmpty(x.OwnerID))&&x.Available).Select(obj => new EnterpriseRoleDto
                    {
                        RoleID = obj.RoleID,
                        Name = obj.Name,
                        OwnerID = obj.OwnerID,
                        Description = obj.Description,
                        Available = obj.Available,
                        EnterpriseName = name
                    }).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool DeleteEnterpriseRoleToEP(int roleId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.RFARoles.FirstOrDefault(obj => obj.RoleID.Equals(roleId));
                    if (null == temp)
                        throw new KnownException("该角色不存在");
                    if (string.IsNullOrEmpty(temp.OwnerID))
                        throw new KnownException("通用角色无法删除");
                    var fun = db.RFAAuthorizations.Where(x => x.RoleID == roleId && !x.Deleted);
                    var userrole = db.UserRoles.Where(x=>x.RoleID == roleId && !x.Deleted);
                    var project = db.Projects.FirstOrDefault(x=>x.Roles.Contains(roleId.ToString())&&!x.Deleted);
                    if (0 < userrole.Count() || project != null)
                        throw new KnownException("该角色已被占用，无法删除");
                    else
                    {
                        db.RFARoles.Remove(temp);
                        foreach (var f in fun)
                        {
                            db.RFAAuthorizations.Attach(f);
                            f.Deleted = true;
                        }
                    }
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            } 
        }

        public bool UpdateEnterpriseRoleToEP(EnterpriseRoleDto enterpriseRole)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        var temp = db.RFARoles.First(x => x.RoleID == enterpriseRole.RoleID&&!string.IsNullOrEmpty(x.OwnerID));
                        if (null == temp)
                            throw new KnownException("该权限中不存在此对象");
                        if (db.RFARoles.Any(n => n.Name == enterpriseRole.Name&&n.RoleID != enterpriseRole.RoleID&&(n.OwnerID??"") == (enterpriseRole.OwnerID??"")))
                            throw new KnownException("角色名冲突，请更改角色名后重试");
                        if (null == db.Enterprises.Where(x => x.EnterpriseID.Equals(enterpriseRole.OwnerID)))
                            throw new KnownException("企业ID不合法");
                        temp.Name = enterpriseRole.Name.Trim();
                        temp.Available = enterpriseRole.Available;
                        temp.Description = enterpriseRole.Description;
                        temp.OwnerID = enterpriseRole.OwnerID;
                        if (!string.IsNullOrEmpty(enterpriseRole.FunctionIDs))
                        {
                            SetEnterpriseRoleFunction(enterpriseRole.RoleID, enterpriseRole.FunctionIDs);
                        }
                        if (0 <= db.SaveChanges())
                        {
                            transaction.Complete();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public List<string> GetFunctions(int roleID)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    return db.RFAAuthorizations.Where(x => x.RoleID == roleID && x.Deleted == false).Select(n => n.FunctionID).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
