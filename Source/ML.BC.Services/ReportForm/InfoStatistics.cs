using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model;
using ML.BC.EnterpriseData.MongoDb;

namespace ML.BC.Services
{
    public class InfoStatistics : IInfoStatistics
    {

        public InfoStatisticsDto GetBackInfoStatistics()
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var mgdb = new MongoDbProvider<SceneItem>();
                    var users = db.FrontUsers.Count();
                    var projects = db.Projects.Where(x => !x.Deleted).Count();
                    var scenes = db.Scenes.Where(x => !x.Deleted).Count();
                    var pictrues = mgdb.GetAll(x => true).Select(n => n.Count).Sum();
                    var sceneItemStatus = mgdb.GetAll(x => x.Status == ItemStatus.Redo && !x.IsExamine && !x.IsExamine).Count();
                    return new InfoStatisticsDto()
                    {
                        Users = users,
                        Scenes = scenes,
                        Projects = projects,
                        Pictures = pictrues,
                        ItemStatus = sceneItemStatus
                    };
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public InfoStatisticsDto GetEnterpriseInfoStatistics(string enterpriseId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var mgdb = new MongoDbProvider<SceneItem>();
                    var users = db.FrontUsers.Where(x => x.EnterpiseID.Equals(enterpriseId)).Count();
                    var projects = db.Projects.Where(x => !x.Deleted && x.EnterpriseID.Equals(enterpriseId)).Count();
                    var sceneIDlist = db.Scenes.Where(x => !x.Deleted && x.EnterpriseID.Equals(enterpriseId)).Select(n => n.SceneID).ToList();
                    var pictrues = mgdb.GetAll(x => sceneIDlist.Contains(x.SceneID)).Select(n => n.Count).Sum();
                    var sceneItemStatus = mgdb.GetAll(x => x.Status == ItemStatus.Redo && sceneIDlist.Contains(x.SceneID) && !x.IsExamine).Count();
                    return new InfoStatisticsDto()
                    {
                        Users = users,
                        Scenes = sceneIDlist.Count(),
                        Projects = projects,
                        Pictures = pictrues,
                        ItemStatus = sceneItemStatus
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
