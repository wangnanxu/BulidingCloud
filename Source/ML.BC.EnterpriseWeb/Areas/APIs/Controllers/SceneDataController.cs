using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.EnterpriseWeb.Areas.APIs.Models;
using ML.BC.Services;
using ML.BC.Infrastructure;
using ML.BC.EnterpriseData.Model;
using ML.BC.Web.Framework.BaiduAPI;
using ML.BC.Web.Framework.ViewModels;
using ML.BC.Web.Framework.Security;

namespace ML.BC.EnterpriseWeb.Areas.APIs.Controllers
{
    [AuthorizeCheck]
    public class SceneDataController : APIControllerBase
    {
        private log4net.ILog _ilog = log4net.LogManager.GetLogger("LOG");
        private ISceneItemManagementService service;
        
        public SceneDataController()
        {
            service = Ioc.GetService<ISceneItemManagementService>();
        }
        [AllowCrossDomainPost]
        public ActionResult AddSceneData(AddSceneDataModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "AddSceneData", Serializer.ToJson(model)));

            var result = new StandardJsonResult<dynamic>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var sceneModel = new EnterpriseData.Model.SceneItem()
                {
                    PictureGuid = model.Guid,
                    SceneID = model.SceneID,
                    GPS = model.Address,
                    Address = GetAddressByGPS(model.Address),
                    CreateTime = model.Time,
                    Description = model.Content,
                    Relation = model.Relation,
                    Count = model.Count,
                    Type = (SceneItemType)model.Type,
                    IsExamine = model.IsExamine,
                    UserID = BCSession.User.UserID,
                };
                var _sceneModel = service.Add(sceneModel);

                result.Value = new { MessageID = _sceneModel == null ? "" : _sceneModel.Id, Address = _sceneModel.Address };
            });

            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "AddSceneData", Serializer.ToJson(result)));
            return result;
        }

        [AllowCrossDomainPost]
        public ActionResult UploadImage(UploadImageModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "UploadImage", Serializer.ToJson(model)));

            var result = new StandardJsonResult();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                Picture pic = new Picture();
                pic.PictureGuid = model.Guid;
                pic.PictureName = model.FileName;
                HttpFileCollectionBase files = HttpContext.Request.Files;
                if (files.Count > 0)
                {
                    pic.PictureStream = files[0].InputStream;
                    bool f = service.SavePicture(pic);
                }
            });

            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "UploadImage", Serializer.ToJson(result)));
            return result;
        }

        [AllowCrossDomainPost]
        public ActionResult GetSceneData(GetSceneDataModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "GetSceneData", Serializer.ToJson(model)));

            var result = new StandardJsonResult<dynamic>();
            result.Value = new List<SceneItemDto>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                int count = 0;
                ItemStatus sceneStatus = ItemStatus.All;
                bool f = Enum.TryParse<ItemStatus>(model.Status + "", out  sceneStatus);
                if (!f)
                {
                    sceneStatus = ItemStatus.All;
                }
                List<SceneItemDto> list = service.GetAllByStatus(BCSession.User.UserID,BCSession.User.Device,model.SceneID, sceneStatus, model.PageSize, model.Time, out count);
                result.Value = new { Count = count, Data = list };
            });

            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "GetSceneData", Serializer.ToJson(result)));
            return result;
        }

        //Comment，UpdateStatus, DeleteSceneItem, DeleteSceneComment

        /// <summary>
        /// 新增评论
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowCrossDomainPost]
        public ActionResult Comment(CommentDataModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "Comment", Serializer.ToJson(model)));

            var result = new StandardJsonResult();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                ItemStatus status = (ItemStatus)model.Status;
                var user = GetSession();
                var examine = new Examine()
                {
                    UserID = user.User.UserID,
                    UserName = user.User.UserName,
                    ExamineStatus = status
                };
                var SN = GetSession();
                service.AddCommentItem(model.MessageID,
                    new Comment()
                    {
                        CommentGuid = model.Guid,
                        Content = model.Content,
                        Time = model.Time,
                        UserID = SN.User.UserID,
                        UserName = SN.User.UserName
                    }, examine);

            });

            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "Comment", Serializer.ToJson(result)));
            return result;
        }
        /// <summary>
        /// 更新现场数据状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowCrossDomainPost]
        public ActionResult UpdateStatus(UpdateStatusModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "UpdateStatus", Serializer.ToJson(model)));
            var result = new StandardJsonResult();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                ItemStatus status = (ItemStatus)model.Status;
                var user = GetSession();
                var examine = new Examine()
                {
                    ExamineStatus = status,
                    UserID = user.User.UserID,
                    UserName = user.User.UserName,
                };
                service.SetSceneItemStatus(model.MessageID,examine);
            });

            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "UpdateStatus", Serializer.ToJson(result)));
            return result;
        }

        /// <summary>
        /// 删除现场数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowCrossDomainPost]
        public ActionResult DeleteSceneItem(DeleteSceneModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "DeleteSceneItem", Serializer.ToJson(model)));

            var result = new StandardJsonResult();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                service.DeleteSceneItem(model.MessageID, model.Time);
            });

            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "DeleteSceneItem", Serializer.ToJson(result)));
            return result;
        }
        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowCrossDomainPost]
        public ActionResult DeleteSceneComment(DeleteCommentDataModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "DeleteSceneComment", Serializer.ToJson(model)));

            var result = new StandardJsonResult();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                service.DeleteCommentItem(model.MessageID, model.CommentGuid, model.Time);
            });

            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "DeleteSceneComment", Serializer.ToJson(result)));
            return result;
        }
        [AllowCrossDomainPost]
        public ActionResult DeleteScenePicture(DeleteScenePicture model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "SyncSceneDataToServer", Serializer.ToJson(model)));
            var result = new StandardJsonResult();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                service.DeleteSceneItemPicture(model.SceneItemID, model.OrgPictureName);
            });
            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "DeleteSceneComment", Serializer.ToJson(result)));
            return result;
        }

        [AllowCrossDomainPost]
        public ActionResult SyncSceneDataToServer(SyncSceneDataToServerModel model)
        {
            _ilog.Info(string.Format("方法名：{0};参数：{1}", "SyncSceneDataToServer", Serializer.ToJson(model)));

            var result = new StandardJsonResult();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                
                var comments = string.IsNullOrEmpty(model.Comments)? new List<Comment>():  Serializer.FromJson<List<Comment>>(model.Comments);
                service.SyncSceneItemStatusAndComments(model.MessageID, (ItemStatus)model.Status, comments.ToArray());
            });

            _ilog.Info(string.Format("方法名：{0};执行结果：{1}", "SyncSceneDataToServer", Serializer.ToJson(result)));

            return result;
        }
        #region private methods

        private string GetAddressByGPS(string gps)
        {
            if (gps.Contains("|"))
            {
                gps = string.Join(",", gps.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            }

            string _Address = string.Empty;
            var gd = new ReverseGeocoding(new ReverseGeocodingParam()
            {
                ak = "",
                callback = "",
                coordtype = "",
                location = gps,
                output = "json",
                pois = 0,
                sn = ""
            });
            ReverseGeocoderResultModel gdresult = gd.GetGeocoderResult();
            _Address = GetAddress(gdresult);
            return _Address;
        }

        /// <summary>
        /// 返回尽可能 精确的地址信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string GetAddress(ReverseGeocoderResultModel model)
        {
            string result = "";
            if (model != null)
            {
                if (model.status == 0)
                {
                    if (model.result != null)
                    {
                        if (!string.IsNullOrEmpty(model.result.formatted_address))
                        {
                            return model.result.formatted_address;
                        }
                        if (!string.IsNullOrEmpty(model.result.sematic_description))
                        {
                            return model.result.sematic_description;
                        } if (!string.IsNullOrEmpty(model.result.business))
                        {
                            return model.result.business;
                        }

                        if (model.result.addressComponent != null)
                        {
                            if (!string.IsNullOrEmpty(model.result.addressComponent.district))
                            {
                                return model.result.addressComponent.district;
                            }
                            if (!string.IsNullOrEmpty(model.result.addressComponent.city))
                            {
                                return model.result.addressComponent.city;
                            }
                            if (!string.IsNullOrEmpty(model.result.addressComponent.province))
                            {
                                return model.result.addressComponent.province;
                            }
                            if (!string.IsNullOrEmpty(model.result.addressComponent.country))
                            {
                                return model.result.addressComponent.country;
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// [纬度,经度]
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private double[] GetLocation(GeocoderResultModel model)
        {
            double[] location = new double[2];
            if (model.result != null)
            {
                if (model.result.location != null)
                {
                    location[0] = model.result.location.lat;
                    location[1] = model.result.location.lng;
                }
            }
            return location;
        }


        /// <summary>
        /// 坐标取地址
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public string Test(string location)
        {
            string _Address = "";
            ReverseGeocoding gd = new ReverseGeocoding(new ReverseGeocodingParam()
            {
                ak = "",
                callback = "",
                coordtype = "wgs84ll",
                location = location,
                output = "json",
                pois = 0,
                sn = ""
            }
            );
            ReverseGeocoderResultModel gdresult = gd.GetGeocoderResult();
            _Address = GetAddress(gdresult);
            return _Address + "<br/><br/>" + Serializer.ToJson(gdresult) ;
        }
        /// <summary>
        /// 地址取坐标
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public string Test2(string address)
        {
            Geocoding gd = new Geocoding(new GeocodingParam()
            {
                address = address,
                ak = "",
                callback = "",
                city = "",
                output = "json",
                sn = ""
            }
           );
            GeocoderResultModel gdresult = gd.GetGeocoderResult();
            // _Address = GetAddress(gdresult);
            double[] ss = GetLocation(gdresult); float f1 = 1.123456789f; float f2 = 110.123456789f;
            
            return ss[0] + "," + ss[1] + "<br/><br/>"+f1+";;"+f2+"<br/>"+ Serializer.ToJson(gdresult); ;
        }
        #endregion
    }
}
