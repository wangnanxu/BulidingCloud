using System;
using ML.BC.EnterpriseData.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.Infrastructure.Exceptions;

namespace ML.BC.Services
{
    public class ProjectSceneCompletionStatistics : IProjectSceneCompletionStatistics
    {
        public ProjectSceneCompletionStatisticsDto GetReportForm(string enterpriseId, DateTime? beginTime, DateTime? endTime, List<int> departments, string address)
        {
            try
            {
                List<Department> dtmt = new List<Department>();
                List<Department> dtmtlist = new List<Department>();
                IQueryable<Department> temp;
                if (string.IsNullOrEmpty(address))
                    address = "";
                //                List<Project> tempprojectlist = new List<Project>();
                using (var db = new BCEnterpriseContext())
                {
                    if (0 == departments.Count())
                    {
                        temp = db.Departments.Where(x => x.EnterpriseID == enterpriseId && x.ParentID == 0 && !x.Deleted);
                        if (temp.Count() <= 0) throw new KnownException("没有相关信息");
                        foreach (var d in temp)
                        {
                            dtmtlist = GetOwnerDepartment(d.DepartmentID, db);
                            dtmt.Add(d);
                            dtmt = dtmt.Concat(dtmtlist).ToList();
                        }
                        departments = dtmt.Select(x => x.DepartmentID).ToList();
                    }
                    var project = db.Projects.Where(x =>
                        x.EnterpriseID.Equals(enterpriseId)
                        && !x.Deleted).ToList();
                    var projectlist = project.Where(x => x.Departments.Split("|".ToCharArray()).Any(n => departments.Any(m => m.ToString() == n))).ToList();
                    List<string> scenelist = new List<string>();
                    List<Project> listprojectdata = new List<Project>();
                    if (!string.IsNullOrEmpty(address))
                    {
                        scenelist = db.Scenes.Where(x =>
                                    x.EnterpriseID.Equals(enterpriseId)
                                    && x.Address.Contains(address)
                                    && !x.Deleted).Select(n => n.ProjectID).ToList();
                        listprojectdata = projectlist.Where(x => (scenelist ?? new List<string>()).Contains(x.ProjectID)
                                          && !((x.BeginDate > (endTime ?? DateTime.MaxValue))
                                          || (x.EndDate < (beginTime ?? DateTime.MinValue)))).ToList();
                    }
                    else
                    {
                        listprojectdata = projectlist.Where(x => !((x.BeginDate > (endTime ?? DateTime.MaxValue))
                                          || (x.EndDate < (beginTime ?? DateTime.MinValue)))).ToList();
                    }
                    var prolist = projectlist.Select(x => x.ProjectID).ToList();
                    if (0 >= prolist.Count())
                        prolist = new List<string>();
                    var listscenedata = db.Scenes.Where(x =>
                        x.EnterpriseID.Equals(enterpriseId)
                        && x.Address.Contains(address)
                        && !x.Deleted
                        && prolist.Contains(x.ProjectID)
                        && !((x.BeginDate > (endTime ?? DateTime.MaxValue)) || (x.EndDate < (beginTime ?? DateTime.MinValue)))).ToList();
                    var endCount = listprojectdata.Where(x => x.Status == (byte)EnterpriseData.Common.Status.End).Count();
                    var ingCount = listprojectdata.Where(x => x.Status == (byte)EnterpriseData.Common.Status.Begin).Count();
                    var readyCount = listprojectdata.Where(x => x.Status == (byte)EnterpriseData.Common.Status.Ready).Count();
                    var endCountscene = listscenedata.Where(x => x.Status == (byte)EnterpriseData.Common.Status.End).Count();
                    var ingCountscene = listscenedata.Where(x => x.Status == (byte)EnterpriseData.Common.Status.Begin).Count();
                    var readyCountscene = listscenedata.Where(x => x.Status == (byte)EnterpriseData.Common.Status.Ready).Count();
                    return new ProjectSceneCompletionStatisticsDto()
                    {
                        ProjectData = new CompletionStatistics()
                        {
                            EndCount = endCount,
                            IngCount = ingCount,
                            ReadyCount = readyCount,
                        },
                        SceneData = new CompletionStatistics()
                        {
                            EndCount = endCountscene,
                            IngCount = ingCountscene,
                            ReadyCount = readyCountscene,
                        }
                    };
                }
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
    }
}
