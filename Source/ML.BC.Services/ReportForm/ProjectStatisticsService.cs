using ML.BC.EnterpriseData.Model;
using ML.BC.EnterpriseData.MongoDb;
using ML.BC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    class ProjectStatisticsService : IProjectStatisticsService
    {


        public List<ProjectStatisDto> GetProjectStatisInfo(string enterpriseId, int? departmentID, string projName, int pageSize, int pageIndex, out int amount)
        {

            try
            {
                if (projName == null) projName = "";
                if (pageIndex < 1) pageIndex = 1;
                if (pageSize < 1) pageSize = 10;
                using (var db = new BCEnterpriseContext())
                {
                    var result = new List<ProjectStatisDto>();
                    var depart = GetOwnerDepartment(departmentID ?? 0, db).Select(x => x.DepartmentID.ToString()).ToList();
                    depart.Add((departmentID ?? -1).ToString());
                    var tempProjList = db.Projects.Where(p => (projName.Equals("") || p.Name.Contains(projName)) && p.EnterpriseID.Equals(enterpriseId) && p.Deleted == false).ToList();
                    var allProjList =
                        tempProjList.Where(p => (p.Departments??"").Split("|".ToCharArray()).Any(x => depart.Contains(x)));
                    if ((amount = allProjList.Count()) < 1)
                    {
                        amount = 0;
                        return result;
                    }
                    else
                    {
                        List<Project> specProjList;
                        int totalPage = (amount + pageSize - 1) / pageSize;
                        if (pageIndex > totalPage)
                        {
                            specProjList = allProjList.OrderBy(p => p.ProjectID).Skip((totalPage - 1) * pageSize).Take(pageSize).ToList();
                        }
                        else
                        {
                            specProjList = allProjList.OrderBy(p => p.ProjectID).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                        }
                        var mgdb = new MongoDbProvider<SceneItem>();
                        foreach (var proj in specProjList)
                        {
                            var stat = new ProjectStatisDto();
                            stat.projectId = proj.ProjectID;
                            stat.projectName = proj.Name;
                            //计算各类型数量
                            for (int i = 0; i < 7; i++)
                            {
                                int idx = 0;
                                List<string> sceneIDs = new List<string>();
                                if (i != 0 && i != 5)
                                {
                                    idx = (int)Math.Pow(2, i);
                                    sceneIDs = db.Scenes.Where(s => s.ProjectID.Equals(proj.ProjectID) && s.Deleted == false).Select(s => s.SceneID).ToList();
                                }
                                if (sceneIDs.Count() < 1)
                                {
                                    stat.typeCount[i] = 0;
                                }
                                else
                                {
                                    try
                                    {
                                        stat.typeCount[i] = (int)mgdb.Count(s => sceneIDs.Contains(s.SceneID) && s.Type.Equals(idx));
                                    }
                                    catch (Exception)
                                    {
                                        stat.typeCount[i] = -1;
                                    }
                                }
                            }
                            result.Add(stat);
                        }
                        return result;
                    }
                }
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
        public ProjectStatisDto GetAllProjectStatisInfo(string enterpriseId, int? departmentID, out int amount)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var result = new ProjectStatisDto();
                    var depart = GetOwnerDepartment(departmentID ?? 0, db).Select(x => x.DepartmentID.ToString()).ToList();
                    depart.Add((departmentID ?? -1).ToString());
                    var ProjList = db.Projects.Where(p => p.EnterpriseID.Equals(enterpriseId) && p.Deleted == false).ToList();
                    var allProjList = ProjList.Where(p => p.Departments.Split("|".ToCharArray()).Any(x => depart.Contains(x)));
                    if ((amount = allProjList.Count()) < 1)
                    {
                        amount = 0;
                        return result;
                    }
                    else
                    {
                        var mgdb = new MongoDbProvider<SceneItem>();
                        foreach (var proj in allProjList)
                        {
                            //计算各类型数量
                            for (int i = 0; i < 7; i++)
                            {
                                List<string> sceneIDs = new List<string>();
                                int idx = 0;
                                if (i != 0 && i != 5)
                                {
                                    idx = (int)Math.Pow(2, i);
                                    sceneIDs = db.Scenes.Where(s => s.ProjectID.Equals(proj.ProjectID) && s.Deleted == false).Select(s => s.SceneID).ToList();
                                }
                                if (sceneIDs.Count() < 1)
                                {
                                    result.typeCount[i] += 0;
                                }
                                else
                                {
                                    try
                                    {
                                        result.typeCount[i] += (int)mgdb.Count(s => sceneIDs.Contains(s.SceneID) && s.Type.Equals(idx));
                                    }
                                    catch (Exception)
                                    {
                                        result.typeCount[i] += 0;
                                    }
                                }
                            }
                        }
                        result.projectName = "所有项目";
                        return result;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
