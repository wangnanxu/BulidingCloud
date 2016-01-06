using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class ProjectAndSceneSyncDto
    {
        public ProjectSyncDto[] Projects { get; set; }
        public SceneSyncDto[] Scenes { get; set; }
    }

    public class ProjectSyncDto
    {
        public string EnterpriseID { get; set; }
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public int ProjectState { get; set; }
        public string ProjectRoles { get; set; }
        public string Departments { get; set; }
        public bool HaveScene { get; set; }
        public string Manager { get; set; }
        public bool Deleted { get; set; }
    }

    public class SceneSyncDto
    {
        public string SceneID { get; set; }
        public string SceneName { get; set; }
        public List<GroupedUser> SceneWorker { get; set; }
        public List<GroupedUser> AllWorkers { get; set; }
        public string Address { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ParentID { get; set; }
        public string ProjectID { get; set; }
        public int SceneState { get; set; }
        public string SceneType { get; set; }
        public bool HasData { get; set; }
        public bool Deleted { get; set; }
    }
}
