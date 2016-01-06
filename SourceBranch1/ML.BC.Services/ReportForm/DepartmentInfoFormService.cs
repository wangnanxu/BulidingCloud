using ML.BC.EnterpriseData.Model;
using ML.BC.EnterpriseData.MongoDb;
using ML.BC.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class DepartmentInfoFormService : IDepartmentInfoFormService
    {
        private DepartmentInfoFormDto GetDepartmentInfoFormOfMyDepartment(int? departmentId, string enterpriseId, DateTime? beginTime, DateTime? endTime, BCEnterpriseContext db)
        {
            try
            {
                var mgdb = new MongoDbProvider<SceneItem>();
                //get my sceneId
                var scenlist = (from scene in db.Scenes
                                join project in db.Projects on scene.ProjectID equals project.ProjectID
                                where project.EnterpriseID == enterpriseId
                                select new
                                {
                                    SceneId = scene.SceneID,
                                    Deleted = scene.Deleted,
                                    RegistDate = scene.RegistDate,
                                    Departments = project.Departments
                                }).ToList();
                //由原来获取当前部门下的所有部门的现场数 改为 获取当前部门的现场数
                var scenelist = scenlist.Where(o => o.Departments.Split("|".ToCharArray()).Any(x => x == (!departmentId.HasValue ? "" : departmentId.Value.ToString())));
                //get projectId in my department
                var projectlist = db.Projects.Where(x => !x.Deleted).ToList();
                //由原来获取当前部门下的所有部门的项目数 改为 获取当前部门的项目数
                var newprolist = projectlist.Where(x => x.Departments.Split("|".ToCharArray()).Any(n => n == (!departmentId.HasValue ? "" : departmentId.Value.ToString())));
                //get pictures of scene on my department
                var sceneIdlist = scenelist.Select(x => x.SceneId);

                var Picount = mgdb.GetAll(x => sceneIdlist.Contains(x.SceneID) && (x.CreateTime > (beginTime ?? DateTime.MinValue)) && (x.CreateTime < (endTime ?? DateTime.MaxValue))).Select(n => n.Count).Sum();
                var pByte = mgdb.GetAll(x => sceneIdlist.Contains(x.SceneID) && (x.CreateTime > (beginTime ?? DateTime.MinValue)) && (x.CreateTime < (endTime ?? DateTime.MaxValue))).Select(n => n.TotalOrgImageBytes).Sum();
                //由原来获取属于当前部门及其子部门的人 改成只获取属于当前部门的人
                var userlist = db.FrontUsers.Where(x => x.DepartmentID == departmentId);


                var depart = db.Departments.FirstOrDefault(x => x.DepartmentID == departmentId.Value);

                return new DepartmentInfoFormDto
                {
                    DepartmentName = depart.Name,
                    DepartmentID = depart.DepartmentID,
                    ParentID = depart.ParentID,
                    PictureByte = pByte,
                    //get users in my department
                    UsersCount = userlist.Where(n =>
                        n.EnterpiseID.Equals(enterpriseId)
                        && n.RegistDate < (endTime ?? DateTime.MaxValue)
                        && n.RegistDate > (beginTime ?? DateTime.MinValue)).Count(),
                    ProjectCount = newprolist.Where(n =>
                    n.RegistDate > (beginTime ?? DateTime.MinValue)
                    && n.RegistDate < (endTime ?? DateTime.MaxValue)).Count(),
                    SceneCount = scenelist.Where(n =>
                        !n.Deleted
                        && n.RegistDate > (beginTime ?? DateTime.MinValue)
                        && n.RegistDate < (endTime ?? DateTime.MaxValue)).Count(),
                    PictureCount = Picount
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private List<Department> GetOwnerDepartment(int departmentId, BCEnterpriseContext db)
        {
            try
            {
                var list = db.Departments.Where(x => x.ParentID == departmentId && !x.Deleted);
                if (null == list)
                    return new List<Department>();
                return list.ToList().Concat(list.ToList().SelectMany(x => GetOwnerDepartment(x.DepartmentID, db))).ToList() ?? new List<Department>();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<DepartmentInfoFormDto> GetDepartmentInfoFormOfEnterprise(string enterpriseId, DateTime? beginTime, DateTime? endTime)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    List<DepartmentInfoFormDto> infolist = new List<DepartmentInfoFormDto>();
                    List<Department> dtmt = new List<Department>();
                    List<Department> dtmtlist = new List<Department>();
                    IQueryable<Department> temp;
                    temp = db.Departments.Where(x => x.EnterpriseID == enterpriseId && x.ParentID == 0 && !x.Deleted);
                    if (temp.Count() <= 0) throw new KnownException("没有相关信息");
                    foreach (var d in temp)
                    {
                        dtmtlist = GetOwnerDepartment(d.DepartmentID, db);
                        dtmt.Add(d);
                        dtmt = dtmt.Concat(dtmtlist).ToList();
                    }
                    if (0 == dtmt.Count())
                        return dtmt.Select(x => new DepartmentInfoFormDto { }).ToList();
                    foreach (var departId in dtmt)
                    {
                        infolist.Add(GetDepartmentInfoFormOfMyDepartment(departId.DepartmentID, enterpriseId, beginTime, endTime, db));
                    }
                    return infolist;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
