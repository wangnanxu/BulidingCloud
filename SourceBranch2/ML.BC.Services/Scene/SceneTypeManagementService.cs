using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model;
using ML.BC.EnterpriseData.Model.Extend;
using ML.BC.Infrastructure.Exceptions;

namespace ML.BC.Services
{
    public class SceneTypeManagementService:ISceneTypeManagementService
    {

        public int AddSceneType(SceneTypeDto sceneType)
        {
            try
            {
                if (string.IsNullOrEmpty(sceneType.EnterpriseID))
                    throw new KnownException("企业ID不允许为空");     
                using (var db = new BCEnterpriseContext())
                {
                    SceneType temp = new SceneType()
                    {
                        Available = sceneType.Available,
                        ID = sceneType.ID,
                        EnterpriseID = sceneType.EnterpriseID,
                        Name = sceneType.Name,
                        ParentID = sceneType.ParentID
                    };
                    db.SceneTypes.Add(temp);
                    if (0 < db.SaveChanges())
                    {
                        return temp.ID;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        private bool DeleteChildrenScene(int sceneTypeId, BCEnterpriseContext db)
        {
            var temp = db.SceneTypes.FirstOrDefault(x => x.ID == sceneTypeId);
            if (null == temp)
                return false;
            
            var scenetype = db.Scenes.Where(x => x.Type.Contains(sceneTypeId.ToString())).FirstOrDefault();
            if (null == scenetype)
                db.SceneTypes.Remove(temp);
            else
                throw new KnownException("该现场类型已经被引用，无法删除");
            var list = db.SceneTypes.Where(x => x.ParentID == sceneTypeId).ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                if (!DeleteChildrenScene(list[i].ID, db))
                    break;
            }
            return 0 < db.SaveChanges();
        }
        public bool DeleteSceneType(int sceneTypeId)
        {
            try
            {
                using(var db = new BCEnterpriseContext())
                {
                    return DeleteChildrenScene(sceneTypeId, db);
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public bool UpdateSceneType(SceneTypeDto sceneType)
        {
            try
            {
                if(string.IsNullOrEmpty(sceneType.EnterpriseID))
                throw new KnownException("企业ID不允许为空");               
                using(var db = new BCEnterpriseContext())
                {
                    var temp = db.SceneTypes.FirstOrDefault(x => x.ID == sceneType.ID);
                    temp.Name = sceneType.Name;
                    temp.EnterpriseID = sceneType.EnterpriseID;
                    temp.ParentID = sceneType.ParentID;
                    temp.Available = sceneType.Available;
                    temp.UpdateTime = DBTimeHelper.DBNowTime(db);
                    return 0 < db.SaveChanges();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public List<SceneTypeDto> GetAllSceneTypeList(string enterpriseId)
        {
            try
            {
                using(var db = new BCEnterpriseContext())
                {
                    return db.SceneTypes.Where(x => x.EnterpriseID.Equals(enterpriseId)).Select(n => new SceneTypeDto { 
                    Available = n.Available,
                    EnterpriseID = n.EnterpriseID,
                    ID = n.ID,
                    Name = n.Name,
                    ParentID = n.ParentID
                    }).ToList();
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
