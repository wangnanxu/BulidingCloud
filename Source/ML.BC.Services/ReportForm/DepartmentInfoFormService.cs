using ML.BC.EnterpriseData.Model;
using ML.BC.EnterpriseData.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class DepartmentInfoFormService:IDepartmentInfoFormService
    {
        private DepartmentInfoFormDto GetDepartmentInfoFormOfMyDepartment(int? departmentId, string enterpriseId, DateTime? beginTime, DateTime? endTime,BCEnterpriseContext db)
        {
            try
            {
                    var mgdb = new MongoDbProvider<SceneItem>();
                    //get my departments
                    var dpartmentlist = GetMyDepartment(enterpriseId,departmentId,db);
                    //get my sceneId
                    var scenlist = (from scene in db.Scenes
                                    join project in db.Projects on scene.ProjectID equals project.ProjectID
                                        select new{
                                        SceneId = scene.SceneID,
                                        Deleted = scene.Deleted,
                                        RegistDate = scene.RegistDate,
                                        Departments = project.Departments
                                        }).ToList();

                    var scenelist = scenlist.Where(o => o.Departments.Split("|".ToCharArray()).Any(x => (dpartmentlist.Any(n => n.DepartmentID.ToString().Equals(x)))));                    
                    //get projectId in my department
                    var projectlist = db.Projects.Where(x => !x.Deleted).ToList();
                    var newprolist = projectlist.Where(x => x.Departments.Split("|".ToCharArray()).Any(n => (dpartmentlist.Any(m => m.DepartmentID.ToString().Equals(n)))));
                    //get pictures of scene on my department
                    var sceneIdlist = scenelist.Select(x => x.SceneId);
                    var Picount = mgdb.GetAll(x => sceneIdlist.Contains(x.SceneID) && (x.CreateTime > (beginTime ?? DateTime.MinValue)) && (x.CreateTime < (endTime ?? DateTime.MaxValue))).Select(n => n.Count).Sum();
                    var pByte = mgdb.GetAll(x => sceneIdlist.Contains(x.SceneID) && (x.CreateTime > (beginTime ?? DateTime.MinValue)) && (x.CreateTime < (endTime ?? DateTime.MaxValue))).Select(n => n.TotalOrgImageBytes).Sum();
                    //get users of my department
                    var templist = dpartmentlist.Select(n=>n.DepartmentID).ToList();
                    var userlist = db.FrontUsers.Where(x=>null != x.DepartmentID&&templist.Contains(x.DepartmentID.Value));
                    var depart = db.Departments.FirstOrDefault(x=>x.DepartmentID == departmentId.Value);
                    return new DepartmentInfoFormDto
                    {
                        DepartmentName = depart.Name,
                        //get users in my department
                        UsersCount = userlist.Where(n =>
                            n.EnterpiseID.Equals(enterpriseId)
                            && n.RegistDate < (endTime??DateTime.MaxValue)
                            && n.RegistDate > (beginTime??DateTime.MinValue)).Count(),
                        ProjectCount = newprolist.Where(n=>
                        n.RegistDate > (beginTime ?? DateTime.MinValue)
                        && n.RegistDate < (endTime ?? DateTime.MaxValue)).Count(),
                        SceneCount = scenelist.Where(n =>
                            !n.Deleted
                            && n.RegistDate > (beginTime ?? DateTime.MinValue)
                            && n.RegistDate < (endTime ?? DateTime.MaxValue)).Count(),
                        PictureCount = Picount,
                        PictureByte = pByte
                    };
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public List<DepartmentInfoFormDto> GetDepartmentInfoFormOfDepartment(int? departmentId, string enterpriseId, DateTime? beginTime, DateTime? endTime)
        {
            try
            {
                using(var db = new BCEnterpriseContext())
                {
                    List<DepartmentInfoFormDto> infolist = new List<DepartmentInfoFormDto>();
                    IEnumerable<Department> dtmt = null;
                    if (departmentId.HasValue)
                    {
                        dtmt = db.Departments.Where(x => x.EnterpriseID == enterpriseId && x.ParentID == departmentId.Value && !x.Deleted);
                    }
                    else
                    {
                        dtmt = db.Departments.Where(x => x.EnterpriseID == enterpriseId && x.ParentID == 0 && !x.Deleted);
                    }
                    if (0 == dtmt.Count())
                        return dtmt.Select(x => new DepartmentInfoFormDto{}).ToList();
                    //foreach (var deptmt in dtmt)
                    //{
                    //    dtmt = dtmt.Concat(GetOwnerDepartment(deptmt.DepartmentID, db));
                    //}
                    foreach(var departId in dtmt)
                    {
                        infolist.Add(GetDepartmentInfoFormOfMyDepartment(departId.DepartmentID, enterpriseId, beginTime, endTime, db));
                    }
                    return infolist;
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
                var list = db.Departments.Where(x => x.ParentID == departmentId && !x.Deleted);
                if (null == list)
                    return list;
                return list.ToList().Concat(list.ToList().SelectMany(x => GetOwnerDepartment(x.DepartmentID, db)));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private List<DepartmentDto> GetMyDepartment(string enterpriseId, int? departmentId, BCEnterpriseContext db)
        {
            try
            {
               IEnumerable<Department> dtmt = null;
               if (departmentId.HasValue)
               {
                    dtmt = db.Departments.Where(x => x.EnterpriseID == enterpriseId && x.DepartmentID == departmentId.Value && !x.Deleted);
               }
               else
               {
                   dtmt = db.Departments.Where(x => x.EnterpriseID == enterpriseId && x.ParentID == 0 && !x.Deleted);
               }

               if (0 == dtmt.Count())
                   return dtmt.Select(x => new DepartmentDto
               {
                    DepartmentID = x.DepartmentID,
                    Name = x.Name,
                }).ToList();
               foreach (var deptmt in dtmt)
               {
                    dtmt = dtmt.Concat(GetOwnerDepartment(deptmt.DepartmentID, db));
               }
               return dtmt.Select(x => new DepartmentDto
               {
                    DepartmentID = x.DepartmentID,
                    Name = x.Name,
               }).ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<DepartmentInfoFormDto> GetDepartmentInfoFormOfEnterprise(string enterpriseId, DateTime? beginTime, DateTime? endTime)
        {
            return GetDepartmentInfoFormOfDepartment(null,enterpriseId, beginTime, endTime);
        }
    }
}
