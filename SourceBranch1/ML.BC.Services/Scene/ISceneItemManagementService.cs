using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model;
using ML.BC.Services;

namespace ML.BC.Services
{
    public interface ISceneItemManagementService : IServiceBase
    {
        /// <summary>
        /// 删除现场数据
        /// </summary>
        /// <param name="sceneItemID"></param>
        /// <param name="updateTime">暂时还未用到，只是为了支持接收这个参数，以后可能会用到，现在暂未做任何处理</param>
        /// <returns></returns>
        bool DeleteSceneItem(string sceneItemID, DateTime updateTime);
        bool DeleteSceneItem(string sceneItemID);
        bool DeleteCommentItem(string sceneItemID, Guid commentGuid, DateTime updateTime);
        bool SavePicture(Picture picture);
        bool SetSceneItemStatus(string sceneItemID, ItemStatus status, DateTime updateTime);
        bool SyncSceneItemStatusAndComments(string sceneItemID, ItemStatus status, Comment[] comments);
        SceneItem Add(SceneItem sceneItem);
        SceneItem AddCommentItem(string sceneItemID, Comment comment, ItemStatus status);
        List<SceneItemDto> GetAllByStatus(string sceneID, ItemStatus status, int pageSize, int pageIndex, out int count);
        List<SceneItemDto> GetAllByStatus(string userID,string deviceID,string sceneID, ItemStatus status, int pageSize, DateTime time, out int count);
        List<SceneItemDto> GetSceneItemForSync(string userId, string device);
    }
}
