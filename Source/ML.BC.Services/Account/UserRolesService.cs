using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.BCBackData.Model;
using ML.BC.Infrastructure.Exceptions;

namespace ML.BC.Services
{
    public class UserRolesServerce : IUserRolesServerce
    {
        public string AddUserRole(string userID, int roleID)
        {
            try
            {
                if (string.IsNullOrEmpty(userID))
                {
                    throw new KnownException("传递参数有误！");
                }

                using (var db = new BCBackContext())
                {
                    if (!db.BackUsers.Any(obj => obj.UserID == userID)) throw new KnownException("不存在的用户ID无法添加用户角色！");
                    if (!db.RFARoles.Any(obj => obj.RoleID == roleID)) throw new KnownException("不存在的角色ID无法添加用户角色！");

                    db.UserRoles.Add(new UserRole() { UserID = userID, RoleID = roleID, UpdateTime = DateTime.Now });
                    var re = db.SaveChanges();
                    if (1 == re)
                    {
                        return userID;
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


        public bool DeleteUserRole(string userID, int roleID)
        {
            try
            {
                if (string.IsNullOrEmpty(userID))
                {
                    throw new KnownException("传递参数有误！");
                }
                using (var db = new BCBackContext())
                {
                    var del = new UserRole() { UserID = userID, RoleID = roleID };
                    db.UserRoles.Attach(del);
                    db.UserRoles.Remove(del);
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

        public List<int> GetAllRolesByUserId(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new KnownException("用户id输入有误");
                }
                List<int> list = new List<int>();
                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }
                using (var db = new BCBackContext())
                {
                    list = db.UserRoles.Where(obj => obj.UserID == userId && !obj.Deleted).Select(obj => obj.RoleID).ToList();
                    list = db.RFARoles.Where(obj => list.Contains(obj.RoleID) && obj.Available).Select(o => o.RoleID).ToList();
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RolesDto> GetAllRoles()
        {
            try
            {
                using (var db = new BCBackContext())
                {
                    var list = db.RFARoles.Where(obj => null != obj && obj.Available).Select(obj => new RolesDto
                    {
                        RoleID = obj.RoleID,
                        Name = obj.Name,
                        Available = obj.Available,
                        OwnerID = obj.OwnerID
                    }).ToList();
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public List<UserRolesDto> SetRole(string userId, List<int> roleID)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(userId) || null == roleID)
        //        {
        //            throw new KnownException("参数传递有误！");
        //        }

        //        List<UserRolesDto> listUr = new List<UserRolesDto>();
        //        using (var db = new BCBackContext())
        //        {
        //            foreach (var id in roleID)
        //            {
        //                UserRole ur = new UserRole() { UserID = userId, RoleID = id, UpdateTime = DateTime.Now };
        //                db.UserRoles.AddOrUpdate(ur);
        //            }

        //            if (0 < db.SaveChanges())
        //            {
        //                foreach (var id in roleID)
        //                {
        //                    listUr.Add(db.UserRoles.FirstOrDefault(obj => obj.UserID == userId && obj.RoleID == id));
        //                }
        //            }
        //        }
        //        return listUr;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}
    }
}
