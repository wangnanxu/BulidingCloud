using ML.BC.EnterpriseData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IProjectSceneManagementService : IServiceBase
    {
        //项目搜索
        List<ProjectDto> SearchProjectOnEnterprise(string projectId, string projectName, string managerName, Status status, string enterpriseId, int pageSize, int pageNumber, out int amount);
        List<ProjectDto> SearchProjectOnDepartment(string projectId, string projectName, string managerName, Status status, int? departmentId, string enterpriseId, int pageSize, int pageNumber, out int amount);
        //现场搜索
        List<ScenesDto> SearchSceneOnDepartment(string sceneName, string projectName, int? departmentID, string enterpriseId, int pageSize, int pageNumber, out int amount);
        List<ScenesDto> SearchSceneOnEnterprise(string sceneName, string projectName, string enterpriseId, int pageSize, int pageNumber, out int amount);
        //获取部门下的所有项目
        List<ProjectDto> GetAllProjectsOfDepartment(int? departmentId, string enterpriseId);
        List<ProjectDto> GetAllProjectsOfDepartment(int? departmentId, string enterpriseId, int pageSize, int pageNumber, int amount);//未用
        //获取项目下的所有现场
        List<ScenesDto> GetAllSceneOfProject(string ProjectId, int pageSize, int pageNumber, out int amount);
        //获取某项目的角色及对应人员列表的列表
        List<KeyValuePair<RoleIdName, List<UserIdName>>> GetRoleUserListOfDepartment(string enterpriseId, int? departmentId, string projectId);
        List<KeyValuePair<RoleIdName, List<UserIdName>>> GetRoleUserListOfEnterprise(string enterpriseId, string projectId);

        //获取某现场的的角色及对应人员列表的列表
        List<KeyValuePair<RoleIdName, List<UserIdName>>> GetRoleUserListOfSceneInDepartment(string enterpriseId, int? departmentId, string projectId, string sceneID);

        List<KeyValuePair<RoleIdName, List<UserIdName>>> GetRoleUserListOfSceneInEnterprise(string enterpriseId, string projectId, string sceneID);
        //项目CRUD
        string AddProject(ProjectDto project);
        bool DeleteProject(string projectId);
        bool UpdateProject(ProjectDto project);

        //现场CRUD
        string AddScene(ScenesDto scene);
        bool DeleteScene(string sceneId);
        bool UpdateScene(ScenesDto scene);
        bool SetSceneStatus(string sceneID, ML.BC.EnterpriseData.Common.Status status);

        EnterpriseData.Model.Scene GetOneScene(string id);
        ProjectAndSceneSyncDto GetProjectAndSceneForSync(string enterpriseId);
        ProjectAndSceneSyncDto GetProjectAndSceneForSync(string userID, string deviceID, string enterpriseID);
    }
}
