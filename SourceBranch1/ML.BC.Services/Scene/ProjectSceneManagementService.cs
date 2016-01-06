using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Common;
using ML.BC.EnterpriseData.Model;
using ML.BC.EnterpriseData.MongoDb;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.MsmqHelper;
using ML.BC.Services.Common;

namespace ML.BC.Services
{
    public class ProjectSceneManagementService : IProjectSceneManagementService
    {
        public List<ProjectDto> SearchProjectOnDepartment(string projectId, string projectName, string managerName, ML.BC.EnterpriseData.Common.Status status, int? departmentID, string enterpriseId, int pageSize, int pageNumber, out int amount)
        {
            try
            {
                if (string.IsNullOrEmpty(managerName))
                    managerName = "";
                using (var db = new BCEnterpriseContext())
                {
                    //if (string.IsNullOrEmpty(projectId))
                    //    projectId = "";
                    //if (string.IsNullOrEmpty(projectName))
                    //    projectName = ""; 
                    var query = db.Projects.Where(n => ((projectId ?? "") == "" || n.ProjectID.Contains(projectId)));
                    query = query.Where(n => ((projectName ?? "") == "" || n.Name.Contains(projectName)));
                    query = query.Where(n => ((int)status == 4 || n.Status == (int)status));
                    query = query.Where(n => n.EnterpriseID == enterpriseId && !n.Deleted);
                    //                    query = query.Where(n => ((managerName ?? "") == "") || n.Managers.Contains(managerName));
                    var list = query.ToList();
                    var uidlist = db.FrontUsers.Where(x => x.EnterpiseID.Equals(enterpriseId) && x.Name.Contains(managerName)).Select(n => n.UserID).ToList();
                    list = list.Where(x => x.Managers.Split("|".ToCharArray()).Any(n => uidlist.Any(m => m == n))).ToList();
                    if (departmentID.HasValue)
                    {
                        var ids = GetMyDepartment(departmentID, enterpriseId);
                        list = list.Where(n => n.Departments.Split("|".ToCharArray()).Any(m => ids.Any(k => m == k)))
                            .ToList();
                    }


                    //                using(var db = new BCEnterpriseContext())
                    //                {
                    //                    var list = db.Projects.Select(x=>x);
                    //                    var dpamtIdlist = GetMyDepartment(departmentID,enterpriseId);
                    //                    List<string> listid = new List<string>();
                    //                    foreach (var st in dpamtIdlist)
                    //                    {
                    //                        var ids = db.Projects.Where(x => x.Departments.Contains(st + "|")&&x.Deleted == false).Select(n => n.ProjectID).ToList();
                    //                        listid.AddRange(ids);
                    //                    }
                    //                    if (status == ML.BC.EnterpriseData.Common.Status.All)
                    //                    {
                    //                        var templist = listid.Where()
                    //                        List<string> tempstr = new List<string>();

                    //                        listid = listid.Where(n => n.Contains(projectId)).ToList();

                    //                        var pList = (from pj in db.Projects
                    //                                     where listid.Any(n => n.Contains(pj.ProjectID)) && pj.Name.Contains(projectName)&&pj.Managers.Contains(managerName)
                    //                                     select pj).ToList();

                    //                        var list3 = tempstr.Where(x => x.Name.Contains(projectName));
                    //                        list = list3.Where(x=>x.Managers.Contains(managerName));
                    //                    }
                    //                    else
                    //                    {
                    //                        list = db.Projects.Where(x => (ML.BC.EnterpriseData.Common.Status)x.Status == status);
                    //                        var list2 = list.Where(x => x.ProjectID.Equals(projectId));
                    //                        var list3 = list2.Where(x => x.Name.Contains(projectName));
                    //                        list = list3.Where(x => x.Managers.Contains(managerName));
                    //                    }
                    int pagecount;
                    amount = list.Count();
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
                    var relist = list.OrderBy(x => x.ProjectID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).Select(x => new ProjectDto
                    {
                        Name = x.Name,
                        BeginDate = x.BeginDate,
                        Status = (ML.BC.EnterpriseData.Common.Status)x.Status,
                        Deleted = x.Deleted,
                        Departments = x.Departments,
                        EndDate = x.EndDate,
                        EnterpriseID = x.EnterpriseID,
                        Managers = x.Managers,
                        ProjectID = x.ProjectID,
                        RegistDate = x.RegistDate,
                        Roles = x.Roles,
                    }).ToList();
                    foreach (var x in relist)
                    {
                        x.DepartmentName = new List<string>();
                        x.ManagerName = new List<string>();
                        x.RoleName = new List<string>();
                    }
                    //待修改
                    foreach (var obj in relist)
                    {
                        List<string> deltempdepmt = new List<string>();
                        List<string> deltempmag = new List<string>();
                        List<string> deltemprole = new List<string>();
                        //获取部门名字
                        var listdeptmtId = obj.Departments.Split('|').ToList();
                        foreach (var depmt in listdeptmtId)
                        {
                            if (!string.IsNullOrEmpty(depmt))
                            {
                                var intdepmt = Convert.ToInt32(depmt);
                                var temp = db.Departments.FirstOrDefault(x => x.DepartmentID == intdepmt);
                                if (null != temp)
                                {
                                    if (temp.Deleted || !temp.Available)
                                        deltempdepmt.Add(depmt);
                                    obj.DepartmentName.Add(temp.Name);
                                }
                                else
                                {
                                    deltempdepmt.Add(depmt);
                                }
                            }
                        }
                        foreach (var del in deltempdepmt)
                        {
                            listdeptmtId.Remove(del);
                        }
                        //获取管理者名字
                        var maglist = obj.Managers.Split('|').ToList();
                        //                        var managnamelist = db.FrontUsers.Select(x => maglist.Where(n => n.Equals(x.UserID)));
                        foreach (var mnger in maglist)
                        {
                            var temp = db.FrontUsers.FirstOrDefault(x => x.UserID == mnger);
                            if (null != temp)
                            {
                                if (temp.Closed)
                                    deltempmag.Add(mnger);
                                obj.ManagerName.Add(temp.Name);
                            }
                            else
                            {
                                deltempmag.Add(mnger);
                            }
                        }
                        foreach (var del in deltempmag)
                        {
                            maglist.Remove(del);
                        }
                        //获取角色名字
                        var rolelist = obj.Roles.Split('|').ToList();
                        foreach (var role in rolelist)
                        {
                            var sb = Convert.ToInt32(role);
                            var temp = db.RFARoles.FirstOrDefault(x => x.RoleID == sb);
                            if (null != temp)
                            {
                                if (!temp.Available)
                                    deltemprole.Add(role);
                                obj.RoleName.Add(temp.Name);
                            }
                            else
                            {
                                deltemprole.Add(role);
                            }
                        }
                        foreach (var del in deltemprole)
                        {
                            rolelist.Remove(del);
                        }
                        obj.Departments = "";
                        foreach (var deptmt in listdeptmtId)
                        {
                            obj.Departments += deptmt + '|';
                        }
                        if (!string.IsNullOrEmpty(obj.Departments))
                            obj.Departments = obj.Departments.Substring(0, obj.Departments.Length - 1);
                        obj.Managers = "";
                        foreach (var manag in maglist)
                        {
                            obj.Managers += manag + '|';
                        }
                        if (!string.IsNullOrEmpty(obj.Managers))
                            obj.Managers = obj.Managers.Substring(0, obj.Managers.Length - 1);
                        obj.Roles = "";
                        foreach (var ro in rolelist)
                        {
                            obj.Roles += ro + '|';
                        }
                        if (!string.IsNullOrEmpty(obj.Roles))
                            obj.Roles = obj.Roles.Substring(0, obj.Roles.Length - 1);
                    }
                    return relist;
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
                //获取本部门以及所有子部门
                var list = db.Departments.Where(x => x.ParentID == departmentId && x.Deleted == false);
                if (null == list)
                    return list;
                return list.ToList().Concat(list.ToList().SelectMany(x => GetOwnerDepartment(x.DepartmentID, db)));
                //var list3 = list2;
                //foreach (var l in list2)ee e4
                //{
                //    list3 = list3.Union(GetOwnerDepartment(l.DepartmentID, db));
                //}
                //return list3;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private List<string> GetMyDepartment(int? departmentId, string enterpriseId)
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
                    if (null == dtmt)
                        throw new KnownException("不存在该部门");
                    var list = dtmt.Concat(GetOwnerDepartment(dtmt.First().DepartmentID, db));
                    return list.Select(x => x.DepartmentID.ToString()).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<ScenesDto> SearchSceneOnDepartment(string sceneName, string projectName, int? departmentID, string enterpriseId, int pageSize, int pageNumber, out int amount)
        {
            try
            {
                //修复现场根节点
                //               fixRoot(enterpriseId);
                if (string.IsNullOrEmpty(sceneName))
                    sceneName = "";
                if (string.IsNullOrEmpty(projectName))
                    projectName = "";
                using (var db = new BCEnterpriseContext())
                {
                    //                    list = list.Where(x => x.Managers.Split("|".ToCharArray()).Any(n => uidlist.Any(m => m == n))).ToList();
                    //根据条件搜索
                    var query = db.Scenes.Where(n => (((sceneName ?? "") == "" || n.Name.Contains(sceneName))) && !n.Deleted && n.EnterpriseID.Equals(enterpriseId)).ToList();
                    var projlist = db.Projects.Where(x => !x.Deleted && x.Name.Contains(projectName)).ToList();
                    if (departmentID.HasValue)
                    {
                        //获取数据范围限定为本部门及其所有子部门
                        var ids = GetMyDepartment(departmentID, enterpriseId);
                        projlist = projlist.Where(n => n.Departments.Split("|".ToCharArray()).Any(m => ids.Any(k => m == k)))
                            .ToList();
                    }
                    query = query.Where(x => projlist.Select(n => n.ProjectID).Contains(x.ProjectID)).ToList();
                    //var dpamtIdlist = GetMyDepartment(departmentID, enterpriseId);
                    //List<string> listid = new List<string>();
                    //foreach (var st in dpamtIdlist)
                    //{
                    //    var ids = db.Projects.Where(x => x.Departments.Contains(st + "|") && x.Deleted == false).Select(n => n.ProjectID).ToList();
                    //    listid.AddRange(ids);
                    //}
                    //var proIdlist = db.Projects.Where(x=>null!=db.Projects.Select(t=>t.Departments).ToList().Intersect(dpamtIdlist)).Select(n=>n.ProjectID);
                    //var sceneList = db.Scenes.Where(x => x.Name.Contains(sceneName) && listid.Contains(x.ProjectID) && x.Deleted == false);
                    //var projList = db.Projects.Where(n => n.Name.Contains(projectName)).ToList();
                    // var temp = db.Scenes.Where(x => x.Name.Contains(sceneName)&&null!= listid.Contains(x.ProjectID)&&x.Deleted == false);
                    //var list1 = db.Projects.Where(n => null != n.Name.Contains(projectName));
                    //var scenelist2 = sceneList;
                    //foreach (var proj in projList)
                    //{
                    //    scenelist2 = scenelist2.Union(sceneList.Where(scene => scene.ProjectID.Equals(proj.ProjectID)));
                    //}
                    int pagecount;
                    amount = query.Count();
                    if (pageSize > 0)
                    {
                        // 获取总共页数
                        pagecount = (query.Count() + pageSize - 1) / pageSize;
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
                    var relist = query.OrderBy(x => x.ProjectID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).Select(x => new ScenesDto
                    {
                        Name = x.Name,
                        BeginDate = x.BeginDate,
                        Status = (ML.BC.EnterpriseData.Common.Status)x.Status,
                        Deleted = x.Deleted,
                        Address = x.Address,
                        LatitudeAndLongitude = x.LatitudeAndLongitude,
                        SceneID = x.SceneID,
                        EndDate = x.EndDate,
                        EnterpriseID = x.EnterpriseID,
                        //EnterpriseName = 
                        //Wokers = ML.BC.Infrastructure.Serializer.FromJson<List<GroupedUser>>(x.Woker),
                        tempUsers = x.Woker,
                        ProjectID = x.ProjectID,
                        RegistDate = x.RegistDate,
                        ParentSceneID = x.ParentSceneID,
                        HasData = x.HasData
                        //ProjectName = list1.First(n=>n.ProjectID.Equals(x.ProjectID)).Name
                    }).ToList();
                    foreach (var scene in relist)
                    {
                        try
                        {
                            scene.Wokers = ML.BC.Infrastructure.Serializer.FromJson<List<GroupedUser>>(scene.tempUsers);
                        }
                        catch (Exception)
                        {
                            scene.Wokers = new List<GroupedUser>();
                        }
                    }


                    try
                    {
                        foreach (var scene in relist)
                        {
                            var ent = db.Enterprises.Where(y => y.EnterpriseID.Equals(scene.EnterpriseID)).FirstOrDefault();
                            scene.EnterpriseName = ent == null ? "企业名未找到" : ent.Name;
                            var proj = projlist.Where(n => n.ProjectID.Equals(scene.ProjectID)).FirstOrDefault();
                            scene.ProjectName = proj == null ? "项目名未找到" : proj.Name;
                        }
                    }
                    catch (Exception e)
                    {
                        var message = e.Message;
                    }

                    //foreach (var x in relist)
                    //{
                    //    if (!CheckRoot(db, x.ProjectID))
                    //    {
                    //        var xx = new Scene
                    //        {
                    //            SceneID = new UniqeIdGenerator().GeneratorScenceID(),
                    //            Deleted = false,
                    //            EnterpriseID = x.EnterpriseID,
                    //            ParentSceneID = "-1",
                    //            ProjectID = x.ProjectID,
                    //            RegistDate = DateTime.Now,
                    //            Name = x.Name + "的现场",
                    //            Status = 3,
                    //        };
                    //        db.Scenes.Add(xx);
                    //        if (0 >= db.SaveChanges())
                    //        {
                    //            throw new KnownException("保存現場根失敗");
                    //        }
                    //    }
                    //}
                    return relist;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<ProjectDto> GetAllProjectsOfDepartment(int? departmentID, string enterpriseId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    //获取本部及其所有子部门的id
                    var dpamtIdlist = GetMyDepartment(departmentID, enterpriseId);
                    List<string> listid = new List<string>();
                    foreach (var st in dpamtIdlist)
                    {
                        var ids = db.Projects.Where(x => x.Departments.Contains(st + "|") && x.Deleted == false).Select(n => n.ProjectID).ToList();
                        listid.AddRange(ids);
                    }
                    var list = db.Projects.Where(x => listid.Contains(x.ProjectID)).ToList();
                    //foreach(var x in list)
                    //{

                    //    if (!CheckRoot(db, x.ProjectID))
                    //    {
                    //        var xx = new Scene
                    //        {
                    //            SceneID = new UniqeIdGenerator().GeneratorScenceID(),
                    //            Deleted = false,
                    //            EnterpriseID = x.EnterpriseID,
                    //            ParentSceneID = "-1",
                    //            ProjectID = x.ProjectID,
                    //            RegistDate = DateTime.Now,
                    //            Name = x.Name + "的现场",
                    //            Status = (byte)x.Status,
                    //        };
                    //        db.Scenes.Add(xx);
                    //        if (0 >= db.SaveChanges())
                    //        {
                    //            throw new KnownException("保存現場根失敗");
                    //        }
                    //    }
                    //}
                    return list.Select(x => new ProjectDto
                    {
                        Name = x.Name,
                        BeginDate = x.BeginDate,
                        Status = (ML.BC.EnterpriseData.Common.Status)x.Status,
                        Deleted = x.Deleted,
                        Departments = x.Departments,
                        EndDate = x.EndDate,
                        EnterpriseID = x.EnterpriseID,
                        Managers = x.Managers,
                        ProjectID = x.ProjectID,
                        RegistDate = x.RegistDate,
                        Roles = x.Roles
                    }).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<ScenesDto> GetAllSceneOfProject(string ProjectId, int pageSize, int pageNumber, out int amount)
        {
            try
            {
                //                var jser = new JavaScriptSerializer();
                using (var db = new BCEnterpriseContext())
                {
                    var enterpriseID = db.Projects.Where(p => p.ProjectID.Equals(ProjectId)).First().EnterpriseID;
                    //                    fixRoot(enterpriseID);
                    var list = db.Scenes.Where(x => x.ProjectID.Equals(ProjectId) && x.Deleted == false);
                    int pagecount;
                    amount = list.Count();
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
                    var result = list.Select(x => new ScenesDto
                    {
                        Name = x.Name,
                        BeginDate = x.BeginDate,
                        Status = (ML.BC.EnterpriseData.Common.Status)x.Status,
                        Address = x.Address,
                        LatitudeAndLongitude = x.LatitudeAndLongitude,
                        Deleted = x.Deleted,
                        SceneID = x.SceneID,
                        EndDate = x.EndDate,
                        EnterpriseID = x.EnterpriseID,
                        EnterpriseName = db.Enterprises.FirstOrDefault(y => y.EnterpriseID.Equals(x.EnterpriseID)).Name,
                        tempUsers = x.Woker,
                        //Wokers = ML.BC.Infrastructure.Serializer.FromJson<List<GroupedUser>>(x.Woker),
                        ProjectID = x.ProjectID,
                        RegistDate = x.RegistDate,
                        ParentSceneID = x.ParentSceneID,
                        ProjectName = db.Projects.FirstOrDefault(n => n.ProjectID.Equals(x.ProjectID)).Name,
                        HasData = x.HasData
                    }).OrderBy(x => x.ProjectID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

                    foreach (var scene in result)
                    {
                        try
                        {
                            scene.Wokers = ML.BC.Infrastructure.Serializer.FromJson<List<GroupedUser>>(scene.tempUsers);
                        }
                        catch (Exception)
                        {
                            scene.Wokers = new List<GroupedUser>();
                        }
                    }
                    return result;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string AddProject(ProjectDto project)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var departIdlist = project.Departments.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    var hasInvalidDepart = (from did in departIdlist
                                            join dp in db.Departments on did equals dp.DepartmentID into tempDP
                                            from tdp in tempDP.DefaultIfEmpty()
                                            where tdp == null || tdp.Deleted
                                            select tdp).Any();
                    if (hasInvalidDepart)
                        throw new KnownException("所选部门无效，请刷新后重试");

                    var roleIdlist = project.Roles.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    var hasInvalidRole = (from rid in roleIdlist
                                          join rl in db.RFARoles on rid equals rl.RoleID into tempRL
                                          from trl in tempRL.DefaultIfEmpty()
                                          where trl == null || !trl.Available
                                          select trl).Any();
                    if (hasInvalidRole)
                        throw new KnownException("所选角色无效，请刷新后重试");

                    var managerIdlist = project.Managers.Split('|').ToList();
                    var hasInvalidUser = (from mid in managerIdlist
                                          join u in db.FrontUsers on mid equals u.UserID into tempU
                                          from tu in tempU.DefaultIfEmpty()
                                          where tu == null || tu.Closed
                                          select tu).Any();
                    if (hasInvalidUser)
                        throw new KnownException("所选管理员无效，请刷新后重试");

                    var proj = new Project
                    {
                        ProjectID = new UniqeIdGenerator().GeneratorProjectID(),
                        Name = project.Name,
                        EnterpriseID = project.EnterpriseID,
                        Departments = project.Departments,
                        Managers = project.Managers,
                        Roles = project.Roles,
                        RegistDate = DateTime.Now,
                        BeginDate = project.BeginDate,
                        EndDate = project.EndDate,
                        Deleted = project.Deleted,
                        Status = (byte)project.Status,
                    };
                    var id = db.Projects.Add(proj).ProjectID;
                    db.SaveChanges();
                    return id;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool DeleteProject(string projectId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.Projects.FirstOrDefault(x => x.ProjectID.Equals(projectId));
                    temp.Deleted = true;
                    var list = db.Scenes.Where(x => x.ProjectID.Equals(temp.ProjectID));
                    foreach (var s in list)
                    {
                        s.Deleted = true;
                    }
                    return 0 <= db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool UpdateProject(ProjectDto project)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var departIdlist = project.Departments.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    var hasInvalidDepart = (from did in departIdlist
                                            join dp in db.Departments on did equals dp.DepartmentID into tempDP
                                            from tdp in tempDP.DefaultIfEmpty()
                                            where tdp == null || tdp.Deleted
                                            select tdp).Any();
                    if (hasInvalidDepart)
                        throw new KnownException("所选部门无效，请刷新后重试");

                    var roleIdlist = project.Roles.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                    var hasInvalidRole = (from rid in roleIdlist
                                          join rl in db.RFARoles on rid equals rl.RoleID into tempRL
                                          from trl in tempRL.DefaultIfEmpty()
                                          where trl == null || !trl.Available
                                          select trl).Any();
                    if (hasInvalidRole)
                        throw new KnownException("所选角色无效，请刷新后重试");

                    var managerIdlist = project.Managers.Split('|').ToList();
                    var hasInvalidUser = (from mid in managerIdlist
                                          join u in db.FrontUsers on mid equals u.UserID into tempU
                                          from tu in tempU.DefaultIfEmpty()
                                          where tu == null || tu.Closed
                                          select tu).Any();
                    if (hasInvalidUser)
                        throw new KnownException("所选管理员无效，请刷新后重试");

                    var temp = db.Projects.FirstOrDefault(x => x.ProjectID.Equals(project.ProjectID));
                    if (null == temp)
                        throw new KnownException("此项目不存在，请刷新后重试。");
                    temp.Name = project.Name;
                    //                   temp.EnterpriseID = project.EnterpriseID;
                    temp.Departments = project.Departments;
                    temp.Managers = project.Managers;
                    //                    temp.Roles = project.Roles;
                    //                    temp.RegistDate = DateTime.Now;
                    temp.BeginDate = project.BeginDate;
                    temp.EndDate = project.EndDate;
                    //                    temp.Deleted = project.Deleted;
                    temp.Status = (byte)project.Status;
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string AddScene(ScenesDto scene)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var parentScene = db.Scenes.FirstOrDefault(s => s.SceneID.Equals(scene.ParentSceneID) && !s.Deleted);
                    if (scene.ParentSceneID != "-1" && null == parentScene)
                        throw new KnownException("所选父现场不存在，请重新操作");
                    if ((parentScene ?? new EnterpriseData.Model.Scene() { HasData = false }).HasData)
                        throw new KnownException("所选现场不能作为父级现场");
                    if ((parentScene ?? new EnterpriseData.Model.Scene() { Status = 1}).Status == 3)
                    {
                        throw new KnownException("所选父现场已经完工，不能操作");
                    }
                    var project = db.Projects.FirstOrDefault(x => x.ProjectID == scene.ProjectID && !x.Deleted);
                    if (null == project)
                        throw new KnownException("所选项目不存在，请刷新后重试");
                    var scenee = new EnterpriseData.Model.Scene
                    {
                        SceneID = new UniqeIdGenerator().GeneratorScenceID(),
                        BeginDate = scene.BeginDate,
                        Deleted = scene.Deleted,
                        EndDate = scene.EndDate,
                        EnterpriseID = scene.EnterpriseID,
                        ParentSceneID = scene.ParentSceneID,
                        ProjectID = scene.ProjectID,
                        RegistDate = DateTime.Now,
                        Name = scene.Name,
                        Address = scene.Address,
                        LatitudeAndLongitude = scene.LatitudeAndLongitude,
                        Status = (byte)scene.Status,
                        Woker = ML.BC.Infrastructure.Serializer.ToJson(scene.Wokers),
                    };
                    var id = db.Scenes.Add(scenee).SceneID;
                    db.SaveChanges();
                    return id;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private int DeleteSceneChildren(BCEnterpriseContext db, string sceneId)
        {
            var temp = db.Scenes.FirstOrDefault(x => x.SceneID.Equals(sceneId));
            if (null == temp)
            {
                return 0;
            }
            //if (temp.Status != (int)EnterpriseData.Common.Status.End)
            temp.Deleted = true;
            //else
            //    throw new KnownException("该现场已完工或者包含已完工的子现场，无法删除");
            var list = db.Scenes.Where(x => x.ParentSceneID.Equals(sceneId)).ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                if (0 >= DeleteSceneChildren(db, list[i].SceneID))
                    break;
            }
            return db.SaveChanges();
        }
        public bool DeleteScene(string sceneId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var Scene = db.Scenes.FirstOrDefault(s => s.SceneID.Equals(sceneId));
                    if (Scene != null && Scene.Status == 3)
                    {
                        throw new KnownException("所选现场已经完工，不能操作");
                    }
                    return 0 < DeleteSceneChildren(db, sceneId);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private bool CheckRoot(BCEnterpriseContext db, string projectId)
        {
            var temp = db.Projects.FirstOrDefault(x => x.ProjectID.Equals(projectId));
            return null == temp;
        }

        /// <summary>
        /// 修复某企业的所有项目的根
        /// </summary>
        /// <param name="_enterpriseID"></param>
        public void fixRoot(string _enterpriseID)
        {
            //return;//即将 不使用虚拟的根了
            using (var db = new BCEnterpriseContext())
            {
                var projList = db.Projects.Where(p => p.EnterpriseID.Equals(_enterpriseID)).ToList();
                foreach (var proj in projList)
                {
                    var rootList = db.Scenes.Where(s => s.ProjectID.Equals(proj.ProjectID) && s.ParentSceneID.Equals("-1")).ToList();
                    if (rootList.Count != 1)
                    {
                        if (rootList.Count > 1)
                        {
                            for (int i = 0; i < rootList.Count() - 1; i++)
                            {
                                db.Scenes.Remove(rootList[i]);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            var rootScene = new EnterpriseData.Model.Scene
                            {
                                SceneID = new UniqeIdGenerator().GeneratorScenceID(),
                                Name = "[项目]" + proj.Name,
                                EnterpriseID = _enterpriseID,
                                ProjectID = proj.ProjectID,
                                ParentSceneID = "-1",
                                RegistDate = DateTime.Now,
                                Status = 3,
                                Deleted = false
                            };
                            db.Scenes.Add(rootScene);
                            db.SaveChanges();
                        }
                    }
                }

            }
        }

        public bool SetSceneStatus(string sceneID, ML.BC.EnterpriseData.Common.Status status)
        {
            try
            {
                var mgdb = new MongoDbProvider<SceneItem>();
                var sceneItems = mgdb.GetAll(o => o.SceneID == sceneID && o.Status != ItemStatus.Final).ToList();
                if (0 != sceneItems.Count)
                {
                    foreach (var item in sceneItems)
                    {
                        item.Status = ItemStatus.Final;
                        mgdb.Update(item);
                    }
                }

                using (var db = new BCEnterpriseContext())
                {
                    var scene = db.Scenes.FirstOrDefault(o => o.SceneID == sceneID);
                    if (null == scene) return false;
                    scene.Status = (byte)status;
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool UpdateScene(ScenesDto scene)
        {
            try
            {
                if (string.IsNullOrEmpty(scene.SceneID))
                    throw new KnownException("现场ID不能为空");

                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.Scenes.FirstOrDefault(x => x.SceneID.Equals(scene.SceneID));
                    if (null == temp)
                        return false;
                    if (temp.Status == 3)
                    {
                        throw new KnownException("所选现场已经完工，不能操作");
                    }
                    temp.BeginDate = scene.BeginDate;
                    //temp.Deleted = scene.Deleted;
                    temp.EndDate = scene.EndDate;
                    temp.Address = scene.Address;
                    temp.LatitudeAndLongitude = scene.LatitudeAndLongitude;
                    //temp.EnterpriseID = scene.EnterpriseID;
                    //temp.ParentSceneID = scene.ParentSceneID;
                    // temp.ProjectID = scene.ProjectID;
                    //temp.RegistDate = scene.RegistDate;
                    temp.Name = scene.Name;
                    temp.Status = (byte)scene.Status;
                    if (temp.Status == (byte)Status.End)
                    {
                        var mgdb = new MongoDbProvider<SceneItem>();
                        var sceneItems = mgdb.GetAll(o => o.SceneID == scene.SceneID && o.Status != ItemStatus.Final).ToList();
                        if (0 != sceneItems.Count)
                        {
                            foreach (var item in sceneItems)
                            {
                                item.Status = ItemStatus.Final;
                                mgdb.Update(item);
                            }
                        }
                    }
                    temp.Woker = ML.BC.Infrastructure.Serializer.ToJson(scene.Wokers);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<ProjectDto> GetAllProjectsOfDepartment(int? departmentId, string enterpriseId, int pageSize, int pageNumber, int amount)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var dpamtIdlist = GetMyDepartment(departmentId, enterpriseId);
                    List<string> listid = new List<string>();
                    foreach (var st in dpamtIdlist)
                    {
                        var ids = db.Projects.Where(x => x.Departments.Contains(st + "|") && x.Deleted == false).Select(n => n.ProjectID).ToList();
                        listid.AddRange(ids);
                    }
                    var list = db.Projects.Where(x => listid.Contains(x.ProjectID));
                    int pagecount;
                    amount = list.Count();
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
                    var relist = list.OrderBy(x => x.ProjectID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).Select(x => new ProjectDto
                    {
                        Name = x.Name,
                        BeginDate = x.BeginDate,
                        Status = (ML.BC.EnterpriseData.Common.Status)x.Status,
                        Deleted = x.Deleted,
                        Departments = x.Departments,
                        EndDate = x.EndDate,
                        EnterpriseID = x.EnterpriseID,
                        Managers = x.Managers,
                        ProjectID = x.ProjectID,
                        RegistDate = x.RegistDate,
                        Roles = x.Roles,
                    }).ToList();
                    foreach (var x in relist)
                    {
                        x.DepartmentName = new List<string>();
                        x.ManagerName = new List<string>();
                        x.RoleName = new List<string>();
                    }
                    foreach (var obj in relist)
                    {
                        var listdeptmtId = obj.Departments.Split('|').ToList();
                        foreach (var depmt in listdeptmtId)
                        {
                            if (!string.IsNullOrEmpty(depmt))
                            {
                                var sb = Convert.ToInt32(depmt);
                                var temp = db.Departments.FirstOrDefault(x => x.DepartmentID == sb);
                                obj.DepartmentName.Add(temp.Name);
                            }
                        }
                        var maglist = obj.Managers.Split('|').ToList();
                        foreach (var mnger in maglist)
                        {
                            var temp = db.FrontUsers.FirstOrDefault(x => x.UserID == mnger);
                            obj.ManagerName.Add(temp.Name);
                        }
                        var rolelist = obj.Roles.Split('|').ToList();
                        foreach (var role in rolelist)
                        {
                            var sb = Convert.ToInt32(role);
                            var temp = db.RFARoles.FirstOrDefault(x => x.RoleID == sb);
                            obj.RoleName.Add(temp.Name);
                        }
                    }
                    return relist;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ProjectAndSceneSyncDto GetProjectAndSceneForSync(string enterpriseId)
        {
            using (var db = new BCEnterpriseContext())
            {
                var query = (from ep in db.Enterprises
                             where ep.EnterpriseID == enterpriseId
                             join pj in db.Projects on ep.EnterpriseID equals pj.EnterpriseID into tempPJ
                             join sn in db.Scenes on ep.EnterpriseID equals sn.EnterpriseID into tempSN
                             select new
                             {
                                 Projects = (
                                 from tpj in tempPJ
                                 where !tpj.Deleted
                                 select new
                                 {
                                     ep.EnterpriseID,
                                     tpj.ProjectID,
                                     tpj.Name,
                                     tpj.Status,
                                     tpj.Roles,
                                     tpj.Departments,
                                     tpj.Managers,
                                     HaveScene = tempSN.Any(n => n.ProjectID == tpj.ProjectID),
                                     tpj.Deleted
                                 }
                                 ),
                                 Scenes = (
                                 from tsn in tempSN
                                 where !tsn.Deleted
                                 select new
                                 {
                                     tsn.SceneID,
                                     tsn.Name,
                                     tsn.Woker,
                                     tsn.Address,
                                     tsn.BeginDate,
                                     tsn.EndDate,
                                     tsn.ParentSceneID,
                                     tsn.ProjectID,
                                     tsn.Status,
                                     tsn.Type,
                                     tsn.HasData,
                                     tsn.Deleted
                                 }
                                 )
                             }).FirstOrDefault();
                if (query == null) return new ProjectAndSceneSyncDto();

                var result = new ProjectAndSceneSyncDto
                {
                    Projects = query.Projects.Select(n => new ProjectSyncDto
                    {
                        EnterpriseID = n.EnterpriseID,
                        ProjectID = n.ProjectID,
                        ProjectName = n.Name,
                        ProjectState = n.Status,
                        ProjectRoles = n.Roles,
                        Departments = n.Departments,
                        HaveScene = n.HaveScene,
                        Manager = n.Managers,
                        Deleted = n.Deleted
                    }).ToArray(),
                    Scenes = query.Scenes.Select(n => new SceneSyncDto
                    {
                        SceneID = n.SceneID,
                        SceneName = n.Name,
                        SceneWorker = SerializerSceneWorkers(n.Woker),
                        Address = n.Address,
                        BeginDate = n.BeginDate,
                        EndDate = n.EndDate,
                        ParentID = n.ParentSceneID,
                        ProjectID = n.ProjectID,
                        SceneState = n.Status,
                        SceneType = n.Type,
                        HasData = n.HasData,
                        Deleted = n.Deleted
                    }).ToArray()
                };
                FillAllWorkers(db, result);
                return result;
            }
        }

        private ProjectAndSceneSyncDto FillAllWorkers(BCEnterpriseContext db, ProjectAndSceneSyncDto projectAndSceneSyncDto)
        {
            if (projectAndSceneSyncDto == null) return null;

            Func<List<SceneSyncDto>, string, List<GroupedUser>> getSubSceneWorkers = null;
            getSubSceneWorkers = (list, parentSceneID) =>
            {
                var result = new List<GroupedUser>();
                list.Where(n => n.ParentID == parentSceneID)
                    .ToList()
                    .ForEach(n =>
                    {
                        result.AddRange(n.SceneWorker);
                        var temp = getSubSceneWorkers(list, n.SceneID);
                        result.AddRange(temp);
                    });

                return result;
            };

            projectAndSceneSyncDto.Scenes.GroupBy(n => n.ProjectID)
                .ToList()
                .ForEach(n =>
                {
                    var sceneList = db.Scenes.Where(s => s.ProjectID == n.Key && !s.Deleted)
                        .Select(s => new { s.SceneID, s.ParentSceneID, s.Woker }).ToList()
                        .Select(s => new SceneSyncDto { SceneID = s.SceneID, ParentID = s.ParentSceneID, SceneWorker = SerializerSceneWorkers(s.Woker) })
                        .ToList();

                    n.ToList().ForEach(s =>
                    {
                        s.AllWorkers = new List<GroupedUser>(s.SceneWorker);
                        var temp = getSubSceneWorkers(sceneList, s.SceneID);
                        s.AllWorkers.AddRange(temp);
                        s.AllWorkers = s.AllWorkers.Distinct().ToList();
                    });
                });

            return projectAndSceneSyncDto;
        }

        public ProjectAndSceneSyncDto GetProjectAndSceneForSync(string userID, string deviceID, string enterpriseID)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    if (null == db.SyncStates.FirstOrDefault(o => (o.UserID == userID) && (o.DeviceID == deviceID)
                        && (o.ActionType == (byte)TypeEnum.Project || o.ActionType == (byte)TypeEnum.Scene)))
                    {
                        var syncProject = new SyncState() { ActionType = (byte)TypeEnum.Project, DeviceID = deviceID, UserID = userID };
                        var syncScene = new SyncState() { ActionType = (byte)TypeEnum.Scene, DeviceID = deviceID, UserID = userID };
                        AddSyncState(syncScene, syncProject);
                        return GetProjectAndSceneForSync(enterpriseID);
                    }

                    var query = (from ep in db.Enterprises
                                 where ep.EnterpriseID == enterpriseID
                                 join pj in db.Projects on ep.EnterpriseID equals pj.EnterpriseID into tempPJ
                                 join sn in db.Scenes on ep.EnterpriseID equals sn.EnterpriseID into tempSN
                                 select new
                                 {
                                     Projects = (
                                     from tpj in tempPJ
                                     from st in db.SyncStates
                                     where st.UserID == userID && st.DeviceID == deviceID && st.ActionType == (byte)TypeEnum.Project && st.SyncTime < tpj.UpdateTime
                                     select new
                                     {
                                         ep.EnterpriseID,
                                         tpj.ProjectID,
                                         tpj.Name,
                                         tpj.Status,
                                         tpj.Roles,
                                         tpj.Departments,
                                         tpj.Managers,
                                         HaveScene = tempSN.Any(n => n.ProjectID == tpj.ProjectID),
                                         tpj.Deleted
                                     }
                                     ),
                                     Scenes = (
                                     from tsn in tempSN
                                     from st in db.SyncStates
                                     where st.UserID == userID && st.DeviceID == deviceID && st.ActionType == (byte)TypeEnum.Scene && st.SyncTime < tsn.UpdateTime
                                     select new
                                     {
                                         tsn.SceneID,
                                         tsn.Name,
                                         tsn.Woker,
                                         tsn.Address,
                                         tsn.BeginDate,
                                         tsn.EndDate,
                                         tsn.ParentSceneID,
                                         tsn.ProjectID,
                                         tsn.Status,
                                         tsn.Type,
                                         tsn.HasData,
                                         tsn.Deleted
                                     }
                                     )
                                 }).FirstOrDefault();
                    if (query == null) return new ProjectAndSceneSyncDto();

                    var result = new ProjectAndSceneSyncDto
                    {
                        Projects = query.Projects.Select(n => new ProjectSyncDto
                        {
                            EnterpriseID = n.EnterpriseID,
                            ProjectID = n.ProjectID,
                            ProjectName = n.Name,
                            ProjectState = n.Status,
                            ProjectRoles = n.Roles,
                            Departments = n.Departments,
                            HaveScene = n.HaveScene,
                            Manager = n.Managers,
                            Deleted = n.Deleted
                        }).ToArray(),
                        Scenes = query.Scenes.Select(n => new SceneSyncDto
                        {
                            SceneID = n.SceneID,
                            SceneName = n.Name,
                            SceneWorker = SerializerSceneWorkers(n.Woker),
                            Address = n.Address,
                            BeginDate = n.BeginDate,
                            EndDate = n.EndDate,
                            ParentID = n.ParentSceneID,
                            ProjectID = n.ProjectID,
                            SceneState = n.Status,
                            SceneType = n.Type,
                            HasData = n.HasData,
                            Deleted = n.Deleted
                        }).ToArray()
                    };
                    FillAllWorkers(db, result);
                    return result;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private static List<GroupedUser> SerializerSceneWorkers(string workers)
        {
            try
            {
                return string.IsNullOrEmpty(workers) ? new List<GroupedUser>() : ML.BC.Infrastructure.Serializer.FromJson<List<GroupedUser>>(workers);
            }
            catch { }
            return new List<GroupedUser>();
        }

        private const string ADDSYNCSTATELOCK = "ADDSYNCSTATELOCK";

        private bool AddSyncState(params SyncStateDto[] syncStateDtos)
        {
            lock (ADDSYNCSTATELOCK)
            {
                try
                {
                    if (null == syncStateDtos || syncStateDtos.Length == 0)
                        throw new ArgumentNullException("syncStateDtos");
                    for (int i = 0; i < syncStateDtos.Length; i++)
                    {
                        if (string.IsNullOrEmpty(syncStateDtos[i].UserID) ||
                            string.IsNullOrEmpty(syncStateDtos[i].DeviceID) || syncStateDtos[i].ActionType < 0)
                            throw new KnownException("对象索引:" + i + " 缺少必要信息，无法添加或更新！");
                    }

                    using (var db = new BCEnterpriseContext())
                    {
                        syncStateDtos.ToList().ForEach(m => m.SyncTime = DateTime.Now);
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
                        var oldlist =
                            db.SyncStates.Where(
                                m =>
                                    userids.Contains(m.UserID) && deviceIDs.Contains(m.DeviceID) &&
                                    actionTypes.Contains(m.ActionType));
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

        //public ProjectAndSceneSyncDto GetProjectAndSceneForSync(string userID, string deviceID, string enerpriseID)
        //{
        //    using (var db = new BCEnterpriseContext())
        //    {
        //        //  同步表中无此用户和设备ID时，创建此记录并将Organization数据初始化发送给APP
        //        if (null == db.SyncStates.FirstOrDefault(o => (o.UserID == userID) && (o.DeviceID == deviceID)))
        //        {
        //            var syncProject = new SyncState() { ActionType = (byte)TypeEnum.Project, DeviceID = deviceID, UserID = userID };
        //            var syncScene = new SyncState() { ActionType = (byte)TypeEnum.Scene, DeviceID = deviceID, UserID = userID };
        //            //AddSyncState(syncProject, syncScene);
        //            //return GetEnterpriseForSync(enterpriseID);
        //        }
        //    throw new NotImplementedException();
        //}

        public List<ProjectDto> SearchProjectOnEnterprise(string projectId, string projectName, string managerName, EnterpriseData.Common.Status status, string enterpriseId, int pageSize, int pageNumber, out int amount)
        {
            return SearchProjectOnDepartment(projectId, projectName, managerName, status, null, enterpriseId, pageSize, pageNumber, out amount);
        }

        public List<ScenesDto> SearchSceneOnEnterprise(string sceneName, string projectName, string enterpriseId, int pageSize, int pageNumber, out int amount)
        {
            return SearchSceneOnDepartment(sceneName, projectName, null, enterpriseId, pageSize, pageNumber, out amount);
        }


        public List<KeyValuePair<RoleIdName, List<UserIdName>>> GetRoleUserListOfDepartment(string enterpriseId, int? departmentId, string projectId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    //获取项目下的角色
                    var stringroles = db.Projects.FirstOrDefault(x => x.ProjectID.Equals(projectId) && !x.Deleted);
                    List<string> roleIdlist = new List<string>();
                    //创建KV的List
                    List<KeyValuePair<RoleIdName, List<UserIdName>>> list = new List<KeyValuePair<RoleIdName, List<UserIdName>>>();
                    //获取本项目有效的角色用户映射关系
                    if (null == stringroles)
                        return list;
                    List<int> rolelist = new List<int>();
                    if (!string.IsNullOrEmpty(stringroles.Roles))
                    {
                        roleIdlist = stringroles.Roles.Split('|').ToList();
                        foreach (var r in roleIdlist)
                        {
                            rolelist.Add(Convert.ToInt32(r));
                        }
                    }
                    else
                    {
                        return list;
                    }
                    //获取部门权限
                    var mydepartments = GetMyDepartment(departmentId, enterpriseId);
                    //获取本部和子部所有人员
                    var users = db.FrontUsers.Where(x => mydepartments.Contains(x.DepartmentID.ToString()) && x.EnterpiseID.Equals(enterpriseId)).Select(n => n.UserID);
                    List<string> uidlist = new List<string>();

                    //var uidl = (from userRole in db.UserRoles
                    //            join usr in db.FrontUsers on userRole.UserID equals usr.UserID into tUSR
                    //            from user in tUSR.DefaultIfEmpty()
                    //            where rolelist.Contains(userRole.RoleID) && user.EnterpiseID == enterpriseId && userRole.Deleted == false && user.Closed == false
                    //            select new
                    //            {
                    //                userRole.UserID
                    //            }).ToList();

                    foreach (var roleid in rolelist)
                    {
                        //一角色对应多人员

                        uidlist = (from userRole in db.UserRoles
                                   join usr in db.FrontUsers on userRole.UserID equals usr.UserID into tUSR
                                   from user in tUSR.DefaultIfEmpty()
                                   where roleid == userRole.RoleID && user.EnterpiseID == enterpriseId && userRole.Deleted == false && user.Closed == false
                                   select
                                   userRole.UserID
                                    ).ToList();

                        list.Add(new KeyValuePair<RoleIdName, List<UserIdName>>(db.RFARoles.Where(x => x.RoleID == roleid && x.Available).Select(n => new RoleIdName
                        {
                            RoleId = n.RoleID.ToString(),
                            RoleName = n.Name
                        }).FirstOrDefault(), db.FrontUsers.Where(x => uidlist.Contains(x.UserID)).Select(n => new UserIdName
                        {
                            UserId = n.UserID,
                            UserName = n.Name
                        }).ToList()));
                    }


                    //foreach (var roleid in rolelist)
                    //{
                    //    //一角色对应多人员
                    //
                    //    uidlist = db.UserRoles.Where(x => x.RoleID == roleid).Select(n => n.UserID).ToList();
                    //    list.Add(new KeyValuePair<RoleIdName, List<UserIdName>>(db.RFARoles.Where(x => x.RoleID == roleid && x.Available).Select(n => new RoleIdName
                    //     {
                    //         RoleId = n.RoleID.ToString(),
                    //         RoleName = n.Name
                    //     }).FirstOrDefault(), db.FrontUsers.Where(x => uidlist.Contains(x.UserID)).Select(n => new UserIdName
                    //     {
                    //         UserId = n.UserID,
                    //         UserName = n.Name
                    //     }).ToList()));
                    //}

                    return list;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<KeyValuePair<RoleIdName, List<UserIdName>>> GetRoleUserListOfEnterprise(string enterpriseId, string projectId)
        {
            return GetRoleUserListOfDepartment(enterpriseId, null, projectId);
        }

        public List<KeyValuePair<RoleIdName, List<UserIdName>>> GetRoleUserListOfSceneInDepartment(string enterpriseId, int? departmentId, string projectId, string sceneID)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    //获取项目下的角色字符串
                    var stringroles = db.Projects.FirstOrDefault(x => x.ProjectID.Equals(projectId) && !x.Deleted);
                    List<string> roleIdlist = new List<string>();
                    //创建KVlist
                    List<KeyValuePair<RoleIdName, List<UserIdName>>> list = new List<KeyValuePair<RoleIdName, List<UserIdName>>>();
                    if (null == stringroles)
                        return list;
                    //获取本项目有效的角色用户映射关系
                    List<int> rolelist = new List<int>();
                    if (!string.IsNullOrEmpty(stringroles.Roles))
                    {
                        roleIdlist = stringroles.Roles.Split('|').ToList();
                        foreach (var r in roleIdlist)
                        {
                            rolelist.Add(Convert.ToInt32(r));
                        }
                    }
                    else
                    {
                        return list;
                    }
                    //获取子部门和本部门
                    var mydepartments = GetMyDepartment(departmentId, enterpriseId);
                    //获取本部及子部的所有用户
                    var users = db.FrontUsers.Where(x => mydepartments.Contains(x.DepartmentID.ToString()) && x.EnterpiseID.Equals(enterpriseId)).Select(n => n.UserID);
                    //获取本现场所有有效的workers
                    var userids = db.Scenes.FirstOrDefault(x => x.ProjectID.Equals(projectId) && x.EnterpriseID.Equals(enterpriseId) && !x.Deleted && x.SceneID.Equals(sceneID));
                    List<string> myuids = new List<string>();
                    if (!string.IsNullOrEmpty(userids.Woker))
                    {
                        myuids = userids.Woker.Split('|').ToList();
                    }
                    IEnumerable<string> uidlist = null;
                    IEnumerable<string> finalusers = null;
                    foreach (var roleid in rolelist)
                    {
                        //获取实际选择的人员
                        uidlist = db.UserRoles.Where(x => x.RoleID == roleid).Select(n => n.UserID).ToList();
                        finalusers = myuids.Intersect(uidlist);
                        list.Add(new KeyValuePair<RoleIdName, List<UserIdName>>(db.RFARoles.Where(x => x.RoleID == roleid && x.Available).Select(n => new RoleIdName
                        {
                            RoleId = n.RoleID.ToString(),
                            RoleName = n.Name
                        }).FirstOrDefault(), db.FrontUsers.Where(x => finalusers.Contains(x.UserID)).Select(n => new UserIdName
                        {
                            UserId = n.UserID,
                            UserName = n.Name
                        }).ToList()));
                    }
                    return list;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<KeyValuePair<RoleIdName, List<UserIdName>>> GetRoleUserListOfSceneInEnterprise(string enterpriseId, string projectId, string sceneID)
        {
            return GetRoleUserListOfSceneInDepartment(enterpriseId, null, projectId, sceneID);
        }


        public EnterpriseData.Model.Scene GetOneScene(string id)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var r = db.Scenes.FirstOrDefault(s => s.SceneID.Equals(id));
                    return r;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
