using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ML.BC.Services;

namespace ML.BC.EnterpriseWeb.Areas.APIs.Models
{
    public class SceneModel : ModelBase
    {
        public string SceneID { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string SceneName { get; set; }
        [Required]
        public string EnterpriseID { get; set; }
        [Required]
        public string ProjectID { get; set; }
        [Required]
        public string ParentID { get; set; }
        //[Required]
        public string SceneWorker { get; set; }//workerIDS 
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public ML.BC.EnterpriseData.Common.Status SceneState { get; set; }
        //[Required]
        public string SceneType { get; set; }

        public static implicit operator ScenesDto(SceneModel model)
        {
            return new ScenesDto()
            {
                SceneID = model.SceneID,
                Address = model.Address,
                BeginDate = model.BeginDate,
                EndDate = model.EndDate,
                EnterpriseID = model.EnterpriseID,
                Name = model.SceneName,
                ParentSceneID = model.ParentID,
                Status = model.SceneState,
                ProjectID = model.ProjectID,
                Wokers = SerializerWorkers(model.SceneWorker)
            };
        }

        private static List<GroupedUser> SerializerWorkers(string workers)
        { 
            try
            {
                return string.IsNullOrEmpty(workers) ? new List<GroupedUser>() : ML.BC.Infrastructure.Serializer.FromJson<List<GroupedUser>>(workers);
            }
            catch{}
            return new List<GroupedUser>();            
        }

    }
}