using ML.BC.EnterpriseData.MongoDb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace ML.BC.EnterpriseData.Model
{
    public enum ItemStatus//说说状态  
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 通过审核
        /// </summary>
        Pass = 1,

        /// <summary>
        /// 整改
        /// </summary>
        Redo = 2,

        /// <summary>
        /// 归档
        /// </summary>
        Final = 3,

        /// <summary>
        /// 全部
        /// </summary>
        All = 4,
    }

    public enum SceneItemType//临检类型
    {
        /// <summary>
        /// 签到
        /// </summary>
        Checkin = 1,

        /// <summary>
        /// 过程照
        /// </summary>
        Progress = 2,

        /// <summary>
        /// 安全照
        /// </summary>
        Safe = 4,

        /// <summary>
        /// 临检照
        /// </summary>
        Visiting = 8,

        /// <summary>
        /// 交底照
        /// </summary>
        Deliver = 16,

        /// <summary>
        /// 签退
        /// </summary>`
        Checkout = 32,

        //  /// <summary>
        //  /// 完工
        //  /// </summary>`
        //  Finish = 64,

    }

    public class Comment
    {
        public Guid CommentGuid { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }
        [BsonIgnore]
        public string Stime { get; set; }
        public string Content { get; set; }
    }
    public class SceneItem : MongoDBEntity
    {
        public string SceneID { get; set; }
        public Guid PictureGuid { get; set; }
        public int Count { get; set; }
        public ItemStatus Status { get; set; }//说说状态
        public string UserID { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }
        [MongoDB.Bson.Serialization.Attributes.BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime UpdateTime { get; set; }
        public string Address { get; set; }
        public string GPS { get; set; }
        public string Description { get; set; }//描述
        public List<ImageUrl> Images { get; set; }//图片url
        public long TotalThuImageBytes { get; set; }
        public long TotalOrgImageBytes { get; set; }
        public List<Comment> Comments { get; set; }//评论
        public SceneItemType Type { get; set; }//说说类型
    }

    public class Picture
    {
        public Guid PictureGuid { get; set; }
        public Stream PictureStream { get; set; }
        public string PictureName { get; set; }
    }

    public class ImageUrl
    {
        public string ThumbnailPicture; //  缩略图url
        public string OriginalPicture;  //  原图url
    }

    public class ImageInfo
    {
        public string ThumbnailPicture; //  缩略图url
        public string OriginalPicture;  //  原图url
        public long ThuImageBytes;  //  缩略图片大小
        public long OrgImageBytes;  //  原始图片大小
    }
}
