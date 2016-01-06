using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ML.BC.EnterpriseWeb.Areas.APIs.Models
{
    /// <summary>
    /// 现场数据
    /// </summary>
    public class AddSceneDataModel : ModelBase
    {
        [Required]
        public Guid Guid { get; set; }
        [Required]
        public string SceneID { get; set; }
        [Required]
        public string Address { get; set; }//经度|韦度
        [Required]
        public DateTime Time { get; set; }
        public string Content { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public int Type { get; set; }
    }

    public class UploadImageModel : ModelBase
    {
        [Required]
        public Guid Guid { get; set; }
        [Required]
        public string FileName { get; set; }
    }
    /// <summary>
    /// 获取现场数据列表
    /// </summary>
    public class GetSceneDataModel : ModelBase
    {
        [Required]
        public string SceneID { get; set; }
        /// <summary>
        /// 状态 默认4 查询全部
        /// </summary>
        [DefaultValue(4)]
        public int Status { get; set; }

        [DefaultValue(10)]
        public int PageSize { get; set; }

        /// <summary>
        /// 表示app端本地数据库中最小的时间。
        /// </summary>
        public DateTime Time { get; set; }
    }
    /// <summary>
    /// 评论
    /// </summary>
    public class CommentDataModel : ModelBase
    {
        [Required]
        public Guid Guid { get; set; }
        /// <summary>
        /// 现场数据id
        /// </summary>
        [Required]
        public string MessageID { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public string Content { get; set; }
    }
    /// <summary>
    /// 更新现场数据状态
    /// </summary>
    public class UpdateStatusModel : ModelBase
    {
        [Required]
        public string MessageID { get; set; }
      
        [Required]
        public DateTime Time { get; set; }

        [Required]
        public int Status { get; set; }
    }
    /// <summary>
    /// 删除现场数据model
    /// </summary>
    public class DeleteSceneModel : ModelBase
    {
       [Required]
        public string MessageID { get; set; }
       [Required]
        public DateTime Time { get; set; }
    }

    /// <summary>
    /// 删除现场数据的评论model
    /// </summary>
    public class DeleteCommentDataModel : ModelBase
    {
        [Required]
        public Guid CommentGuid { get; set; }
        /// <summary>
        /// 评论id
        /// </summary>
        [Required]
        public string MessageID { get; set; }

       [Required]
        public DateTime Time { get; set; }

    }

    /// <summary>
    /// APP 离线现场数据同步到服务器
    /// </summary>
    public class SyncSceneDataToServerModel : ModelBase
    {
        [Required]
        public string MessageID { get; set; }
        [Required]
        public int Status { get; set; }
        public string Comments { get; set; }
    }
}