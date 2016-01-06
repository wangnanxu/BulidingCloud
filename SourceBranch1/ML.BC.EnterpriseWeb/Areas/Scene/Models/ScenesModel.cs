using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ML.BC.EnterpriseData.Model;
using ML.BC.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;
using ML.BC.EnterpriseData.Common;

namespace ML.BC.EnterpriseWeb.Areas.Scene.Models
{
    public class ScenesViewModel
    {
        public int id { get; set; }
        public int _parentId { get; set; }
        public string SceneID { get; set; }
        public string Name { get; set; }
        public string LatitudeAndLongitude{get;set;}
        public string Address { get; set; }
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }//new
        public string ParentSceneID { get; set; }
        public List<KeyValuePair<RoleIdName, List<UserIdName>>> Workers { get; set; }//分组的人员
        public string RegistDate { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public int Status { get; set; }
        public bool Deleted { get; set; }

        public bool HasData { get; set; }

        static public implicit operator ScenesViewModel(ScenesDto dto)
        {
            if (dto.Wokers == null) { dto.Wokers = new List<Services.GroupedUser>(); }
            return new ScenesViewModel
            {
                SceneID = dto.SceneID,
                Name = dto.Name,
                Address = dto.Address,
                LatitudeAndLongitude = dto.LatitudeAndLongitude,
                ProjectID = dto.ProjectID,
                ProjectName = dto.ProjectName,
                ParentSceneID = dto.ParentSceneID,
                //Workers = processString(dto.Wokers),
                RegistDate = dto.RegistDate.ToString("yyyy-MM-dd"),
                BeginDate = dto.BeginDate==null?"":dto.BeginDate.Value.ToString("yyyy-MM-dd"),
                EndDate = dto.EndDate == null ? "" : dto.EndDate.Value.ToString("yyyy-MM-dd"),
                Status = (int)dto.Status,
                Deleted = dto.Deleted,
                HasData = dto.HasData
                //workernames
            };
        }

        static public List<string> processString(string org)
        {
            var list = new List<string>();
            list = org.Split('|').ToList();
            return list;
        }
        static public string processList(List<string> org)
        {
            StringBuilder s = new StringBuilder();
            foreach (var str in org)
            {
                s.Append(str + "|");
            }
            return s.ToString();
        }
    }

    public class SceneSearchModel
    {
        [DefaultValue(1)]
        public int page { get; set; }

        [DefaultValue(15)]
        public int rows { get; set; }
        public string SceneName { get; set; }//搜索
        public string ProjectName { get; set; }//搜索 
        public string ProjectID { get; set; }//搜索 项目id

        [DefaultValue(4)]
        public Status Status { get; set; }
        public string from { get; set; }//来源标识  地图 或者 清单
    }



    public class ScenesNewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string ProjectID { get; set; }


        [Required]
        public string Address { get; set; }


        public string ParentSceneID { get; set; }
        //public List<string> Workers { get; set; }//workerID    
        //List<GroupedUser> RoleWorkers { get; set; }
        public List<FrontEndGroupedUser> RoleWorkers { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        [DefaultValue(0)]
        public int Status { get; set; }

        static public List<string> processString(string org)
        {
            var list = new List<string>();
            list = org.Split('|').ToList();
            return list;
        }
        static public string processList(List<string> org)
        {
            StringBuilder s = new StringBuilder();
            foreach (var str in org)
            {
                s.Append(str + "|");
            }
            return s.ToString();
        }
        static public implicit operator ScenesDto(ScenesNewModel model)
        {
            //StringBuilder str = new StringBuilder();
            //foreach (var groupUser in model.RoleWorkers)
            //{
            //    foreach (var user in groupUser.worker)
            //    {
            //        if(user!=null) str.Append(user+"|");
            //    }
            //}
            return new ScenesDto
            {
                Address = model.Address,
                ParentSceneID = model.ParentSceneID,
                Status = (ML.BC.EnterpriseData.Common.Status)model.Status,
                ProjectID = model.ProjectID,
                //Wokers = str.ToString(),
                Name = model.Name,
                BeginDate = model.BeginDate,
                EndDate = model.EndDate
                //enterpriseID  经纬度手动处理
            };
        }

    }



    public class SceneUpdateModdel
    {
        [Required]
        public string SceneID { get; set; }

        [Required]
        public string Name { get; set; }


        //public List<string> Workers { get; set; }//workerID   

        //List<GroupedUser> RoleWorkers { get; set; }
        public List<FrontEndGroupedUser> RoleWorkers { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public int Status { get; set; }

        static public implicit operator ScenesDto(SceneUpdateModdel model)
        {
            //StringBuilder str = new StringBuilder();
            //foreach (var groupUser in model.RoleWorkers)
            //{
            //    foreach (var user in groupUser.worker)
            //    {
            //        str.Append(user + "|");
            //    }
            //}
            return new ScenesDto
            {
                Address = model.Address,
                SceneID = model.SceneID,
                Status = (ML.BC.EnterpriseData.Common.Status)model.Status,
                //Wokers = str.ToString(),
                Name = model.Name,
                BeginDate = model.BeginDate,
                EndDate = model.EndDate
            };
        }
        static public List<string> processString(string org)
        {
            var list = new List<string>();
            org = org.Replace("||", "|");
            list = org.Split('|').ToList();
            return list;
        }
        static public string processList(List<string> org)
        {
            StringBuilder s = new StringBuilder();
            foreach (var str in org)
            {
                if (str.Equals("")) continue;
                s.Append(str + "|");
            }
            return s.ToString();
        }

    }


    public class SceneResultModel
    {
        public int total;
        public List<ScenesViewModel> rows;
    }

    public class SceneTreeHelper
    {
        private Dictionary<string, int> map = new Dictionary<string, int>();//储存字符串id到数字id的映射

        //将字符串形式id转换为数字id并储存到字典供easyui使用
        //easyui仅支持数字id

        public int processSID(string sid)
        {


            if (sid.Equals("-1") || sid.Equals("")) return 0;

            if (map.ContainsKey(sid))
            {
                return map[sid];
            }
            else
            {
                int idx = map.Count + 1;
                map.Add(sid, idx);
                return idx;
            }
        }
    }


    public class SceneItemViewModel
    {
        public string sceneItemID;
        public string sceneID;
        public ItemStatus status;//说说状态
        public string userID;
        public string userName;
        public string time;//发表时间
        public string address;
        public string description;//描述
        public List<ImageUrl> images;//图片url
        public List<Comment> comments;//评论
        public SceneItemType type;//说说类型
        public string userAvartarURL;//用户头像

        static public implicit operator SceneItemViewModel(SceneItemDto item)
        {
            return new SceneItemViewModel
            {
                sceneID = item.SceneID,
                sceneItemID = item.Id,
                status = item.Status,
                userID = item.UserID,
                time = item.CreateTime.ToString("yyyy-MM-dd HH:mm"),
                address = item.Address,
                description = item.Description,
                images = item.Images??new List<ImageUrl>(),
                comments = item.Comments??new List<Comment>(),
                type = item.Type,
                userName = item.UserName,
                userAvartarURL = item.UserPicture

            };
        }

        private SceneItemViewModel() { }//不能直接构造 必须用基类构造
    }

    public class SceneItemSearchModel
    {
        [DefaultValue(1)]
        public int page { get; set; }
        [DefaultValue(10)]
        public int rows { get; set; }

        [Required]
        public string SceneID { get; set; }
    }
    public class SceneItemResultModel
    {
        public int total;
        public List<SceneItemViewModel> rows;
    }


    public class SceneItemNewModel
    {
        [Required]
        public string SceneID { get; set; }

        public string content { get; set; }
        
        //public List<string> file { get; set; }

        [Required]
        public SceneItemType SceneItemType { get; set; }//说说类型
    }

    
    public class FrontEndGroupedUser
    {
        public int role { get; set; }
        public List<string> worker { get; set; }

         static public implicit operator GroupedUser(FrontEndGroupedUser user){
            GroupedUser gu = new GroupedUser(user.role, user.worker);
            return gu;
        }
    }
}