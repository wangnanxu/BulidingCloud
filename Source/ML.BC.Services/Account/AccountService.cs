using ML.BC.BCBackData.Model;
using ML.BC.EnterpriseData.Common;
using ML.BC.EnterpriseData.Model;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class AccountService : IAccountService
    {
        public Account.Dtos.UserDto Get(string username)
        {
            using (var db = new BCBackContext())
            {
                var user = db.BackUsers.Where(n => n.Name == username)
                    .Select(n => new Account.Dtos.UserDto
                    {
                        UserID = n.UserID,
                        Name = n.Name,
                        Mobile = n.Mobile,
                        RegistDate = n.RegistDate,
                        LastDate = n.LastDate,
                        LastIP = n.LastIP,
                        Closed = n.Closed,
                        UpdateTime = n.UpdateTime,
                    }).FirstOrDefault();
                return user;
            }
        }

        public bool CanLogin(string userId, string password)
        {
            using (var db = new BCBackContext())
            {
                var user = db.BackUsers.FirstOrDefault(n => n.UserID == userId);
                var encedpass = CryptoService.MD5Encrypt(password);
                return user != null && user.Closed == false
                       && user.Password == encedpass;
            }
        }

        public bool UpdateUserLogin(string userId, string lastIP)
        {
            using (var db = new BCBackContext())
            {
                var user = db.BackUsers.First(n => n.UserID == userId);
                user.LastDate = DateTime.Now;
                user.LastIP = lastIP;
                user.UpdateTime = DateTime.Now;
                db.Entry<BackUser>(user).State = System.Data.Entity.EntityState.Modified;
                return db.SaveChanges() > 0;
            }
        }

        public string CreateUser(Account.Dtos.UserDto userDto)
        {
            using (var db = new BCBackContext())
            {
                if (string.IsNullOrEmpty(userDto.Password))
                {
                    throw new KnownException("密码不允许为空");
                }
                var uid = Ioc.GetService<ML.BC.Services.Common.IUniqeIdGenerator>().GeneratorBackUserID();
                var user = new BackUser
                {
                    UserID = uid,
                    Name = userDto.Name,
                    Password = CryptoService.MD5Encrypt(userDto.Password),
                    Mobile = userDto.Mobile,
                    RegistDate = DateTime.Now,
                    Closed = userDto.Closed,
                    UpdateTime = DateTime.Now
                };
                db.BackUsers.Add(user);
                if (db.SaveChanges() > 0)
                {
                    return user.UserID;
                }
                else
                {
                    return null;
                }
            }
        }

        public Account.Dtos.SessionUserDto GetSessionUser(string userId)
        {
            using (var db = new BCBackContext())
            {
                var user = (from u in db.BackUsers
                            where u.UserID == userId
                            join ur in db.UserRoles on u.UserID equals ur.UserID into tempUR
                            select new
                            {
                                u,
                                roles = tempUR.DefaultIfEmpty().Where(n => !n.Deleted).Select(n => n.RoleID),
                                functions = (from tur in tempUR.DefaultIfEmpty()
                                             where tur.Deleted == false
                                             join r in db.RFARoles on tur.RoleID equals r.RoleID
                                             where r.Available
                                             join rf in db.RFAAuthorizations on tur.RoleID equals rf.RoleID into tempRF
                                             from trf in tempRF.DefaultIfEmpty()
                                             where !trf.Deleted
                                             select trf.FunctionID)
                            }).FirstOrDefault();

                #region get functionIds
                //TODO:get it from the cache
                var functions = db.RFAFunctions.ToList();
                var functionIds = new List<string>();

                Action<string> recursionFunctions = null;
                recursionFunctions = (functionId) =>
                {
                    if (functionIds.Contains(functionId)) return;
                    functionIds.Add(functionId);
                    var tempParent = (from fun in functions
                                      where fun.FunctionID == functionId
                                      join tfun in functions on fun.ParentID equals tfun.FunctionID
                                      select tfun).FirstOrDefault();
                    if (tempParent != null)
                    {
                        recursionFunctions(tempParent.FunctionID);
                    }
                };
                user.functions.ToList().ForEach(recursionFunctions);
                #endregion
                if (null == user)
                    throw new KnownException("不存在该用户");
                var sessionUserDto = new Account.Dtos.SessionUserDto
                {
                    UserID = user.u.UserID,
                    UserName = user.u.Name,
                    LastLoginTime = user.u.LastDate ?? DateTime.Now,
                    LastIP = user.u.LastIP,
                    RoleIDs = user.roles.ToArray(),
                    FunctionIDs = functionIds.ToArray()
                };
                return sessionUserDto;
            }
        }

        public Account.Dtos.UserDto GetById(string userId)
        {
            using (var db = new BCBackContext())
            {
                var user = db.BackUsers.Where(n => n.UserID == userId)
                    .Select(n => new Account.Dtos.UserDto
                    {
                        UserID = n.UserID,
                        Name = n.Name,
                        Mobile = n.Mobile,
                        RegistDate = n.RegistDate,
                        LastDate = n.LastDate,
                        LastIP = n.LastIP,
                        Closed = n.Closed,
                        UpdateTime = n.UpdateTime,
                    }).FirstOrDefault();
                return user;
            }
        }


        public List<Account.Dtos.UserDto> GetList(int pageNumber, int pageSize, out int amount)
        {
            try
            {
                using (var db = new BCBackContext())
                {
                    var list = db.BackUsers.Select(obj => new Account.Dtos.UserDto
                    {
                        UserID = obj.UserID,
                        Name = obj.Name,
                        Mobile = obj.Mobile,
                        LastIP = obj.LastIP,
                        LastDate = obj.LastDate,
                        Password = obj.Password,
                        UpdateTime = obj.UpdateTime,
                        Closed = obj.Closed,
                        RegistDate = obj.RegistDate
                    });
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

                    return list.OrderBy(x => x.UserID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Account.Dtos.UserDto> SearchUserByName(string nameKeyword, int pageNumber, int pageSize, out int amount)
        {
            try
            {
                using (var db = new BCBackContext())
                {
                    var list = db.BackUsers.Where(obj => (nameKeyword ?? "") == "" || obj.Name.Contains(nameKeyword));
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
                    return list.OrderByDescending(x => x.UserID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).Select(obj => new Account.Dtos.UserDto
                    {
                        UserID = obj.UserID,
                        Name = obj.Name,
                        Mobile = obj.Mobile,
                        LastIP = obj.LastIP,
                        LastDate = obj.LastDate,
                        Password = obj.Password,
                        UpdateTime = obj.UpdateTime,
                        Closed = obj.Closed,
                        RegistDate = obj.RegistDate
                    }).ToList();
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public int GetAmount()
        {
            try
            {
                using (var db = new BCBackContext())
                {
                    return db.BackUsers.Count();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool DeleteUser(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return false;
                }
                using (var db = new BCBackContext())
                {
                    var temp = db.BackUsers.FirstOrDefault(obj => obj.UserID.Equals(userId));
                    IQueryable<ML.BC.BCBackData.Model.UserRole> userrolelist;
                    if (null != temp)
                        userrolelist = db.UserRoles.Where(x => x.UserID == temp.UserID);
                    //                    db.BackUsers.Attach(temp);
                    db.BackUsers.Remove(temp);
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool UpdateUser(Account.Dtos.UserDto user)
        {
            try
            {
                using (var db = new BCBackContext())
                {
                    var temp = db.BackUsers.First(x => x.UserID == user.UserID);
                    if (null == temp)
                        throw new KnownException("该对象不存在");
                    temp.Closed = user.Closed;
                    temp.Mobile = user.Mobile;
                    if (!string.IsNullOrEmpty(user.Password))
                        temp.Password = CryptoService.MD5Encrypt(user.Password);
                    temp.Name = user.Name;
                    temp.UpdateTime = DateTime.Now;
                    return 0 < db.SaveChanges();

                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public List<Account.Dtos.UserLoginStateDto> GetUserLoginStateList(string UserName, int pageNumber, int pageSize, out int amount)
        {
            try
            {
                if (string.IsNullOrEmpty(UserName))
                    UserName = "";
                using (var db = new BCEnterpriseContext())
                {
                    ClearUserLoginState(db);

                    var list = db.UserLoginStates.Where(n => n.UserName.Contains(UserName))
                        .Select(obj => new Account.Dtos.UserLoginStateDto
                        {
                            UserID = obj.UserID,
                            Device = obj.Device,
                            LoginIP = obj.LoginIP,
                            LoginTime = obj.LoginTime,
                            LoginToken = obj.LoginToken,
                            UserLoginStateID = obj.UserLoginStateID,
                            UserName = obj.UserName
                        });
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
                    return list.OrderByDescending(x => x.UserID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool ClearUserLoginState(BCEnterpriseContext db)
        {
            try
            {
                int refreshRate;
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["KeepSessionRequestRate"], out refreshRate);
                refreshRate = refreshRate > 0 ? refreshRate : ML.BC.Infrastructure.Common.ConstantData.KEEPSESSIONREQUESTRATE;
                var refreshTime = ML.BC.EnterpriseData.Model.Extend.DBTimeHelper.DBNowTime(db).AddMinutes(0 - refreshRate * 2);

                var list = db.UserLoginStates.Where(n => n.Device == "PC" && n.UpdateTime < refreshTime).ToList();
                list.ForEach(n =>
                {
                    LoginLogWrite(db, n, EnterpriseData.Common.LoginStatus.TimeOut, "登录超时");
                });

                var result = db.ExcuteNonQuerySql(
                    new string[] { "UserLoginState" },
                    string.Format("DELETE FROM [UserLoginState] WHERE [Device]='PC' AND [UpdateTime]<'{0}'",
                    refreshTime));
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
            }
        }
        private void LoginLogWrite(BCEnterpriseContext db, UserLoginState userLoginState, EnterpriseData.Common.LoginStatus status, string description)
        {
            if (userLoginState == null) return;
            var loginLog = new UserLoginLog
            {
                UserID = userLoginState.UserID,
                UserName = userLoginState.UserName,
                Device = userLoginState.Device,
                IP = userLoginState.LoginIP,
                Status = (int)status,
                Description = description
            };
            if (status == EnterpriseData.Common.LoginStatus.Login && userLoginState.LoginTime.HasValue)
            {
                loginLog.Time = userLoginState.LoginTime.Value;
            }
            else
            {
                loginLog.Time = ML.BC.EnterpriseData.Model.Extend.DBTimeHelper.DBNowTime(db);
            }

            LoginLogWrite(db, loginLog);
        }
        private void LoginLogWrite(BCEnterpriseContext db, UserLoginLog loginLog)
        {
            if (loginLog == null) return;
            db.UserLoginLogs.Add(loginLog);
            db.SaveChanges();
        }

        public bool DeleteUserLoginState(string userId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var user = db.UserLoginStates.FirstOrDefault(x => x.UserID.Equals(userId));
                    db.UserLoginStates.Remove(user);
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public int GetAmount(System.Linq.Expressions.Expression<Func<BackUser, bool>> filter)
        {
            try
            {
                using (var db = new BCBackContext())
                {
                    return db.BackUsers.Count(filter);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public List<UserLoginLogDto> SearchUserLoginLog(DateTime? beginTime, DateTime? endTime, string userName, string enterpriseName, EnterpriseData.Common.LoginStatus status, int pageNumber, int pageSize, out int amount)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var temp = new Department()
                    {
                        Name = "",
                    };
                    var entlist = db.Enterprises.Where(x => x.Name.Contains(enterpriseName) || (enterpriseName ?? "") == "").Select(n => n.EnterpriseID).ToList();
                    var userlist = db.FrontUsers.Where(x => entlist.Contains(x.EnterpiseID) && (x.Name.Contains(userName) || (userName ?? "") == "")).Select(n => new EnterpriseUsers
                    {
                        DepartmentName = "",
                        EnterpriseName = "",
                        UserID = n.UserID,
                        UserName = n.Name,
                        DepartmentID = n.DepartmentID,
                        EnterpriseID = n.EnterpiseID
                    }).ToList();
                    foreach (var user in userlist)
                    {
                        user.DepartmentName = (db.Departments.FirstOrDefault(m => m.DepartmentID == (user.DepartmentID ?? -5)) ?? temp).Name;
                        user.EnterpriseName = db.Enterprises.FirstOrDefault(m => m.EnterpriseID == user.EnterpriseID).Name;
                    }
                    var uidlist = userlist.Select(x => x.UserID).ToList();
                    var list = db.UserLoginLogs.Where(x => x.Time < (endTime ?? DateTime.MaxValue)
                        && x.Time > (beginTime ?? DateTime.MinValue)
                        && uidlist.Contains(x.UserID)
                        && ((int)status == x.Status || status == EnterpriseData.Common.LoginStatus.GetAll));
                    int pagecount;
                    amount = list.Count();
                    if (pageSize > 0)
                    {
                        pagecount = (list.Count() + pageSize - 1) / pageSize;
                    }
                    else
                    {
                        pagecount = 0;
                    }
                    if (pageNumber > pagecount)
                        pageNumber = pagecount;
                    if (pageNumber < 1)
                        pageNumber = 1;
                    var relist = list.OrderByDescending(x => x.Time).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList().Select(m => new UserLoginLogDto
                    {
                        UserLoginLogID = m.UserLoginLogID,
                        EnterpriseName = userlist.FirstOrDefault(x => x.UserID == m.UserID).EnterpriseName,
                        DepartmentName = userlist.FirstOrDefault(x => x.UserID == m.UserID).DepartmentName,
                        UserID = m.UserID,
                        Status = (LoginStatus)m.Status,
                        Description = m.Description,
                        Time = m.Time,
                        UserName = userlist.FirstOrDefault(x => x.UserID == m.UserID).UserName,
                        IP = m.IP,
                        Device = m.Device
                    }).ToList();
                    return relist;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
