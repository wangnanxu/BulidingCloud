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
using System.Messaging;
using ML.BC.Services.Common;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.MsmqHelper;

namespace ML.BC.Services
{
    public class EnterpriseDepartmentManagementService:IEnterpriseDepartmentManagementService
    {
        public List<DepartmentDto> SearchDepartmentByName(string nameKeyword, int pageSize, int pageNumber)
        {
            try
            {
                if(string.IsNullOrEmpty(nameKeyword))
                {
                    nameKeyword = "";
                }
                using(var db = new BCEnterpriseContext())
                {
                    var list = db.Departments.Where(x => x.Name.Contains(nameKeyword)).Select(x => new DepartmentDto
                    {
                        DepartmentID = x.DepartmentID,
                        ParentID = x.ParentID,
                        Name = x.Name,
                        EnterpriseID = x.EnterpriseID,
                        Description = x.Description,
                        Available = x.Available,
                        Deleted = x.Deleted
                    });;
                    int pagecount;
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
                    return list.OrderBy(x => x.DepartmentID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        private IEnumerable<Department> GetOwnerDepartment(int departmentId, BCEnterpriseContext db)
        {
            try
            {
                var list = db.Departments.Where(x => x.ParentID == departmentId && x.Deleted == false);
                if (null == list)
                    return list;
                return list.ToList().Concat(list.ToList().SelectMany(x => GetOwnerDepartment(x.DepartmentID, db)));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<DepartmentDto> GetMyDepartment(string enterpriseId,int? departmentId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    IEnumerable<Department> dtmt = null;
                    if (departmentId.HasValue)
                    {
                        dtmt = db.Departments.Where(x => x.EnterpriseID == enterpriseId && x.DepartmentID == departmentId.Value && x.Deleted == false);
                    }
                    else
                    {
                        dtmt = db.Departments.Where(x => x.EnterpriseID == enterpriseId && x.ParentID == 0 && x.Deleted == false);
                    }
                    
                    if (0 == dtmt.Count())
                        return dtmt.Select(x => new DepartmentDto
                        {
                            DepartmentID = x.DepartmentID,
                            ParentID = x.ParentID,
                            Name = x.Name,
                            EnterpriseID = x.EnterpriseID,
                            Description = x.Description,
                            Available = x.Available,
                            Deleted = x.Deleted
                        }).ToList();
                    //var templist = dtmt.ToList();
                    //for (int i = 0; i < dtmt.Count();i++ )
                    //{
                    //    dtmt = dtmt.Concat(GetOwnerDepartment(templist[i].DepartmentID, db));
                    //}
                    foreach(var deptmt in dtmt)
                    {
                        dtmt = dtmt.Concat(GetOwnerDepartment(deptmt.DepartmentID, db));
                    }
                    return dtmt.Select(x => new DepartmentDto
                    {
                        DepartmentID = x.DepartmentID,
                        ParentID = x.ParentID,
                        Name = x.Name,
                        EnterpriseID = x.EnterpriseID,
                        Description = x.Description,
                        Available = x.Available,
                        Deleted = x.Deleted
                    }).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<DepartmentDto> GetAllAvaliableDepartmentByEnterpriseId(string enterpriseId)
        {
            try
            {
                if(string.IsNullOrEmpty(enterpriseId))
                {
                    throw new KnownException("企业ID不能为空");
                }
                using (var db = new BCEnterpriseContext())
                {
                    return db.Departments.Where(x=>x.EnterpriseID.Equals(enterpriseId)&&x.Available&&x.Deleted == false).Select(x=>new DepartmentDto
                    {
                        DepartmentID = x.DepartmentID,
                        ParentID = x.ParentID,
                        Name = x.Name,
                        EnterpriseID = x.EnterpriseID,
                        Description = x.Description,
                        Available = x.Available,
                        Deleted = x.Deleted
                    }).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Update(DepartmentDto department)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.Departments.FirstOrDefault(x => x.DepartmentID == department.DepartmentID && !x.Deleted);
                    if(null == temp)
                    {
                        throw new KnownException("此部门不存在");
                    }
                    temp.ParentID = department.ParentID;
                    temp.Name = department.Name;
                    temp.Description = department.Description;
                    temp.Available = department.Available;
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private int DeleteDepartment(int departmentId,BCEnterpriseContext db)
        {
            try
            {
                var temp = db.Departments.FirstOrDefault(x => x.DepartmentID == departmentId);
                if(null == temp)
                {
                    return 0;
                }
                temp.Deleted = true;
                var list = db.Departments.Where(x => x.ParentID == departmentId).ToList() ;
                for (int i = 0; i < list.Count();i++ )
                {
                    if (0 == DeleteDepartment(list[i].DepartmentID, db))
                        break;
                }
                return db.SaveChanges();
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public bool Delete(int departmentId,string enterpriseId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var mydepartments = GetOwnerDepartment(departmentId, db).Select(x=>x.DepartmentID).ToList();
                    mydepartments.Add(departmentId);
                    var users = db.FrontUsers.FirstOrDefault(x => mydepartments.Contains(x.DepartmentID??-5) && x.EnterpiseID.Equals(enterpriseId));
                    if (null == users)
                        return 0 != DeleteDepartment(departmentId, db);
                    else
                        throw new KnownException("该部门存在人员，无法删除");
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int Add(DepartmentBase department)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    //Department temp = new Department {
                    //    ParentID = department.ParentID,
                    //    Name = department.Name,
                    //    EnterpriseID = department.EnterpriseID,
                    //    Description = department.Description,
                    //    Available = department.Available,
                    //    Deleted = department.Deleted
                    //};
                    db.Departments.Add(department);
                    if(0>=db.SaveChanges())
                    {
                        throw new KnownException("部门添加失败");
                    }
                    else
                    {
                        return department.ParentID;
                    }
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
                using(var db = new BCEnterpriseContext())
                {
                    return db.Departments.Count();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public string GetEnterpriseNameById(string enterpriseId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var name = db.Enterprises.First(x => x.EnterpriseID.Equals(enterpriseId));
                    return name.EnterpriseID;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetDepartmentNameById(int departmentId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var department = db.Departments.FirstOrDefault(x => x.DepartmentID == departmentId);
                    if (department == null) return "";
                    if(null == department)
                    {
                        return null;
                    }
                    else
                    {
                        return department.Name;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //private bool CheckRoot()
        //{

        //}
        public DepartmentDto GetRootDepartmentByEntpriseId(string enterpriseId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var fc = db.Departments.FirstOrDefault(x => x.DepartmentID == 0);
                    if(!(null == fc))
                    {
                        return fc;
                    }
                    else
                    {
                        var temp = db.Enterprises.First(x => x.EnterpriseID.Equals(enterpriseId));
                        var rootname = temp.Name;
                        var st = new DepartmentDto
                        {
                            ParentID = 0,
                            Name = rootname,
                            Available = true,
                            Deleted = false,
                            EnterpriseID = enterpriseId,
                            Description = "Chuckie is a big SB??"
                        };
                        db.Departments.Add(st);
                        if(0>=db.SaveChanges())
                        {
                            throw new KnownException("生成根节点失败");
                        }
                        else
                        {
                            return st;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<DepartmentDto> GetMyDepartment(string enterpriseId)
        {
            return GetMyDepartment(enterpriseId, null);
        }


        public DepartmentDto GetParentDepartment(int departmentId)
        {
            try
            {
                using(var db = new BCEnterpriseContext())
                {
                    var myDepartment = db.Departments.FirstOrDefault(x => x.DepartmentID == departmentId&&!x.Deleted);
                    return db.Departments.Where(x => x.DepartmentID == myDepartment.ParentID&&!x.Deleted).Select(n => new DepartmentDto
                        {
                            Available = n.Available,
                            Deleted = n.Deleted,
                            DepartmentID = n.DepartmentID,
                            Description = n.Description,
                            EnterpriseID = n.EnterpriseID,
                            Name = n.Name,
                            ParentID = n.ParentID
                        }).ToList().FirstOrDefault();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
