using ML.BC.EnterpriseData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services.Common
{
    public interface IAppSyncService
    {
        bool AddUserMessageQueueItem(UserMessageQueueItem userMessaeQueueItem);
        bool ConfirmSyncSuccess(string userId, string device, AppSyncActionEnum action, DateTime syncTime);
        List<UserMessageQueueItem> PopupUserMessageQueueItem(string userId, string device);
        bool ClearCache(string userID, string deviceID);
        bool ConfirmSentSuccess(List<string> userMessaeQueueItemIDs);
        FrontUserDto GetFrontUserDtoByID(string frontUserID);
        List<SyncStateDto> GetExistSyncStateDtosBySceneItemID(string sceneItemID);
        List<SyncStateDto> GetExistSyncStateDtosBySceneID(string sceneID);
        List<SyncStateDto> GetExistSyncStateDtosByEnterpriseID(string EnterpriseID);
        List<SyncStateDto> GetExistSyncStateDtosRelateFrontUserIDAtEnterprise(string userID);
        List<SyncStateDto> GetExistSyncStateDtosRelateDepartmentIDAtEnterprise(string DepartmentID);
        List<SyncStateDto> GetExistSyncStateDtoByUserIDs(string userIDs);
        List<SyncStateDto> GetExistSyncStateDtoByDepartMentIDs(List<int> departMentIDs);
        List<SyncStateDto> GetExistSyncStateDtoByProjectID(string projectID);
        List<SyncStateDto> GetExistSyncStateDtoByRoleID(int RoleID);
        bool InitUserSyncState(string enterpiseId, string userId, string device);
    }
}
