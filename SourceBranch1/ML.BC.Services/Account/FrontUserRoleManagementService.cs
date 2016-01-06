using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model;
using ML.BC.Infrastructure.Exceptions;

namespace ML.BC.Services
{
    public class FrontUserRoleManagementService : IFrontUserRoleManagementService
    {
        public string AddFrontUserRole(string frontUserID, int roleID)
        {
            try
            {
                if (string.IsNullOrEmpty(frontUserID))
                {
                    throw new KnownException("添加用户角色不能为空！");
                }
                using (var db = new BCEnterpriseContext())
                {
                    if (!db.FrontUsers.Any(obj => obj.UserID == frontUserID)) throw new KnownException("不存在相关的用户，无法添加用户角色！");
                    if (!db.RFARoles.Any(obj => obj.RoleID == roleID)) throw new KnownException("角色相关信息不存在，无法添加用户角色！");

                    db.UserRoles.Add(new UserRole() { RoleID = roleID, UserID = frontUserID, UpdateTime = DateTime.Now });
                    if (0 < db.SaveChanges())
                    {
                        return frontUserID;
                    }
                    else
                    {
                        throw new KnownException("用户角色保存数据库失败！");
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public bool DeleteFrontUserRole(string frontUserID, int roleID)
        {
            try
            {
                if (string.IsNullOrEmpty(frontUserID))
                {
                    throw new KnownException("删除的用户角色不能为空！");
                }
                using (var db = new BCEnterpriseContext())
                {
                    var del = db.UserRoles.FirstOrDefault(
                            obj => obj.UserID == frontUserID && obj.RoleID == roleID);
                    if (null == del)
                    {
                        throw new KnownException("不存在该记录，无法删除！");
                    }

                    //DbEntityEntry<UserRole> entry = db.Entry<UserRole>(frontUserRole);
                    //db.UserRoles.Attach(entry.Entity);
                    //db.UserRoles.Remove(entry.Entity);
                    db.UserRoles.Remove(del);
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<int> GetAllEnterpriseRolesByFrontUserId(string frontUserId)
        {
            try
            {
                if (string.IsNullOrEmpty(frontUserId))
                {
                    throw new KnownException("Id不能为空！");
                }
                using (var db = new BCEnterpriseContext())
                {
                    var IDs = db.UserRoles.Where(obj => obj.UserID == frontUserId).Select(obj => obj.RoleID);
                    return db.RFARoles.Where(obj => IDs.Contains(obj.RoleID) && obj.Available).Select(o => o.RoleID).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<UserRolesDto> SetFrontUserRoles(string frontUserID, List<int> enterpriseRoleIDs)
        {
            try
            {
                if (string.IsNullOrEmpty(frontUserID) || null == enterpriseRoleIDs)
                {
                    throw new KnownException("用于设置的参数信息不正确！");
                }

                var list = new List<UserRolesDto>();
                using (var db = new BCEnterpriseContext())
                {
                    if (!db.FrontUsers.Any(obj => obj.UserID == frontUserID)) throw new KnownException("不存在相关的用户，无法更新用户角色！");

                    var isFresh = (from rid in enterpriseRoleIDs
                                   join rl in db.RFARoles on rid equals rl.RoleID into tempRL
                                   from trl in tempRL.DefaultIfEmpty()
                                   where trl == null || !trl.Available
                                   select trl).Any();
                    if (isFresh) throw new KnownException("指定的角色不存在，或已被删除，请刷新后重试！");

                    foreach (var set in enterpriseRoleIDs.Select(roleID => new UserRole() { UserID = frontUserID, RoleID = roleID, UpdateTime = DateTime.Now }))
                    {
                        db.UserRoles.AddOrUpdate(set);
                    }

                    var del = db.UserRoles.Where(obj => (obj.UserID == frontUserID) && (!enterpriseRoleIDs.Contains(obj.RoleID))).ToList();
                    foreach (var d in del) db.UserRoles.Remove(d);

                    if (0 < db.SaveChanges())
                    {
                        list = db.UserRoles.Where(obj => (obj.UserID == frontUserID) && (enterpriseRoleIDs.Contains(obj.RoleID)))
                                                .Select(obj => new UserRolesDto
                                                                   {
                                                                       UserID = obj.UserID,
                                                                       RoleID = obj.RoleID,
                                                                       UpdateTime = DateTime.Now
                                                                   }).ToList();

                        ForceLogout(db, frontUserID, EnterpriseData.Common.LoginStatus.Logout, "用户权限更改，系统强制登出");
                    }

                    return list;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void ForceLogout(BCEnterpriseContext db, string userId, EnterpriseData.Common.LoginStatus status, string description)
        {
            var loginStateList = db.UserLoginStates.Where(n => n.UserID == userId).ToList();
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
    }
}
