using System;
using ML.BC.EnterpriseData.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.MongoDb;
using MongoDB.Driver.Linq;

namespace ML.BC.Services
{
    public class RectificationStatistical : IRectificationStatistical
    {

        public RectificationStatisticalDto GetRectificationStatistical(string enterpriseID, int? departmentID, string userID)
        {
            try
            {

                var mgdb = new MongoDbProvider<SceneItem>();
                using (var db = new BCEnterpriseContext())
                {
                    var itemAllList = new List<SceneItem>();
                    if (string.IsNullOrEmpty(userID))
                    {
                        var departmentlist = GetOwnerDepartment(departmentID ?? -1, db);

                        var tempscenelist = (from scene in db.Scenes
                                             join project in db.Projects on scene.ProjectID equals project.ProjectID
                                             where project.EnterpriseID == enterpriseID && !project.Deleted && !scene.Deleted
                                             select new
                                             {
                                                 DepartmentIDs = project.Departments,
                                                 SceneID = scene.SceneID
                                             }).ToList();

                        var scenelist =
                            tempscenelist.Where(
                                x => x.DepartmentIDs.Split("|".ToCharArray()).Contains(departmentID.ToString())).Select(n => n.SceneID).ToList();
                        //所有现场数据
                        itemAllList = mgdb.GetAll(x => scenelist.Contains(x.SceneID)).ToList();
                    }
                    else
                    {
                        //所有现场数据
                        itemAllList = mgdb.GetAll(x => x.UserID.Equals(userID)).ToList();
                    }

                    //当前需要整改现场数据
                    var itemRectificationList = itemAllList.Where(x => (x.Examines??new List<Examine>()).Any(n => n.ExamineStatus == ItemStatus.Redo) );
                    var itemtemlist = itemAllList.Where(x => x.Examines != null);
                    //在记录操作状态的Examine集合中，最后一个记录状态不为整改，但是在这个集合中至少有一个为需要整改，则记录该条为整改完成.整改完成的现场数据
                    var itemFinishList =
                        itemtemlist.Where(
                            x => (((x.Examines.OrderByDescending(n => n.Time).First()).ExamineStatus !=
                                 ItemStatus.Redo) && (x.Examines.Any(m => m.ExamineStatus == ItemStatus.Redo))));
                    var pictureCount = itemAllList.Select(x => x.Count).Sum();
                    var stallitem = itemAllList.Where(x => "" == x.Relation);
                    return new RectificationStatisticalDto()
                    {
                        FinishCount = itemFinishList.Count(),
                        RectificationCount = itemRectificationList.Count(),
                        AllCount = stallitem.Count(),
                        PictureCount = pictureCount
                    };
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
