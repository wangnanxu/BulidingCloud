using ML.BC.EnterpriseData.Model;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ML.BC.Services.Account.Dtos;

namespace ML.BC.Services
{
    public class EnterpriseAccountService : IEnterpriseAccountService
    {
        public FrontUserDto Get(string username)
        {
            try
            {
                if (string.IsNullOrEmpty(username)) throw new KnownException("用户名称不能为空！");

                using (var db = new BCEnterpriseContext())
                {
                    var user = db.FrontUsers.Where(n => n.Name == username)
                        .Select(obj => new FrontUserDto()
                        {
                            UserID = obj.UserID,
                            Name = obj.Name,
                            EnterpiseID = obj.EnterpiseID,
                            DepartmentID = obj.DepartmentID,
                            Mobile = obj.Mobile,
                            LastIP = obj.LastIP,
                            LastDate = obj.LastDate,
                            Password = obj.Password,
                            UpdateTime = obj.UpdateTime,
                            Closed = obj.Closed,
                            RegistDate = obj.RegistDate,
                            LoginByDesktop = obj.LoginByDesktop,
                            LoginByMobile = obj.LoginByMobile
                        }).FirstOrDefault();
                    return user;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public bool CanLogin(string userCode, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(userCode) || string.IsNullOrEmpty(password)) throw new KnownException("用户名或密码不能为空！");
                using (var db = new BCEnterpriseContext())
                {
                    var userInfo = (from u in db.FrontUsers
                                    where u.UserID == userCode
                                    join ep in db.Enterprises on u.EnterpiseID equals ep.EnterpriseID into tempEP
                                    from tep in tempEP.DefaultIfEmpty()
                                    select new
                                    {
                                        u,
                                        tep
                                    }).FirstOrDefault();
                    if (userInfo == null) throw new KnownException(string.Format("不存在用户名为\"{0}\"的用户！", userCode));

                    CheckUserAndEnterpriseState(userCode, userInfo.u, userInfo.tep);
                    var encodPass = CryptoService.MD5Encrypt(password);
                    if (userInfo.u.Password != encodPass) throw new KnownException("登录失败，你输入的密码错误！");

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool CheckUserAndEnterpriseState(string userCode, FrontUser user, ML.BC.EnterpriseData.Model.Enterprise enterprise)
        {
            if (user == null) throw new KnownException(string.Format("不存在用户名为\"{0}\"的用户！", userCode));

            if (enterprise == null || enterprise.Deleted) throw new KnownException("你所在的企业已经被删除，如有疑问请联系管理员！");

            if (enterprise.Status == 1) throw new KnownException(string.Format("你所在的企业已经被暂停，如有疑问请联系管理员，并提供此企业编码：｛0｝！", enterprise.EnterpriseID));

            if (enterprise.Status == 2) throw new KnownException(string.Format("你所在的企业已经被关闭，如有疑问请联系管理员，并提供此企业编码：｛0｝！", enterprise.EnterpriseID));

            if (user.Closed) throw new KnownException("你的帐号已被关闭，如有疑问请联系管理员！");

            return true;
        }
        public bool CheckUserIsLogin(string userCode, string device)
        {
            using (var db = new BCEnterpriseContext())
            {
                var userInfo = (from u in db.FrontUsers
                                where u.UserID == userCode
                                join uls in db.UserLoginStates on u.UserID equals uls.UserID into tempULS
                                join ep in db.Enterprises on u.EnterpiseID equals ep.EnterpriseID into tempEP
                                select new
                                {
                                    u,
                                    tep = tempEP.DefaultIfEmpty().FirstOrDefault(),
                                    uls = tempULS.FirstOrDefault(n => n.Device == device)
                                }).FirstOrDefault();

                if (userInfo == null) throw new KnownException(string.Format("不存在用户名为\"{0}\"的用户！", userCode));

                CheckUserAndEnterpriseState(userCode, userInfo.u, userInfo.tep);

                if (userInfo.uls == null) throw new KnownException("登录状态丢失，请重新登录。");

                if (userInfo.uls.Device == "PC")
                {
                    var refreshTime = ML.BC.Infrastructure.Common.ConstantData.KEEPSESSIONREQUESTRATE;
                    int.TryParse(System.Configuration.ConfigurationManager.AppSettings["KeepSessionRequestRate"], out refreshTime);
                    if (userInfo.uls.UpdateTime.AddMinutes(refreshTime * 2) < ML.BC.EnterpriseData.Model.Extend.DBTimeHelper.DBNowTime(db))
                        throw new KnownException("登录状态丢失，请重新登录。");
                }

                return true;
            }
        }

        public bool UpdateUserLoginState(string userId, string lastIP, string Device, string LoginToken)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException("userId");
                if (string.IsNullOrEmpty(lastIP)) throw new ArgumentNullException("lastIP");

                using (var db = new BCEnterpriseContext())
                {
                    var user = db.FrontUsers.First(n => n.UserID == userId);
                    user.LastDate = DateTime.Now;
                    user.LastIP = lastIP;
                    db.Entry<FrontUser>(user).State = System.Data.Entity.EntityState.Modified;

                    var updateResult = db.SaveChanges() > 0;

                    var setLoginStateResult = AddUserLoginState(db, new UserLoginStateDto()
                    {
                        Device = Device,
                        LoginIP = lastIP,
                        LoginTime = user.UpdateTime,
                        LoginToken = LoginToken,
                        UserID = user.UserID,
                        UserName = user.Name
                    });

                    LoginLogWrite(db, new UserLoginLog
                    {
                        UserID = user.UserID,
                        UserName = user.Name,
                        IP = lastIP,
                        Device = Device,
                        Time = user.UpdateTime,
                        Description = string.Empty,
                        Status = (int)EnterpriseData.Common.LoginStatus.Login
                    });

                    return updateResult && setLoginStateResult > 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void LoginLogWrite(BCEnterpriseContext db, UserLoginState userLoginState, EnterpriseData.Common.LoginStatus status, string description)
        {
            if (userLoginState==null) return;
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
        public string CreateUser(FrontUserDto userDto)
        {
            try
            {
                if (null == userDto) throw new KnownException("创建用户信息不存在，无法创建！");
                using (var db = new BCEnterpriseContext())
                {
                    if (string.IsNullOrEmpty(userDto.Password))
                    {
                        throw new KnownException("密码不允许为空");
                    }
                    var uid = Ioc.GetService<ML.BC.Services.Common.IUniqeIdGenerator>().GeneratorBackUserID();
                    var user = new FrontUser()
                    {
                        UserID = uid,
                        Name = userDto.Name,
                        Password = CryptoService.MD5Encrypt(userDto.Password),
                        Mobile = userDto.Mobile,
                        RegistDate = DateTime.Now,
                        Closed = userDto.Closed,
                        UpdateTime = DateTime.Now
                    };
                    db.FrontUsers.Add(user);
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
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Account.Dtos.SessionUserDto GetSessionUser(string userId, string device)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) throw new KnownException("用户名称不能为空！");
                using (var db = new BCEnterpriseContext())
                {
                    ClearUserLoginState(db);

                    var user = (from u in db.FrontUsers
                                where u.UserID == userId
                                join ls in db.UserLoginStates on u.UserID equals ls.UserID
                                where ls.Device == device
                                join ep in db.Enterprises on u.EnterpiseID equals ep.EnterpriseID into tempEP
                                join ur in db.UserRoles on u.UserID equals ur.UserID into tempUR
                                select new
                                {
                                    u,
                                    ls,
                                    ep = tempEP.DefaultIfEmpty().FirstOrDefault(),
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

                    if (user == null) throw new KnownException("用户名不存在或用户当前登录状态丢失请重新登录。");

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

                    var sessionUserDto = new Account.Dtos.SessionUserDto
                    {
                        UserID = user.u.UserID,
                        UserName = user.u.Name,
                        LastLoginTime = user.u.LastDate ?? DateTime.Now,
                        LastIP = user.u.LastIP,
                        Picture = UriExtensions.GetFullUrl(user.u.Picture),
                        Device = user.ls.Device,
                        Token = user.ls.LoginToken,
                        EnterpriseID = user.u.EnterpiseID,
                        EnterpriseName = user.ep == null ? string.Empty : user.ep.Name,
                        DepartmentID = user.u.DepartmentID,
                        RoleIDs = user.roles.ToArray(),
                        FunctionIDs = functionIds.ToArray()
                    };
                    return sessionUserDto;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public FrontUserDto GetById(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) throw new KnownException("用户ID不能为空！");
                using (var db = new BCEnterpriseContext())
                {
                    var user = db.FrontUsers.Where(n => n.UserID == userId)
                        .Select(obj => new FrontUserDto
                        {
                            UserID = obj.UserID,
                            Name = obj.Name,
                            EnterpiseID = obj.EnterpiseID,
                            DepartmentID = obj.DepartmentID,
                            Mobile = obj.Mobile,
                            LastIP = obj.LastIP,
                            LastDate = obj.LastDate,
                            Password = obj.Password,
                            UpdateTime = obj.UpdateTime,
                            Closed = obj.Closed,
                            RegistDate = obj.RegistDate,
                            LoginByDesktop = obj.LoginByDesktop,
                            LoginByMobile = obj.LoginByMobile
                        }).FirstOrDefault();
                    return user;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public List<FrontUserDto> GetList(int pageNumber, int pageSize, out int amount)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var list = db.FrontUsers.Select(obj => new FrontUserDto
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

        public List<FrontUserDto> SearchUserByName(string nameKeyword, int pageNumber, int pageSize, out int amount)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var list = db.FrontUsers.Where(obj => (nameKeyword ?? "") == "" || obj.Name.Contains(nameKeyword));
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
                    return list.OrderBy(x => x.UserID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).Select(obj => new FrontUserDto
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
                using (var db = new BCEnterpriseContext())
                {
                    return db.FrontUsers.Count();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int GetAmount(System.Linq.Expressions.Expression<Func<FrontUser, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) throw new KnownException("用户ID不能为空，无法删除！");
                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.FrontUsers.FirstOrDefault(obj => obj.UserID.Equals(userId));
                    db.FrontUsers.Attach(temp);
                    db.FrontUsers.Remove(temp);
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool UpdateUser(FrontUserDto user)
        {
            try
            {
                if (null == user) throw new KnownException("用户信息为空，不能更新！");
                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.FrontUsers.First(x => x.UserID == user.UserID);
                    if (null == temp)
                        throw new KnownException("该对象不存在");
                    if (!string.IsNullOrEmpty(user.Password))
                        temp.Password = CryptoService.MD5Encrypt(user.Password);
                    temp.Closed = user.Closed;
                    temp.Mobile = user.Mobile;
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

        #region 用户登录状态

        public List<UserLoginStateDto> GetUserLoginStateList(SessionUserDto SNUser, bool ShowAll, string UserName, int pageNumber, int pageSize, out int amount)
        {
            using (var db = new BC.EnterpriseData.Model.BCEnterpriseContext())
            {
                ClearUserLoginState(db);

                string EnterpriseID = SNUser.EnterpriseID;
                int DepartmentID = SNUser.DepartmentID == null ? 0 : SNUser.DepartmentID.Value;
                List<FrontUser> FUserList = new List<FrontUser>();
                List<string> FUserIds = new List<string>();
                var queryfusers = db.FrontUsers.Where(m => m.EnterpiseID == EnterpriseID);
                if (queryfusers.Count() > 0)
                {
                    FUserList = queryfusers.ToList();//当前企业下的所有用户
                    FUserIds = FUserList.Select(m => m.UserID).ToList();
                }

                var list = db.UserLoginStates.Where(m => FUserIds.Contains(m.UserID));
                if (!string.IsNullOrEmpty(UserName))
                {
                    list = list.Where(m => m.UserName.Contains(UserName));
                }
                if (ShowAll == false)
                {
                    var depList = GetSubDepartmentList(DepartmentID, db);
                    List<int> depIdList = new List<int>();
                    depIdList.Add(DepartmentID);
                    if (depList != null && depList.Count > 0)
                    {
                        depIdList.AddRange(depList.Select(m => m.DepartmentID));//子部门id
                    }

                    var subUsers = FUserList.Where(m => depIdList.Contains(m.DepartmentID.Value));//子部门用户
                    if (subUsers != null && subUsers.Count() > 0)
                    {
                        FUserIds = subUsers.Select(m => m.UserID).ToList();
                        list = list.Where(m => FUserIds.Contains(m.UserID));
                    }
                    else
                    {
                        amount = 0;
                        return new List<UserLoginStateDto>();
                    }

                }
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

                return list.OrderBy(x => x.UserLoginStateID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).Select(obj => new UserLoginStateDto()
                {
                    UserID = obj.UserID,
                    Device = obj.Device,
                    LoginIP = obj.LoginIP,
                    LoginTime = obj.LoginTime,
                    LoginToken = obj.LoginToken,
                    UserLoginStateID = obj.UserLoginStateID,
                    UserName = obj.UserName
                }).ToList();
            }
        }
        /// <summary>
        /// 获取一个部门下的所有子部门
        /// </summary>
        /// <param name="DepartmentId">部门id</param>
        /// <param name="db"></param>
        /// <returns></returns>

        public bool DeleteUserLoginState(long UserLoginStateID, string reason = "")
        {
            if (UserLoginStateID <= 0) return false;
            using (var db = new BC.EnterpriseData.Model.BCEnterpriseContext())
            {
                var _m = db.UserLoginStates.Where(m => m.UserLoginStateID == UserLoginStateID).FirstOrDefault();
                if (_m == null) return true;
                LoginLogWrite(db, _m, EnterpriseData.Common.LoginStatus.Logout, reason);
                db.UserLoginStates.Remove(_m);
                int i = db.SaveChanges();
                return true;
            }
        }

        public bool DeleteUserLoginState(string UserID, string Device = "", string reason = "")
        {
            if (string.IsNullOrEmpty(UserID)) return false;
            using (var db = new BC.EnterpriseData.Model.BCEnterpriseContext())
            {
                var _m = db.UserLoginStates.Where(m => m.UserID == UserID && (string.IsNullOrEmpty(Device) || m.Device == Device)).FirstOrDefault();
                if (_m != null)
                {
                    LoginLogWrite(db, _m, EnterpriseData.Common.LoginStatus.Logout, reason);
                    db.UserLoginStates.Remove(_m);
                }
                int i = db.SaveChanges();
                return i > 0 ? true : false;
            }
        }

        public bool RefreshUserLoginState(string userId, string device = "")
        {
            using (var db = new BC.EnterpriseData.Model.BCEnterpriseContext())
            {
                var _m = db.UserLoginStates.FirstOrDefault(m => m.UserID == userId && (string.IsNullOrEmpty(device) || m.Device == device));
                if (_m != null)
                {
                    _m.UpdateTime = DateTime.Now;
                }
                return db.SaveChanges() > 0;
            }
        }
        private List<Department> GetSubDepartmentList(int DepartmentId, BCEnterpriseContext db)
        {
            List<Department> list = new List<Department>();
            var sublist = db.Departments.Where(m => m.ParentID == DepartmentId);
            if (sublist != null && sublist.Count() > 0)
            {
                list.AddRange(sublist);
                foreach (var item in sublist.ToList())
                {
                    var temp = GetSubDepartmentList(DepartmentId, db);
                    if (temp.Count > 0)
                    {
                        list.AddRange(temp);
                    }
                }
            }
            return list;
        }

        private long AddUserLoginState(BCEnterpriseContext db, Account.Dtos.UserLoginStateDto model)
        {
            if (model == null) throw new ArgumentNullException("UserLoginStateDto");

            var temp = db.UserLoginStates.FirstOrDefault(m => m.UserID == model.UserID && m.Device == model.Device) ?? new UserLoginState();
            temp.UserID = model.UserID;
            temp.Device = model.Device;
            temp.UserName = model.UserName;
            temp.LoginToken = model.LoginToken;
            temp.LoginIP = model.LoginIP;
            temp.LoginTime = DateTime.Now;
            if (temp.UserLoginStateID == 0)
            {
                db.UserLoginStates.Add(temp);
            }
            else
            {
                db.Entry<UserLoginState>(temp).State = System.Data.Entity.EntityState.Modified;
            }
            db.SaveChanges();
            model.UserLoginStateID = temp.UserLoginStateID;

            return temp.UserLoginStateID;
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
        #endregion



    }
}
