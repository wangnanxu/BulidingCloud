using System;
using ML.BC.EnterpriseData.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Common;
using ML.BC.EnterpriseData.Model.Extend;
using ML.BC.Infrastructure.Exceptions;

namespace ML.BC.Services
{
    public class Scan : IScan
    {
        public bool AddScan(ScanDto scan)
        {
            try
            {
                if (string.IsNullOrEmpty(scan.ObjectID) || string.IsNullOrEmpty(scan.UserID))
                {
                    return false;
                }
                using (var db = new BCEnterpriseContext())
                {
                    var scanlog =
                        db.ScanLog.FirstOrDefault(x => x.ObjectID.Equals(scan.ObjectID) && x.UserID.Equals(scan.UserID) && x.Type == (int)scan.Type);
                    if (null != scanlog)
                    {
                        return false;
                    }
                    if (scan.Type == ScanType.Project)
                    {
                        db.ScanLog.Add(new ScanLog()
                        {
                            UserID = scan.UserID,
                            ObjectID = scan.ObjectID,
                            Type = (int)scan.Type,
                            Time = DBTimeHelper.DBNowTime()
                        });
                        return 0 < db.SaveChanges();
                    }
                    else
                    {
                        return ScanSceneParents(scan.UserID, scan.ObjectID, db);
                    }
                    //var temp = (scan.Type == ScanType.Project) ? (null == db.ScanLog.Add(new ScanLog()
                    //{
                    //    UserID = scan.UserID,
                    //    ObjectID = scan.ObjectID,
                    //    Type = (int)scan.Type,
                    //    Time = DBTimeHelper.DBNowTime()
                    //})) : ScanSceneParents(scan.UserID, scan.ObjectID,db);
                    //return temp;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private IEnumerable<EnterpriseData.Model.Scene> GetParentsScenes(string sceneID, BCEnterpriseContext db)
        {
            try
            {
                var list = db.Scenes.Where(x => x.SceneID.Equals(sceneID) && !x.Deleted);
                if (0 == list.Count())
                {
                    return list;
                }
                return list.ToList().Concat(list.ToList().SelectMany(x => GetParentsScenes(x.ParentSceneID, db)));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private bool ScanSceneParents(string userID, string sceneID, BCEnterpriseContext db)
        {
            try
            {
                var templist = GetParentsScenes(sceneID, db);
                var list = templist.Select(x => x.SceneID).ToList();
                var scenelist = db.ScanLog.Where(x => list.Contains(x.ObjectID) && x.Type == (int)ScanType.Scene && x.UserID.Equals(userID)).Select(n => n.ObjectID).ToList();
                var sc = new List<string>();
                sc.Add(sceneID);
                if (0 < scenelist.Count())
                {
                    sc = list.Except(scenelist).ToList();
                    sc.Distinct();
                }

                if (1 < sc.Count())
                {
                    foreach (var scene in sc)
                    {
                        var obj = new ScanLog()
                        {
                            UserID = userID,
                            ObjectID = scene,
                            Type = (int)ScanType.Scene,
                            Time = DBTimeHelper.DBNowTime()
                        };
                        db.ScanLog.Add(obj);
                    }
                }
                else
                {
                    var obj = new ScanLog()
                    {
                        UserID = userID,
                        ObjectID = sceneID,
                        Type = (int)ScanType.Scene,
                        Time = DBTimeHelper.DBNowTime()
                    };
                    db.ScanLog.Add(obj);
                }
                return 0 < db.SaveChanges();
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
        public ScanReportDto GetScanCount(string userID)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var user = GetMyDepartment(userID, db);
                    var department = GetOwnerDepartment(user.DepartmentID ?? -1, db).Select(x => x.DepartmentID).ToList();
                    if (0 == department.Count())
                    {
                        department = new List<int>();
                    }
                    department.Add(user.DepartmentID ?? -1);
                    var projectScan = db.ScanLog.Where(x => x.UserID.Equals(userID) && x.Type == (int)ScanType.Project);
                    var projectAll =
                        db.Projects.Where(
                            x =>
                                x.EnterpriseID.Equals(user.EnterpiseID) && !x.Deleted &&
                                department.Contains(user.DepartmentID ?? -1)).Select(n => n.ProjectID);
                    var sceneScan = db.ScanLog.Where(x => x.UserID.Equals(userID) && x.Type == (int)ScanType.Scene);
                    var sceneAll = db.Scenes.Where(x => projectAll.Contains(x.ProjectID) && !x.Deleted);
                    return new ScanReportDto()
                    {
                        ProjectAll = projectAll.Count(),
                        ProjectScan = projectScan.Count(),
                        SceneAll = sceneAll.Count(),
                        SceneScan = sceneScan.Count()
                    };
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
