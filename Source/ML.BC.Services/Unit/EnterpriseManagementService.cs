using ML.BC.EnterpriseData.Model;
using ML.BC.Services.Enterprise;
using ML.BC.Services.Enterprise.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq.Expressions;
using ML.BC.EnterpriseData.Model.Extend;
using ML.BC.Services.Common;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Services.Unit.Dtos;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.MsmqHelper;

namespace ML.BC.Services
{
    public class EnterpriseManagementService : IEnterpriseManagementService
    {
        public List<EnterpriseDto> SearchEnterpriseByCondition(string professionId, string propertyId, string enterpriseName, int pSize, int pNum, out int amount)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    if (string.IsNullOrEmpty(enterpriseName)) enterpriseName = "";
                    var list = db.Enterprises.Where(x => x.Name.Contains(enterpriseName));
                    if (!string.IsNullOrEmpty(professionId))
                    {
                        list = list.Where(x => x.ProfessionID.Equals(professionId));
                    }
                    if (!string.IsNullOrEmpty(propertyId))
                    {
                        list = list.Where(x => x.PropertyID.Equals(propertyId));
                    }
                    amount = list.Count();
                    int count;
                    //每页信息量判断
                    if (pSize > 0)
                    {
                        // 获取总共页数
                        count = (amount + pSize - 1) / pSize;
                    }
                    else
                    {
                        count = 0;
                    }
                    //页码判断，小于1则为1，大于最大页码则为最大页码
                    if (pNum > count)
                        pNum = count;
                    if (pNum < 1)
                        pNum = 1;
                    return list.Select(obj => new EnterpriseDto
                    {
                        ProfessionName = db.EnterpriseProfessions.FirstOrDefault(x => x.EnterpriseProfessionID == obj.ProfessionID && x.Available).Name,
                        EnterpriseID = obj.EnterpriseID,
                        Name = obj.Name,
                        ProfessionID = obj.ProfessionID,
                        Province = obj.Province,
                        City = obj.City,
                        Address = obj.Address,
                        Telephone = obj.Telephone,
                        Fax = obj.Fax,
                        Status = obj.Status,
                        RegistDate = obj.RegistDate,
                        UpdateTime = obj.UpdateTime,
                        Deleted = obj.Deleted,
                        PropertyID = obj.PropertyID,
                        PropertyName = db.EnterprisePropertys.FirstOrDefault(x => x.EnterprisePropertyID == obj.PropertyID && x.Available).Name
                    }).OrderBy(x => x.EnterpriseID).Skip(pSize * (pNum - 1)).Take(pSize).ToList();
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
                    return db.Enterprises.Count();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<EnterpriseDto> GetAllEnterpriseList()
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    return db.Enterprises.Where(x => !x.Deleted).Select(obj => new EnterpriseDto
                    {
                        EnterpriseID = obj.EnterpriseID,
                        Name = obj.Name,
                        ProfessionID = obj.ProfessionID,
                        Province = obj.Province,
                        City = obj.City,
                        Address = obj.Address,
                        Telephone = obj.Telephone,
                        Fax = obj.Fax,
                        Status = obj.Status,
                        RegistDate = obj.RegistDate,
                        UpdateTime = obj.UpdateTime,
                        Deleted = obj.Deleted,
                        PropertyID = obj.PropertyID
                    }).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool DeleteEnterprise(string enterpriseId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.Enterprises.FirstOrDefault(obj => obj.EnterpriseID.Equals(enterpriseId));
                    if (temp != null)
                        temp.Deleted = true;
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string AddEnterprise(EnterpriseDto enterprise)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var idGenerator = ML.BC.Infrastructure.Ioc.GetService<IUniqeIdGenerator>();
                    var enterpriseId = idGenerator.GeneratorEnterpriseID(enterprise.ProfessionID, enterprise.PropertyID);
                    //为null则行业ID和性质ID长度有误
                    if (null == enterpriseId)
                    {
                        throw new KnownException("企业ID生成失败");
                    }
                    else
                    {
                        enterprise.EnterpriseID = enterpriseId;
                    }
                    // 设置注册和更新时间为当前时间
                    enterprise.RegistDate = DateTime.Now;
                    enterprise.UpdateTime = DateTime.Now;
                    // 判断不允许为空的字段是否为空
                    if (
                       string.IsNullOrEmpty(enterprise.EnterpriseID) ||
                       string.IsNullOrEmpty(enterprise.Name) ||
                       string.IsNullOrEmpty(enterprise.ProfessionID) ||
                       string.IsNullOrEmpty(enterprise.PropertyID) ||
                       null == enterprise.UpdateTime ||
                       null == enterprise.RegistDate)
                        throw new KnownException("不允许非空字段为空");
                    var prof = db.EnterpriseProfessions.FirstOrDefault(x => x.EnterpriseProfessionID.Equals(enterprise.ProfessionID));
                    if (null == prof)
                    {
                        throw new KnownException("行业ID无效");
                    }
                    var prop = db.EnterprisePropertys.FirstOrDefault(x => x.EnterprisePropertyID.Equals(enterprise.PropertyID));
                    if (null == prop)
                    {
                        throw new KnownException("性质ID无效");
                    }
                    db.Enterprises.Add(enterprise);
                    //添加企业超级管理员
                    var uid = AddEnterpriseAdmin(db, enterpriseId);
                    //添加企业超级管理员用户角色关系
                    AddEnterpriseAdaminRole(db, enterpriseId, uid);
                    if (db.SaveChanges() > 0)
                    {
                        return enterprise.EnterpriseID;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private void AddEnterpriseAdaminRole(BCEnterpriseContext db, string enterpriseId, string uid)
        {
            UserRole adminRole = new UserRole()
            {
                Deleted = false,
                RoleID = ML.BC.EnterpriseData.Common.CommonConfig.ENTERPRISEMANAGERROLEID,
                UpdateTime = System.DateTime.Now,
                UserID = uid
            };
            db.UserRoles.Add(adminRole);
        }
        private string AddEnterpriseAdmin(BCEnterpriseContext db, string enterpriseId)
        {
            FrontUser admin = new FrontUser()
            {
                Name = enterpriseId + "Admin",
                EnterpiseID = enterpriseId,
                DepartmentID = null,
                Password = CryptoService.MD5Encrypt(ML.BC.EnterpriseData.Common.CommonConfig.ENTERPRISEMANAGERPASSWORD),
                Closed = false,
                RegistDate = System.DateTime.Now,
                UpdateTime = System.DateTime.Now,
                UserID = Ioc.GetService<ML.BC.Services.Common.IUniqeIdGenerator>().GeneratorFrontUserID()
            };

            db.FrontUsers.Add(admin);
            return admin.UserID;
        }
        public bool UpdateEnterprise(EnterpriseDto enterprise)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.Enterprises.First(x => x.EnterpriseID.Equals(enterprise.EnterpriseID));
                    if (null == temp)
                        throw new KnownException("该对象不存在");
                    temp.Name = enterprise.Name;
                    temp.ProfessionID = enterprise.ProfessionID;
                    temp.Province = enterprise.Province;
                    temp.City = enterprise.City;
                    temp.Address = enterprise.Address;
                    temp.Telephone = enterprise.Telephone;
                    temp.Fax = enterprise.Fax;
                    temp.Status = enterprise.Status;
                    temp.UpdateTime = enterprise.UpdateTime;
                    temp.PropertyID = enterprise.PropertyID;
                    if (db.SaveChanges() > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public EnterpriseDto GetOneByEnterpriseID(string enterpriseId)
        {
            if (enterpriseId.Equals("")) return new EnterpriseDto
            {
                EnterpriseID = "",
                Name = "运营平台"
            };
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var ent = db.Enterprises.FirstOrDefault(x => x.EnterpriseID.Equals(enterpriseId));
                    if (ent == null)
                    {
                        return new EnterpriseDto
                        {
                            EnterpriseID = enterpriseId,
                            Name = "企业[" + enterpriseId + "]不存在"
                        };
                    }
                    return ent;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private const string ADDSYNCSTATELOCK = "ADDSYNCSTATELOCK";
        private bool AddSyncState(params SyncStateDto[] syncStateDtos)
        {
            lock (ADDSYNCSTATELOCK)
            {
                try
                {
                    if (null == syncStateDtos || syncStateDtos.Length == 0) throw new ArgumentNullException("syncStateDtos");
                    for (int i = 0; i < syncStateDtos.Length; i++)
                    {
                        if (string.IsNullOrEmpty(syncStateDtos[i].UserID) || string.IsNullOrEmpty(syncStateDtos[i].DeviceID) || syncStateDtos[i].ActionType < 0)
                            throw new KnownException("对象索引:" + i + " 缺少必要信息，无法添加或更新！");
                    }

                    using (var db = new BCEnterpriseContext())
                    {
                        syncStateDtos.ToList().ForEach(m => m.SyncTime = DBTimeHelper.DBNowTime(db));
                        var objs = syncStateDtos.Select(n => new SyncState
                        {
                            SyncStateID = n.SyncStateID,
                            UserID = n.UserID,
                            DeviceID = n.DeviceID,
                            ActionType = n.ActionType,
                            SyncTime = n.SyncTime
                        }).ToList();

                        List<string> userids = objs.Select((n) => n.UserID).ToList();
                        List<string> deviceIDs = objs.Select((n) => n.DeviceID).ToList();
                        List<byte> actionTypes = objs.Select((n) => n.ActionType).ToList();
                        var oldlist = db.SyncStates.Where(m => userids.Contains(m.UserID) && deviceIDs.Contains(m.DeviceID) && actionTypes.Contains(m.ActionType));
                        oldlist.ToList().ForEach(n =>
                        {
                            db.SyncStates.Remove(n);
                        });

                        objs.ForEach(n =>
                        {
                            db.SyncStates.Add(n);
                        });
                        db.SaveChanges();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public EnterpriseSyncDto GetEnterpriseForSync(string enterpriseID, string userID, string deviceID)
        {
            using (var db = new BCEnterpriseContext())
            {
                //  同步表中无此用户和设备ID时，创建此记录并将Organization数据初始化发送给APP
                if (null == db.SyncStates.FirstOrDefault(o => (o.UserID == userID) && (o.DeviceID == deviceID)
                    && ((o.ActionType == (byte)TypeEnum.User) || (o.ActionType == (byte)TypeEnum.Role) || (o.ActionType == (byte)TypeEnum.Department))))
                {
                    var syncDepartment = new SyncState() { ActionType = (byte)TypeEnum.Department, DeviceID = deviceID, UserID = userID };
                    var syncUser = new SyncState() { ActionType = (byte)TypeEnum.User, DeviceID = deviceID, UserID = userID };
                    var syncRole = new SyncState() { ActionType = (byte)TypeEnum.Role, DeviceID = deviceID, UserID = userID };
                    AddSyncState(syncDepartment, syncUser, syncRole);
                    return GetEnterpriseForSync(enterpriseID);
                }

                var tempData = (from ep in db.Enterprises
                                where ep.EnterpriseID == enterpriseID
                                join dp in db.Departments on ep.EnterpriseID equals dp.EnterpriseID into tempDP
                                join user in db.FrontUsers on ep.EnterpriseID equals user.EnterpiseID into tempUser
                                join rl in db.RFARoles on ep.EnterpriseID equals rl.OwnerID into tempRL
                                select new
                                {
                                    EnterpriseID = ep.EnterpriseID,
                                    Departments = (
                                    from tdp in tempDP.DefaultIfEmpty()
                                    //where !tdp.Deleted
                                    from st in db.SyncStates
                                    where st.UserID == userID && st.DeviceID == deviceID && st.ActionType == (byte)TypeEnum.Department && st.SyncTime <= tdp.UpdateTime
                                    select tdp),
                                    FrontUsers = (
                                    from tusr in tempUser.DefaultIfEmpty()
                                    from st in db.SyncStates
                                    where st.UserID == userID && st.DeviceID == deviceID && st.ActionType == (byte)TypeEnum.User && st.SyncTime <= tusr.UpdateTime
                                    select new
                                    {
                                        tusr.UserID,
                                        tusr.Name,
                                        tusr.DepartmentID,
                                        tusr.Closed,
                                    }
                                    ),
                                    UserRoles = (
                                    from tul in tempRL.DefaultIfEmpty()
                                    join ur in db.UserRoles on tul.RoleID equals ur.RoleID into tempUR
                                    from tur in tempUR.DefaultIfEmpty()
                                    //where tul.Available && (tur == null || !tur.Deleted)
                                    from st in db.SyncStates
                                    where st.UserID == userID && st.DeviceID == deviceID && st.ActionType == (byte)TypeEnum.Role && (st.SyncTime <= tul.UpdateTime || tur == null || st.SyncTime <= tur.UpdateTime)
                                    select new
                                    {
                                        RoleId = tul.RoleID,
                                        RoleName = tul.Name,
                                        Available = tul.Available,
                                        UserID = tur.UserID,
                                        Deleted = tur != null && tur.Deleted
                                    }
                                    ).Union(
                                        from tul in db.RFARoles
                                        where tul.OwnerID == null
                                        join ur in db.UserRoles on tul.RoleID equals ur.RoleID into tempUR
                                        from tur in tempUR.DefaultIfEmpty()
                                        join user in tempUser on tur.UserID equals user.UserID into tempUser2
                                        from tuser in tempUser2.DefaultIfEmpty()
                                        where tul.Available && (tur == null || !tur.Deleted) && tuser != null
                                        from st in db.SyncStates
                                        where st.UserID == userID && st.DeviceID == deviceID && st.ActionType == 3 && (st.SyncTime <= tul.UpdateTime || (tur == null || st.SyncTime <= tur.UpdateTime))
                                        select new
                                        {
                                            RoleId = tul.RoleID,
                                            RoleName = tul.Name,
                                            Available = tul.Available,
                                            UserID = tur.UserID,
                                            Deleted = tur != null && tur.Deleted
                                        }
                                    )
                                }).FirstOrDefault();
                if (tempData == null) return new EnterpriseSyncDto();

                var result = new EnterpriseSyncDto
                {
                    EnterpriseID = tempData.EnterpriseID,
                    Departments = tempData.Departments.Select(obj => (DepartmentSyncDto)obj).ToArray(),
                    FrontUsers = tempData.FrontUsers.Select(n => new FrontUserSyncDto
                    {
                        EnterpriseID = tempData.EnterpriseID,
                        UserID = n.UserID,
                        Name = n.Name,
                        Closed = n.Closed,
                        DepartmentID = n.DepartmentID
                    }).ToArray(),
                    UserRoles = tempData.UserRoles.Select(n => new UserRoleSyncDto
                    {
                        EnterpriseID = tempData.EnterpriseID,
                        UserID = n.UserID,
                        RoleID = n.RoleId,
                        RoleName = n.RoleName,
                        Deleted = n.Deleted || !n.Available
                    }).ToArray()
                };

                return result;
            }
        }

        public EnterpriseSyncDto GetEnterpriseForSync(string enterpriseId)
        {
            using (var db = new BCEnterpriseContext())
            {
                var tempData = (from ep in db.Enterprises
                                where ep.EnterpriseID == enterpriseId
                                join dp in db.Departments on ep.EnterpriseID equals dp.EnterpriseID into tempDP
                                join user in db.FrontUsers on ep.EnterpriseID equals user.EnterpiseID into tempUser
                                join rl in db.RFARoles on ep.EnterpriseID equals rl.OwnerID into tempRL
                                select new
                                {
                                    EnterpriseID = ep.EnterpriseID,
                                    Departments = (
                                    from tdp in tempDP.DefaultIfEmpty()
                                    where !tdp.Deleted
                                    select tdp),
                                    FrontUsers = (
                                    from tur in tempUser.DefaultIfEmpty()
                                    select new
                                    {
                                        tur.UserID,
                                        tur.Name,
                                        tur.DepartmentID,
                                        tur.Closed
                                    }
                                    ),
                                    UserRoles = (
                                    from tul in tempRL.DefaultIfEmpty()
                                    join ur in db.UserRoles on tul.RoleID equals ur.RoleID into tempUR
                                    from tur in tempUR.DefaultIfEmpty()
                                    where tul.Available && (tur == null || !tur.Deleted)
                                    select new
                                    {
                                        RoleId = tul.RoleID,
                                        RoleName = tul.Name,
                                        Available = tul.Available,
                                        UserID = tur.UserID,
                                        Deleted = tur != null && tur.Deleted
                                    }
                                    ).Union(
                                        from tul in db.RFARoles
                                        where tul.OwnerID == null
                                        join ur in db.UserRoles on tul.RoleID equals ur.RoleID into tempUR
                                        from tur in tempUR.DefaultIfEmpty()
                                        join user in tempUser on tur.UserID equals user.UserID into tempUser2
                                        from tuser in tempUser2.DefaultIfEmpty()
                                        where tul.Available && (tur == null || !tur.Deleted) && tuser != null
                                        select new
                                        {
                                            RoleId = tul.RoleID,
                                            RoleName = tul.Name,
                                            Available = tul.Available,
                                            UserID = tur.UserID,
                                            Deleted = tur != null && tur.Deleted
                                        }
                                    )
                                }).FirstOrDefault();
                if (tempData == null) return new EnterpriseSyncDto();

                var result = new EnterpriseSyncDto
                {
                    EnterpriseID = tempData.EnterpriseID,
                    Departments = tempData.Departments.Select(obj => (DepartmentSyncDto)obj).ToArray(),
                    FrontUsers = tempData.FrontUsers.Select(n => new FrontUserSyncDto
                    {
                        EnterpriseID = tempData.EnterpriseID,
                        UserID = n.UserID,
                        Name = n.Name,
                        Closed = n.Closed,
                        DepartmentID = n.DepartmentID
                    }).ToArray(),
                    UserRoles = tempData.UserRoles.Select(n => new UserRoleSyncDto
                    {
                        EnterpriseID = tempData.EnterpriseID,
                        UserID = n.UserID,
                        RoleID = n.RoleId,
                        RoleName = n.RoleName,
                        Deleted = n.Deleted || !n.Available
                    }).ToArray()
                };

                return result;
            }
        }
    }
}
