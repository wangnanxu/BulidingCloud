using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model;
using ML.BC.EnterpriseData.MongoDb;

namespace ML.BC.Services
{
    public class ReviewStatistics : IReviewStatistics
    {
        private string ProjectFunction = "Root.ProjectManagement.ProjectListManagement.ShowAll";
        private string SceneFunction = "Root.ProjectManagement.SceneListManagement.ShowAll";
        public ReviewStatisticsDto GetReviewStatistics(string userID, DateTime? beginTime, DateTime? endTime)
        {
            try
            {
                var mgdb = new MongoDbProvider<SceneItem>();
                using (var db = new BCEnterpriseContext())
                {
                    var roles = (from users in db.UserRoles
                                 join role in db.RFAAuthorizations on users.RoleID equals role.RoleID
                                 where users.UserID == userID && !users.Deleted && !role.Deleted && (role.FunctionID == ProjectFunction || role.FunctionID == SceneFunction)
                                 select new
                                 {
                                     funID = role.FunctionID
                                 }).Distinct();
                    if (0 == roles.Count())
                    {
                        return new ReviewStatisticsDto();
                    }
                    var user = GetMyDepartment(userID, db);
                    var proflag = roles.FirstOrDefault(x => x.Equals(ProjectFunction));
                    var sceneflag = roles.FirstOrDefault(x => x.Equals(SceneFunction));
                    var department = new List<int>();
                    var tempprojectAll = new List<string>();
                    var tempsceneAll = new List<EnterpriseData.Model.Scene>();
                    //获取企业所有项目
                    var tempprojectlist = db.Projects.Where(x => x.EnterpriseID.Equals(user.EnterpiseID) && !x.Deleted && (beginTime ?? DateTime.MinValue) < x.UpdateTime && x.UpdateTime < (endTime ?? DateTime.MaxValue)).ToList();
                    if (null != proflag)
                    {
                        //没有查看全部权限时传入参数为级部门ID
                        department = GetNowDepartment(user.EnterpiseID, user.DepartmentID, db).Select(x => x.DepartmentID).ToList();
                    }
                    else
                    {
                        //拥有查看全部权限时候传入参数为Null
                        department = GetNowDepartment(user.EnterpiseID, null, db).Select(x => x.DepartmentID).ToList();
                    }
                    if (0 == department.Count())
                    {
                        department = new List<int>();
                    }
                    department.Add(user.DepartmentID ?? -1);

                    tempprojectAll = tempprojectlist.Where(x => x.Departments.Split("|".ToCharArray()).Any(n => department.Any(m => m.ToString() == n)))
                        .Select(n => n.ProjectID).ToList();
                    if (null != sceneflag)
                    {
                        //没有查看全部权限时传入参数为级部门ID
                        department = GetNowDepartment(user.EnterpiseID, user.DepartmentID, db).Select(x => x.DepartmentID).ToList();
                    }
                    else
                    {
                        //拥有查看全部权限时候传入参数为Null
                        department = GetNowDepartment(user.EnterpiseID, null, db).Select(x => x.DepartmentID).ToList();
                    }
                    if (0 == department.Count())
                    {
                        department = new List<int>();
                    }
                    department.Add(user.DepartmentID ?? -1);

                    var tempprojAll = tempprojectlist.Where(x => x.Departments.Split("|".ToCharArray()).Any(n => department.Any(m => m.ToString() == n)))
                        .Select(n => n.ProjectID).ToList();

                    department = GetOwnerDepartment(user.DepartmentID ?? -1, db).Select(x => x.DepartmentID).ToList();
                    if (0 == department.Count())
                    {
                        department = new List<int>();
                    }
                    department.Add(user.DepartmentID ?? -1);
                    tempsceneAll = db.Scenes.Where(x => tempprojAll.Contains(x.ProjectID) && !x.Deleted).ToList();
                    var templist = tempsceneAll.Select(x => x.SceneID).ToList();
                    var scenelist =
                        mgdb.GetAll(
                            x => (null != x.Examines) && templist.Contains(x.SceneID))
                            .Select(m => m.SceneID).Distinct().ToList();
                    var proIDlist =
                        tempsceneAll.Where(x => scenelist.Contains(x.SceneID) && !x.Deleted).Select(n => n.ProjectID).ToList();
                    var projectlist = tempprojAll.Where(x => proIDlist.Contains(x)).ToList();
                    return new ReviewStatisticsDto()
                    {
                        ProjectAll = tempprojectAll.Count(),
                        ProjectScan = projectlist.Count(),
                        SceneAll = tempsceneAll.Count(),
                        SceneScan = scenelist.Count()
                    };
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private List<string> GetProjectList()
        {
            return new List<string>();
        }

        private List<DepartmentDto> GetNowDepartment(string enterpriseId, int? departmentId, BCEnterpriseContext db)
        {
            try
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
                foreach (var deptmt in dtmt)
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
            catch (Exception e)
            {
                throw e;
            }
        }
        private FrontUser GetMyDepartment(string userID, BCEnterpriseContext db)
        {
            try
            {
                return db.FrontUsers.FirstOrDefault(x => x.UserID.Equals(userID));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private IEnumerable<Department> GetOwnerDepartment(int departmentId, BCEnterpriseContext db)
        {
            try
            {
                var list = db.Departments.Where(x => x.ParentID == departmentId && !x.Deleted);
                if (0 == list.Count())
                    return list;
                return list.ToList().Concat(list.ToList().SelectMany(x => GetOwnerDepartment(x.DepartmentID, db)));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
