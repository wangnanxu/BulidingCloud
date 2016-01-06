using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ML.BC.EnterpriseData.Model;
using ML.BC.EnterpriseData.Model.Extend;
using ML.BC.EnterpriseData.MongoDb;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.MsmqHelper;
using MongoDB.Driver.Linq;
using Image = System.Drawing.Image;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Utilities;

namespace ML.BC.Services
{
    public class SceneItemManagementService : ISceneItemManagementService
    {
        private const string PictureDbName = "Picture";

        public bool SetSceneItemStatus(string sceneItemID, ItemStatus status, DateTime updateTime)
        {
            try
            {
                if (string.IsNullOrEmpty(sceneItemID)) throw new KnownException("id为空，无法设置！");
                var db = new MongoDbProvider<SceneItem>();
                var item = db.GetById(sceneItemID);
                item.UpdateTime = DBTimeHelper.DBNowTime();
                item.Status = status;
                db.Update(item);
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public bool SyncSceneItemStatusAndComments(string sceneItemID, ItemStatus status, Comment[] comments)
        {
            try
            {
                if (string.IsNullOrEmpty(sceneItemID)) throw new ArgumentNullException("sceneItemID");
                var db = new MongoDbProvider<SceneItem>();
                var item = db.GetById(sceneItemID);
                if (item == null) throw new KnownException("此现场数据不存在.");
                item.Comments = comments.ToList();
                item.Status = status;
                item.UpdateTime = ML.BC.EnterpriseData.Model.Extend.DBTimeHelper.DBNowTime();
                db.Update(item);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //  时间参数暂未用到，以后可能在记录操作时间需要用到。
        public bool DeleteSceneItem(string sceneItemID, DateTime updateTime)
        {
            return DeleteSceneItem(sceneItemID);
        }

        public bool DeleteSceneItem(string sceneItemID)
        {
            try
            {
                if (string.IsNullOrEmpty(sceneItemID)) throw new KnownException("id为空，无法删除！");
                var mgdb = new MongoDbProvider<SceneItem>();
                var sceneItem = mgdb.GetAll(o => o.Id == sceneItemID).FirstOrDefault();
                if (null == sceneItem)
                {
                    return true;
                }
                else if (sceneItem.Status == ItemStatus.Final)
                {
                    throw new KnownException("已经归档，不能删除！");
                }

                //  如果有图片，需要删除图片
                if (null != sceneItem.Images)
                {
                    foreach (var image in sceneItem.Images)
                    {
                        mgdb.DeleteFileByName(image.OriginalPicture, PictureDbName);
                        mgdb.DeleteFileByName(image.ThumbnailPicture, PictureDbName);
                    }
                }
                mgdb.Delete(sceneItemID);

                using (var db = new BCEnterpriseContext())
                {
                    var scene = db.Scenes.First(o => o.SceneID == sceneItem.SceneID);
                    if (scene.HasData && (null == mgdb.GetByCondition(o => o.SceneID == sceneItem.SceneID))) scene.HasData = false;
                    db.SaveChanges();
                }
                return Send2MsmqAsDelete(sceneItemID, OperationEnum.Deleted, sceneItem.SceneID);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool DeleteCommentItem(string sceneItemID, Guid commentGuid, DateTime updateTime)
        {
            var mgdb = new MongoDbProvider<SceneItem>();
            var sceneItem = mgdb.GetById(sceneItemID);
            if (sceneItem.Status == ItemStatus.Final) throw new KnownException("已经归档，无法删除");
            var del = sceneItem.Comments.FirstOrDefault(o => o.CommentGuid == commentGuid);
            if (null == del) throw new KnownException("评论不存在，无法删除");
            if (sceneItem.Comments.Remove(del))
                sceneItem.UpdateTime = DBTimeHelper.DBNowTime();
            mgdb.Update(sceneItem);

            //  删除评论分发消息
            Send2MsmqAsDelete(commentGuid.ToString(), OperationEnum.Deleted, sceneItem.SceneID);
            return true;
        }

        public SceneItem AddCommentItem(string sceneItemID, Comment comment, ItemStatus status)
        {
            try
            {
                if (string.IsNullOrEmpty(sceneItemID) || null == comment) throw new KnownException("id或信息对象为空，无法添加！");
                var db = new MongoDbProvider<SceneItem>();
                var item = db.GetById(sceneItemID);
                if (item == null) throw new KnownException("此数据在服务器上已被删除，请手动删除此条数据！");
                if (ItemStatus.Final == item.Status) throw new KnownException("已经归档，无法评论！");
                if (item.Comments == null) item.Comments = new List<Comment>();
                comment.Time = DBTimeHelper.DBNowTime();
                item.Comments.Add(comment);
                item.Status = status;
                item.UpdateTime = DBTimeHelper.DBNowTime();
                var re = db.Update(item);
                re.Images = MakeUrlWithPictureName(re.Images);
                return re;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public List<SceneItemDto> GetAllByStatus(string sceneID, ItemStatus status, int pageSize, int pageIndex,
            out int count)
        {
            try
            {
                if (string.IsNullOrEmpty(sceneID)) throw new ArgumentNullException("sceneID");
                var mgdb = new MongoDbProvider<SceneItem>();

                var query = ItemStatus.All == status
                    ? mgdb.GetAll(o => (o.SceneID == sceneID))
                    : mgdb.GetAll(o => (o.SceneID == sceneID) && (o.Status == status));

                count = query.Count();
                int pageTotal;

                if (pageSize > 0)
                {
                    pageTotal = (count + pageSize - 1) / pageSize;
                }
                else
                {
                    pageSize = 10;
                    pageTotal = (count + pageSize - 1) / pageSize;
                }

                if (pageIndex > pageTotal)
                    pageIndex = pageTotal;
                if (pageIndex < 1)
                    pageIndex = 1;

                var sceneItems = query
                    .OrderByDescending(obj => obj.CreateTime)
                    .Skip(pageSize * (pageIndex - 1))
                    .Take(pageSize)
                    .ToList();

                using (var db = new BCEnterpriseContext())
                {
                    if (0 == sceneItems.Count) return new List<SceneItemDto>();
                    var userIds = sceneItems.Select(o => o.UserID).ToList();
                    var reUserInfo = db.FrontUsers.Where(obj => userIds.Contains(obj.UserID)).Select(o => new
                    {
                        uid = o.UserID,
                        name = o.Name,
                        picture = o.Picture
                    }).ToList();

                    var re = (from item in sceneItems
                              join r in reUserInfo on item.UserID equals r.uid into tempSU
                              from t in tempSU.DefaultIfEmpty()
                              select new SceneItemDto
                              {
                                  Id = item.Id,
                                  SceneID = item.SceneID,
                                  PictureGuid = item.PictureGuid,
                                  Count = item.Count,
                                  Status = item.Status,
                                  UserID = item.UserID,
                                  CreateTime = item.CreateTime,
                                  UpdateTime = item.UpdateTime,
                                  Address = item.Address,
                                  GPS = item.GPS,
                                  Description = item.Description,
                                  Images = item.Images,
                                  Comments = item.Comments,
                                  Type = item.Type,
                                  UserName = t == null ? string.Empty : t.name,
                                  UserPicture = t == null ? string.Empty : UriExtensions.GetFullUrl(t.picture)
                              })
                        .ToList();
                    foreach (var r in re)
                    {
                        r.Images = MakeUrlWithPictureName(r.Images);
                    }
                    return re;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<SceneItemDto> GetAllByStatus(string userID, string deviceID, string sceneID, ItemStatus status,
            int pageSize, DateTime time, out int count)
        {
            if (string.IsNullOrEmpty(sceneID)) throw new ArgumentNullException("sceneID");

            using (var db = new BCEnterpriseContext())
            {
                var iSsync =
                    db.SyncStates.Any(
                        n =>
                            n.UserID == userID && n.DeviceID == deviceID &&
                            n.ActionType == (int)ML.BC.Infrastructure.MsmqHelper.TypeEnum.SceneData);
                if (!iSsync)
                {
                    var syncSceneData = new SyncState()
                    {
                        UserID = userID,
                        DeviceID = deviceID,
                        ActionType = (int)TypeEnum.SceneData,
                        SyncTime = DateTime.Now
                    };

                    db.SyncStates.Add(syncSceneData);
                    if (!(0 < db.SaveChanges()))
                    {
                        var logger = log4net.LogManager.GetLogger(typeof(SceneItemManagementService));
                        logger.Error("第一次写入同步现场数据的状态失败！");
                    }
                }
            }

            var mgdb = new MongoDbProvider<SceneItem>();
            var query = ItemStatus.All == status
                ? mgdb.GetAll(o => (o.SceneID == sceneID) && (o.UpdateTime < time))
                : mgdb.GetAll(o => (o.SceneID == sceneID) && (o.Status == status) && (o.UpdateTime < time));
            count = query.Count();
            if (pageSize < 1) pageSize = 10;
            var sceneItems = query
                .OrderByDescending(obj => obj.CreateTime)
                .Take(pageSize)
                .ToList();

            using (var db = new BCEnterpriseContext())
            {
                if (0 == sceneItems.Count) return new List<SceneItemDto>();
                var userIds = sceneItems.Select(o => o.UserID).ToList();
                var reUserInfo = db.FrontUsers.Where(obj => userIds.Contains(obj.UserID)).Select(o => new
                {
                    uid = o.UserID,
                    name = o.Name,
                    picture = o.Picture
                }).ToList();

                var re = (from item in sceneItems
                          join r in reUserInfo on item.UserID equals r.uid into tempSU
                          from t in tempSU.DefaultIfEmpty()
                          select new SceneItemDto
                          {
                              Id = item.Id,
                              SceneID = item.SceneID,
                              PictureGuid = item.PictureGuid,
                              Count = item.Count,
                              Status = item.Status,
                              UserID = item.UserID,
                              CreateTime = item.CreateTime,
                              UpdateTime = item.UpdateTime,
                              Address = item.Address,
                              GPS = item.GPS,
                              Description = item.Description,
                              Images = item.Images,
                              Comments = item.Comments,
                              Type = item.Type,
                              UserName = t == null ? string.Empty : t.name,
                              UserPicture = t == null ? string.Empty : UriExtensions.GetFullUrl(t.picture)
                          })
                    .ToList();
                foreach (var r in re)
                {
                    r.Images = MakeUrlWithPictureName(r.Images);
                }
                return re;
            }
        }

        public List<string> GetUserIDBySceneID(string sceneID)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var works = db.Scenes.Where(obj => obj.SceneID == sceneID).Select(o => o.Woker).FirstOrDefault();
                    return null == works ? new List<string>() : works.Split('|').ToList();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public SceneItem Add(SceneItem sceneItem)
        {
            try
            {
                if (null == sceneItem) throw new KnownException("现场数据为空，不能添加！");
                if (string.IsNullOrEmpty(sceneItem.SceneID) && string.IsNullOrEmpty(sceneItem.UserID) &&
                    string.IsNullOrEmpty(sceneItem.GPS) && (string.IsNullOrEmpty(sceneItem.Address)))
                    throw new KnownException("必要的现场数据缺失，无法保存！");
                using (var db = new BCEnterpriseContext())
                {
                    if (db.Scenes.Any(o => o.ParentSceneID == sceneItem.SceneID
                        && o.Status == (byte)ML.BC.EnterpriseData.Common.Status.End))
                        throw new KnownException("当前现场不是叶子现场,或者该现场已完工，不能添加现场数据！");

                }
                sceneItem.UpdateTime = DBTimeHelper.DBNowTime();

                var mgdb = new MongoDbProvider<SceneItem>();
                if (sceneItem.Count == 0)
                {
                    var si = mgdb.Insert(sceneItem);
                    return SetSceneHasData(si.SceneID) ? si : null;
                }

                var sessionDatas = HttpContext.Current.Session[sceneItem.PictureGuid.ToString()] as List<ImageInfo>;
                sessionDatas = sessionDatas ?? new List<ImageInfo>();

                var listUrl = new List<ImageUrl>();
                if (sceneItem.Count != sessionDatas.Count) throw new KnownException("图片还未上传完成！");
                foreach (var info in sessionDatas)
                {
                    sceneItem.TotalOrgImageBytes += info.OrgImageBytes;
                    sceneItem.TotalThuImageBytes += info.ThuImageBytes;
                    listUrl.Add(new ImageUrl() { OriginalPicture = info.OriginalPicture, ThumbnailPicture = info.ThumbnailPicture });
                }
                sceneItem.Images = listUrl;

                var re = mgdb.Insert(sceneItem);
                if (null == re) throw new KnownException("保存失败！");
                if (!SetSceneHasData(re.SceneID)) return null;
                HttpContext.Current.Session[sceneItem.PictureGuid.ToString()] = null;
                re.Images = MakeUrlWithPictureName(re.Images);
                return re;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static bool SetSceneHasData(IMongoDbProvider<SceneItem, string> mgdb, string sceneID)
        {
            try
            {
                if (null != mgdb.GetAll(o => o.SceneID == sceneID)) return true;
                using (var db = new BCEnterpriseContext())
                {
                    var scene = db.Scenes.First(o => o.SceneID == sceneID);
                    if (!scene.HasData) return true;
                    scene.HasData = false;
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private static bool SetSceneHasData(string sceneID)
        {
            try
            {
                if (string.IsNullOrEmpty(sceneID)) return false;
                using (var db = new BCEnterpriseContext())
                {
                    var scene = db.Scenes.First(o => o.SceneID == sceneID);
                    if (!scene.HasData)
                    {
                        scene.HasData = true;
                        return 0 < db.SaveChanges();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool SavePicture(Picture picture)
        {
            try
            {
                if (null == picture || string.IsNullOrEmpty(PictureDbName)) throw new KnownException("图片信息不存在！");
                var orgPicName = "org" + picture.PictureName;
                var thuPicName = "thu" + picture.PictureName;
                long thuImageBytes = 0;
                long orgImageBytes = 0;

                var db = new MongoDbProvider<SceneItem>();
                db.SaveFileByStream(picture.PictureStream, orgPicName, PictureDbName);
                orgImageBytes = picture.PictureStream.Length;

                //  缩略图
                var miniStream = ImageHelper.MakeThumbnailImage(picture.PictureStream);
                db.SaveFileByStream(miniStream, thuPicName, PictureDbName);
                thuImageBytes = miniStream.Length;

                var sessionUrlData = HttpContext.Current.Session[picture.PictureGuid.ToString()] as List<ImageInfo>;
                if (sessionUrlData == null)
                {
                    sessionUrlData = new List<ImageInfo> { new ImageInfo()
                    {
                        OriginalPicture = orgPicName, 
                        ThumbnailPicture = thuPicName,
                        OrgImageBytes = orgImageBytes,
                        ThuImageBytes = thuImageBytes
                    } };
                    HttpContext.Current.Session[picture.PictureGuid.ToString()] = sessionUrlData;
                }
                else
                {
                    sessionUrlData.Add(new ImageInfo()
                    {
                        OriginalPicture = orgPicName,
                        ThumbnailPicture = thuPicName,
                        OrgImageBytes = orgImageBytes,
                        ThuImageBytes = thuImageBytes
                    });
                }
                picture.PictureStream.Dispose();
                return true;
            }
            catch (Exception ex)
            {

                throw new KnownException(ex.Message);
            }

        }

        private const string GETSCENEITEMFORSYNCLOCK = "GETSCENEITEMFORSYNCLOCK";

        public List<SceneItemDto> GetSceneItemForSync(string userId, string device)
        {
            lock (GETSCENEITEMFORSYNCLOCK)
            {
                using (var db = new BCEnterpriseContext())
                {
                    List<string> sceneIDs = new List<string>();

                    #region get sceneIDs

                    var userDepartmentID = db.FrontUsers.Where(o => o.UserID == userId).Select(o => o.DepartmentID).FirstOrDefault().ToString();
                    if (!string.IsNullOrEmpty(userDepartmentID))
                    {
                        sceneIDs = (from scene in db.Scenes
                                    join project in db.Projects on scene.ProjectID equals project.ProjectID into tPrj
                                    from prj in tPrj.DefaultIfEmpty()
                                    where prj.Departments.Contains(userDepartmentID)
                                    select scene.SceneID
                                    ).Distinct()
                                     .ToList();
                    }

                    if (!(db.SyncStates.Any(n => n.UserID == userId && n.DeviceID == device && n.ActionType == (int)ML.BC.Infrastructure.MsmqHelper.TypeEnum.SceneData)))
                    {
                        var syncSceneData = new SyncState()
                        {
                            UserID = userId,
                            DeviceID = device,
                            ActionType = (int)TypeEnum.SceneData,
                            SyncTime = DateTime.Now
                        };
                        db.SyncStates.Add(syncSceneData);
                        if (0 < db.SaveChanges())
                        {
                            return new List<SceneItemDto>();
                        }
                        else
                        {
                            var logger = log4net.LogManager.GetLogger(typeof(SceneItemManagementService));
                            logger.Error("第一次写入同步现场数据的状态失败！");
                            return new List<SceneItemDto>();
                        }
                    }

                    var lastSyncTime = (db.SyncStates.FirstOrDefault(
                        n =>
                            n.UserID == userId && n.DeviceID == device &&
                            n.ActionType == (int)ML.BC.Infrastructure.MsmqHelper.TypeEnum.SceneData)
                                             ?? new SyncState { SyncTime = DateTime.MinValue }).SyncTime;

                    #endregion

                    var mgdb = new MongoDbProvider<SceneItem>();
                    var sceneItems = mgdb.GetAll(n => (sceneIDs.Contains(n.SceneID)) && (n.UpdateTime >= lastSyncTime));

                    var userIds = sceneItems.Select(n => n.UserID).ToList();
                    var relateUsers = db.FrontUsers.Where(obj => userIds.Contains(obj.UserID)).Select(o => new
                    {
                        uid = o.UserID,
                        name = o.Name,
                        picture = o.Picture
                    }).ToList();

                    var resultList = (from item in sceneItems
                                      join r in relateUsers on item.UserID equals r.uid into tempSU
                                      from t in tempSU.DefaultIfEmpty()
                                      select new SceneItemDto
                                      {
                                          Id = item.Id,
                                          SceneID = item.SceneID,
                                          PictureGuid = item.PictureGuid,
                                          Count = item.Count,
                                          Status = item.Status,
                                          UserID = item.UserID,
                                          CreateTime = item.CreateTime,
                                          UpdateTime = item.UpdateTime,
                                          Address = item.Address,
                                          GPS = item.GPS,
                                          Description = item.Description,
                                          Images = item.Images,
                                          Comments = item.Comments,
                                          Type = item.Type,
                                          UserName = t == null ? string.Empty : t.name,
                                          UserPicture = t == null ? string.Empty : UriExtensions.GetFullUrl(t.picture)
                                      })
                        .OrderByDescending(o => o.UpdateTime)
                        .ToList();
                    foreach (var r in resultList)
                    {
                        r.Images = MakeUrlWithPictureName(r.Images);
                    }
                    return resultList;
                }
            }
        }

        //  暂用方法，需要优化
        private static bool Send2MsmqAsDelete(string id, OperationEnum operation, string sceneID)
        {
            try
            {
                var msg = new MessageItem
                {
                    EntityName = "SceneItem",
                    Operation = operation,
                    Type = TypeEnum.SceneData,
                    ChangeTime = DBTimeHelper.DBNowTime(),
                    Data = new List<CustomKeyValue>
                    {
                        new CustomKeyValue() {Key = "Id", Value = id},
                        new CustomKeyValue() {Key = "SceneID", Value = sceneID}
                    }
                };

                using (var msmq = ML.BC.Infrastructure.Ioc.GetService<IMsmqProvider>())
                {
                    msmq.Send<List<MessageItem>>(new Message(new List<MessageItem> { msg }));
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private static List<ImageUrl> MakeUrlWithPictureName(List<ImageUrl> imagesName)
        {
            if (null == imagesName) return null;
            var baseUrl = ConfigurationManager.ConnectionStrings["BaseUrlByName"].ToString();

            foreach (var info in imagesName)
            {
                info.OriginalPicture = baseUrl + info.OriginalPicture;
                info.ThumbnailPicture = baseUrl + info.ThumbnailPicture;
            }
            return imagesName;
        }
    }
}
