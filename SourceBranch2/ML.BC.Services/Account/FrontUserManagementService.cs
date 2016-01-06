using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading;
using System.Transactions;
using System.Web.UI.WebControls;
using ML.BC.EnterpriseData.Model;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure;
using ML.BC.Services.Common;

namespace ML.BC.Services
{
    public class FrontUserManagementService : IFrontUserManagementService, IEnterpriseUserManagementService
    {
        public bool DeleteFrontUser(string frontUserId)
        {
            try
            {
                if (string.IsNullOrEmpty(frontUserId))
                {
                    throw new KnownException("Id不能为空！");
                }
                using (var db = new BCEnterpriseContext())
                {
                    var del = db.FrontUsers.FirstOrDefault(x => x.UserID == frontUserId);
                    IQueryable<UserRole> delrole;
                    if (null != del)
                    {
                        delrole = db.UserRoles.Where(x => x.UserID == del.UserID);
                        foreach (var roleuser in delrole)
                        {
                            db.UserRoles.Remove(roleuser);
                        }
                        del.Closed = true;
                    }
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public string AddFrontUser(FrontUserDto frontUserDto)
        {
            try
            {
                if (null == frontUserDto)
                {
                    throw new KnownException("用户信息为空，无法添加！");
                }
                if (string.IsNullOrEmpty(frontUserDto.EnterpiseID) || string.IsNullOrEmpty(frontUserDto.Password) || null == frontUserDto.UserRoleIDs)
                {
                    throw new KnownException("关键信息缺失，无法新增用户！");
                }

                var makeId = Ioc.GetService<IUniqeIdGenerator>().GeneratorFrontUserID();
                frontUserDto.UserID = makeId;
                frontUserDto.Password = CryptoService.MD5Encrypt(frontUserDto.Password);
                frontUserDto.RegistDate = DateTime.Now;
                frontUserDto.LastDate = DateTime.Now;
                frontUserDto.UpdateTime = DateTime.Now;

                using (var db = new BCEnterpriseContext())
                {
                    if (!db.Enterprises.Any(obj => obj.EnterpriseID == frontUserDto.EnterpiseID && !obj.Deleted)) throw new KnownException("没有相关企业信息或企业已被禁用！,不能添加用户，请尝试刷新后重试！");
                    if (null != frontUserDto.DepartmentID
                        && (!db.Departments.Any(obj => obj.EnterpriseID == frontUserDto.EnterpiseID && obj.DepartmentID == frontUserDto.DepartmentID && !obj.Deleted)))
                    {
                        throw new KnownException("此企业下不存在该部门信息或已被删除，无法添加用户，请尝试刷新后重试！");
                    }
                    if (frontUserDto.UserRoleIDs.Count == 0) throw new KnownException("必须为用户指定角色");
                    var r = (db.RFARoles.Where(o => frontUserDto.UserRoleIDs.Contains(o.RoleID) ? o.Available : true)).Select(o => o.RoleID).ToList();
                    if (!frontUserDto.UserRoleIDs.All(o => (r.Contains(o)))) throw new KnownException("指定的角色不存在，或已被删除，请尝试刷新后重试！");
                    using (TransactionScope transaction = new TransactionScope())
                    {
                        db.FrontUsers.Add(frontUserDto);
                        db.SaveChanges();
                        var result = SetFrontUserRoles(frontUserDto.UserID, frontUserDto.UserRoleIDs);
                        if (!result)
                        {
                            throw new KnownException("保存用户角色失败！");
                        }
                        else
                        {
                            transaction.Complete();
                            return frontUserDto.UserID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private static bool SetFrontUserRoles(string frontUserID, List<int> enterpriseRoleIDs)
        {
            try
            {
                if (string.IsNullOrEmpty(frontUserID) || null == enterpriseRoleIDs)
                {
                    throw new KnownException("用于设置的参数信息不正确！");
                }

                using (var db = new BCEnterpriseContext())
                {
                    if (!db.FrontUsers.Any(obj => obj.UserID == frontUserID)) throw new KnownException("不存在相关的用户，无法更新用户角色！");

                    foreach (var set in enterpriseRoleIDs.Select(roleID => new UserRole() { UserID = frontUserID, RoleID = roleID, UpdateTime = DateTime.Now }))
                    {
                        db.UserRoles.AddOrUpdate(set);
                    }

                    var del = db.UserRoles.Where(obj => (obj.UserID == frontUserID) && (!enterpriseRoleIDs.Contains(obj.RoleID))).ToList();
                    foreach (var d in del) db.UserRoles.Remove(d);

                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public bool UpdateFrontUser(FrontUserDto frontUserDto)
        {
            try
            {
                if (null == frontUserDto)
                {
                    throw new KnownException("用户信息为空，无法更新！");
                }
                if (string.IsNullOrEmpty(frontUserDto.EnterpiseID) || null == frontUserDto.UserRoleIDs)
                {
                    throw new KnownException("关键信息缺失，无法更新用户！");
                }

                using (var db = new BCEnterpriseContext())
                {
                    if (!db.Enterprises.Any(obj => obj.EnterpriseID == frontUserDto.EnterpiseID && !obj.Deleted)) throw new KnownException("没有相关企业信息或企业已被禁用！,不能更新用户，请尝试刷新后重试！");
                    if (null != frontUserDto.DepartmentID
                        && (!db.Departments.Any(obj => obj.EnterpriseID == frontUserDto.EnterpiseID && obj.DepartmentID == frontUserDto.DepartmentID && !obj.Deleted)))
                    {
                        throw new KnownException("此企业下不存在该部门信息或已被删除，无法更新用户，请尝试刷新后重试！");
                    }
                    if (frontUserDto.UserRoleIDs.Count == 0) throw new KnownException("必须为用户指定角色");
                    var r = (db.RFARoles.Where(o => frontUserDto.UserRoleIDs.Contains(o.RoleID) ? o.Available : true)).Select(o => o.RoleID).ToList();
                    if (!frontUserDto.UserRoleIDs.All(o => (r.Contains(o)))) throw new KnownException("指定的角色不存在，或已被删除，请尝试刷新后重试！");

                    using (TransactionScope transaction = new TransactionScope())
                    {
                        var query = db.FrontUsers.FirstOrDefault(obj => obj.UserID == frontUserDto.UserID);
                        if (!db.FrontUsers.Any(obj => obj.UserID == frontUserDto.UserID)) throw new KnownException("不存在该记录无法更新！");
                        if (!string.IsNullOrEmpty(frontUserDto.Password))
                        {
                            query.Password = CryptoService.MD5Encrypt(frontUserDto.Password);
                        }
                        query.Name = frontUserDto.Name;
                        query.EnterpiseID = frontUserDto.EnterpiseID;
                        query.DepartmentID = frontUserDto.DepartmentID;
                        query.Mobile = frontUserDto.Mobile;
                        query.UpdateTime = DateTime.Now;
                        query.Closed = frontUserDto.Closed;
                        db.SaveChanges();

                        var result = SetFrontUserRoles(frontUserDto.UserID, frontUserDto.UserRoleIDs);
                        if (!result)
                        {
                            throw new KnownException("更新用户角色失败！");
                        }
                        else
                        {
                            transaction.Complete();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public bool UpdateFrontUserInfo(FrontUserDto frontUserDto)
        {
            try
            {
                if (null == frontUserDto)
                {
                    throw new KnownException("更新对象不能为空！");
                }
                using (var db = new BCEnterpriseContext())
                {


                    var q = db.FrontUsers.FirstOrDefault(obj => obj.UserID == frontUserDto.UserID);
                    if (null == q)
                    {
                        throw new KnownException("不存在该记录无法更新！");
                    }

                    q.Name = frontUserDto.Name;
                    q.Mobile = frontUserDto.Mobile;
                    q.UpdateTime = DateTime.Now;
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public FrontUserDto GetFrontUserByUserID(string userID)
        {
            try
            {
                if (string.IsNullOrEmpty(userID)) throw new KnownException("UserID不能为空！");
                using (var db = new BCEnterpriseContext())
                {
                    var u = db.FrontUsers.FirstOrDefault(obj => obj.UserID == userID);
                    if (null == u) return null;
                    return u;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<FrontUserDto> GetFrontUserByPartialInfo(FrontUserDto model, int pageSize, int pageIndex, out int count)
        {
            try
            {
                if (null == model)
                {
                    model = new FrontUserDto();
                }
                using (var db = new BCEnterpriseContext())
                {
                    var query = db.FrontUsers.Where(
                        obj => (string.IsNullOrEmpty(model.EnterpiseID) ? true : obj.EnterpiseID == model.EnterpiseID) &&
                              (string.IsNullOrEmpty(model.Name) ? true : obj.Name.Contains(model.Name)));
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
                                .OrderBy(obj => obj.UserID)
                                .Skip(pageSize * (pageIndex - 1))
                                .Take(pageSize)
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
                                }).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<FrontUserDto> GetAllFrontUser(string enterpriseId, int pageSize, int pageIndex, out int count)
        {
            try
            {
                if (string.IsNullOrEmpty(enterpriseId))
                {
                    return GetFrontUserByPartialInfo(null, pageSize, pageIndex, out count);
                }
                else
                {
                    var dto = new FrontUserDto() { EnterpiseID = enterpriseId };
                    return GetFrontUserByPartialInfo(dto, pageSize, pageIndex, out count);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        public List<FrontUserDto> SearchUserByName(string enterpriseNameKeyword, string userNameKeyword, int pageIndex, int pageSize,
            out int count)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var entIDList = db.Enterprises.Where(
                            obj => string.IsNullOrEmpty(enterpriseNameKeyword) ? true : obj.Name.Contains(enterpriseNameKeyword))
                            .Select(o => o.EnterpriseID)
                            .ToList();

                    var query = db.FrontUsers.Where(obj => (string.IsNullOrEmpty(userNameKeyword) ? true : obj.Name.Contains(userNameKeyword))
                                                        && (entIDList.Contains(obj.EnterpiseID)));

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

                    return query
                            .OrderBy(obj => obj.UserID)
                            .Skip(pageSize * (pageIndex - 1))
                            .Take(pageSize)
                            .Select(obj => new FrontUserDto()
                            {
                                UserID = obj.UserID,
                                Name = obj.Name,
                                EnterpiseID = obj.EnterpiseID,
                                DepartmentID = obj.DepartmentID,
                                DepartmentName = db.Departments.FirstOrDefault(x => x.DepartmentID == obj.DepartmentID).Name.ToString(),
                                Mobile = obj.Mobile,
                                LastIP = obj.LastIP,
                                LastDate = obj.LastDate,
                                Password = obj.Password,
                                UpdateTime = obj.UpdateTime,
                                Closed = obj.Closed,
                                RegistDate = obj.RegistDate,
                                LoginByDesktop = obj.LoginByDesktop,
                                LoginByMobile = obj.LoginByMobile
                            }).ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<FrontUserDto> GetUserList(string enterpriseID, int pageIndex, int pageSize, out int count)
        {
            return GetAllFrontUser(enterpriseID, pageSize, pageIndex, out count);
        }

        public List<FrontUserDto> SearchUser(string enterpriseID, string departmentKeyword,
                                                string nameKeyword, int pageIndex, int pageSize, out int count)
        {
            try
            {
                if (string.IsNullOrEmpty(enterpriseID))
                {
                    throw new KnownException("企业ID信息缺失！");
                }

                using (var db = new BCEnterpriseContext())
                {
                    var departmentIDList = db.Departments.Where(obj => string.IsNullOrEmpty(departmentKeyword) ? true : obj.Name.Contains(departmentKeyword) && !obj.Deleted)
                            .Select(o => o.DepartmentID)
                            .ToList();
                    if (departmentIDList.Count <= 0)
                    {
                        throw new KnownException("部门没有任何数据");
                    }
                    var query = db.FrontUsers.Where(obj => (obj.Name.Contains(nameKeyword))
                        && (string.IsNullOrEmpty(enterpriseID) ? true : obj.EnterpiseID.Equals(enterpriseID))
                        && (departmentIDList.Contains(obj.DepartmentID.Value) || obj.DepartmentID == null)
                        && !obj.Closed);

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

                    return query
                            .OrderBy(obj => obj.UserID)
                            .Skip(pageSize * (pageIndex - 1))
                            .Take(pageSize)
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
                            }).ToList();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool DeleteUser(string userID)
        {
            return DeleteFrontUser(userID);
        }

        public bool DisableUser(string userID)
        {
            try
            {
                if (string.IsNullOrEmpty(userID))
                {
                    throw new KnownException("用户ID不正确！");
                }
                using (var db = new BCEnterpriseContext())
                {
                    var q = db.FrontUsers.FirstOrDefault(obj => obj.UserID == userID);
                    if (q == null) throw new KnownException("此用户不存在！");

                    return q.Closed;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public FrontUserDto AddUser(FrontUserDto newUser)
        {
            try
            {
                if (null == newUser)
                {
                    throw new KnownException("添加用户信息不存在！");
                }
                using (var db = new BCEnterpriseContext())
                {
                    var entprise = db.Enterprises.Any(x => x.EnterpriseID.Equals(newUser.EnterpiseID) && !x.Deleted);
                    if (!entprise) throw new KnownException("企业信息不存在或者已被禁用，请刷新重试");
                    var id = AddFrontUser(newUser);
                    var reDto = db.FrontUsers.FirstOrDefault(obj => obj.UserID == id);
                    if (null == reDto)
                    {
                        throw new KnownException("保存用户失败！");
                    }
                    return reDto;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool UpdateUser(FrontUserDto user)
        {
            return UpdateFrontUser(user);
        }

        public bool UpdateUserPassword(string userId, string oldPass, string newPass)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) &&
                string.IsNullOrEmpty(oldPass) &&
                string.IsNullOrEmpty(newPass))
                {
                    throw new KnownException("缺少必要信息，无法更新密码！");
                }
                using (var db = new BCEnterpriseContext())
                {
                    var q = db.FrontUsers.FirstOrDefault(obj => obj.UserID == userId);

                    if (null == q)
                    {
                        throw new KnownException("用户不存在！");
                    }
                    if (oldPass == CryptoService.MD5Decrypt(q.Password))
                    {
                        q.Password = CryptoService.MD5Encrypt(newPass);
                        db.Entry(q).State = EntityState.Modified;
                        if (0 < db.SaveChanges()) return true;
                    }
                    else
                    {
                        throw new KnownException("原密码错误，修改密码失败！");
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsExistUser(string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    throw new KnownException("检查名字不能为空！");
                }
                using (var db = new BCEnterpriseContext())
                {
                    return 0 > db.FrontUsers.Count(obj => obj.Name.Contains(userName));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CheckUserPass(string userID, string pass)
        {
            try
            {
                if (string.IsNullOrEmpty(userID) &&
                                string.IsNullOrEmpty(pass))
                {
                    throw new KnownException("ID或密码为空！");
                }
                using (var db = new BCEnterpriseContext())
                {
                    var cryPass = CryptoService.MD5Encrypt(pass);
                    var p = db.FrontUsers.FirstOrDefault(obj => obj.UserID == userID && obj.Password == cryPass);
                    return null != p;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<FrontUserDto> GetUserByDepartmentIDs(List<int> departIDs, int pageSize, int pageIndex, out int count)
        {
            try
            {
                if (null == departIDs)
                {
                    throw new KnownException("部门ID信息缺失！");
                }
                using (var db = new BCEnterpriseContext())
                {
                    var query = db.FrontUsers.Where(obj => departIDs.Contains(obj.DepartmentID ?? 0) && obj.Closed == false);

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

                    return query
                            .OrderBy(obj => obj.UserID)
                            .Skip(pageSize * (pageIndex - 1))
                            .Take(pageSize)
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
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<FrontUserDto> GetAllUserByDepartmentID(string enterpriseID, int? departmentID, int pageSize, int pageIndex, out int count)
        {
            try
            {
                if (string.IsNullOrEmpty(enterpriseID)) throw new KnownException("企业ID不能为空！");
                if (null == departmentID) GetAllFrontUser(enterpriseID, pageSize, pageIndex, out count);
                List<int> departmentIDs;
                using (var db = new BCEnterpriseContext())
                {
                    departmentIDs = GetSonDepartments(db, departmentID ?? 0).Where(o => o.EnterpriseID == enterpriseID).Select(o => o.DepartmentID).ToList();
                }

                departmentIDs.Add(departmentID ?? 0);
                return GetUserByDepartmentIDs(departmentIDs, pageSize, pageIndex, out count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private List<FrontUserDto> GetUserByDepartmentIDsAndEnterpriseID(string enterpriseID, List<int> departIDs, int pageSize, int pageIndex, out int count)
        {
            try
            {
                if (null == departIDs)
                {
                    throw new KnownException("部门ID信息缺失！");
                }
                using (var db = new BCEnterpriseContext())
                {
                    var query = db.FrontUsers.Where(obj => (obj.EnterpiseID == enterpriseID) && (departIDs.Contains(obj.DepartmentID ?? 0)));

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

                    return query
                            .OrderBy(obj => obj.UserID)
                            .Skip(pageSize * (pageIndex - 1))
                            .Take(pageSize)
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
                            }).ToList();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private static IEnumerable<Department> GetSonDepartments(BCEnterpriseContext db, int parentID)
        {
            try
            {
                var query = from d in db.Departments
                            where d.ParentID == parentID
                            select d;
                return query.ToList().Concat(query.ToList().SelectMany(o => GetSonDepartments(db, o.DepartmentID)));
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }





        public bool SetUserAvatar(string userID, string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(userID) ||
                string.IsNullOrEmpty(imageUrl))
                {
                    //throw new KnownException("缺少必要信息，无法设置头像！");
                    return false;
                }
                using (var db = new BCEnterpriseContext())
                {
                    var q = db.FrontUsers.FirstOrDefault(obj => obj.UserID == userID);

                    if (null == q)
                    {
                        //throw new KnownException("用户不存在！");
                        return false;
                    }
                    q.Picture = imageUrl;
                    return db.SaveChanges() > 0;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public List<FrontUserDto> GetScanUsers(string sceneFunctionID,string projectFunctionID,string enterpriseID, List<int?> departmentID)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(sceneFunctionID)&&string.IsNullOrWhiteSpace(projectFunctionID))
                    throw new KnownException("参数传入不合法");
                using (var db = new BCEnterpriseContext())
                {
                    var tempusers = from fun in db.RFAAuthorizations
                        join user in db.UserRoles on fun.RoleID equals user.RoleID
                        where (fun.FunctionID == (sceneFunctionID??projectFunctionID)) && fun.Deleted == false
                        select new
                        {
                            userID = user.UserID
                        };
                    var users = tempusers.Select(x => x.userID);
                    
                    var userlist = db.FrontUsers.Where(x =>
                                (users.Contains(x.UserID)) && x.EnterpiseID.Equals(enterpriseID) &&
                                (departmentID.Contains(x.DepartmentID) || x.DepartmentID == null)).Select(obj=>new FrontUserDto()
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
                                });

                    return  userlist.ToList();
                }
            }
            catch (Exception e)
            {
                
                throw e;
            }
            
        }
    }
}
