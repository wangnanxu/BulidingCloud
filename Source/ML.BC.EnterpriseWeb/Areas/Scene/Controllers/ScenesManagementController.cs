using ML.BC.EnterpriseWeb.Areas.Scene.Models;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Model;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Web.Framework.Controllers;
using System.Collections.Generic;
using System.Web.Mvc;
using ML.BC.Web.Framework;
using System;
using ML.BC.EnterpriseData.Model;
using System.Linq;
using ML.BC.EnterpriseData.Common;
using ML.BC.Web.Framework.BaiduAPI;
using ML.BC.Web.Framework.ViewModels;
using System.Web;
using System.IO;
using ML.BC.EnterpriseData.Model.Extend;
using ML.BC.Web.Framework.Security;

namespace ML.BC.EnterpriseWeb.Areas.Scene.Controllers
{
    [AuthorizeCheck]
    public class ScenesManagementController : BCControllerBase
    {

        private IFrontUserManagementService _userService = Ioc.GetService<IFrontUserManagementService>();
        private IProjectSceneManagementService _sceneService = Ioc.GetService<IProjectSceneManagementService>();
        private ISceneItemManagementService _sceneItemService = Ioc.GetService<ISceneItemManagementService>();
        private IEnterpriseRoleManagementService _roleService = Ioc.GetService<IEnterpriseRoleManagementService>();
   


        public ScenesManagementController()
        {

        }

        public ActionResult ScenesIndex()
        {
            ViewBag.UserId = GetSession().User.UserID ?? "";
            return View(ViewBag);
        }

        //gis地图首页

        public ActionResult GISMap()
        {
            ViewBag.UserId = GetSession().User.UserID ?? "";
            return View(ViewBag);

        }

        [PermissionControlAttribute(new string[] { Functions.Root_ProjectManagement_SceneListManagement_List, Functions.Root_ProjectManagement_GISMapManagement_SeeMap })]
        public ActionResult GetScenesList(SceneSearchModel queryModel)
        {
            string _enterpriseID = BCSession.User.EnterpriseID;
            int? _departmentID = BCSession.User.DepartmentID;
            string _userID = BCSession.User.UserID;
            var result = new StandardJsonResult<SceneResultModel>();
            result.Try(() =>
            {

                int amount;
                List<ScenesDto> temp;
                //bool searchMode = false;
                //筛选模式
                //if (queryModel.ProjectID != null)
                //{
                //    temp = _sceneService.GetAllSceneOfProject(queryModel.ProjectID, 9999, queryModel.page, out amount);
                //}
                ////搜索模式 仅供地图搜索 列表采用前端本地搜索
                //else 
                if (queryModel.ProjectName != null || queryModel.SceneName != null && queryModel.from.Equals("map"))
                {

                    //searchMode = true;
                    if (queryModel.SceneName == null) queryModel.SceneName = "";
                    //if (queryModel.Status == 0) queryModel.Status = Status.All;
                    //temp = _sceneService.GetAllSceneOfProject(queryModel.ProjectID, 9999, 1, out amount);
                    //temp = temps.Where(s => s.Name.Contains(queryModel.SceneName)&&(queryModel.Status==Status.All?true:s.Status==queryModel.Status)).ToList();
                    //if (queryModel.SceneName.Equals("") && queryModel.Status == Status.All) searchMode = false;
                    if (_departmentID == null || HasFunction(Functions.Root_ProjectManagement_SceneListManagement_ShowAll))
                    {
                        //Root_SystemSetting_SceneListManagement_ShowAll
                        temp = _sceneService.SearchSceneOnEnterprise(queryModel.SceneName, queryModel.ProjectName, _enterpriseID, queryModel.rows, queryModel.page, out amount);
                    }
                    else
                    {
                        temp = _sceneService.SearchSceneOnDepartment(queryModel.SceneName, queryModel.ProjectName, _departmentID, _enterpriseID, queryModel.rows, queryModel.page, out amount);
                    }
                }
                else if (queryModel.ProjectID != null)//按项目筛选
                {

                    //searchMode = true;
                    if (queryModel.SceneName == null) queryModel.SceneName = "";
                    if (queryModel.Status == 0) queryModel.Status = Status.All;
                    temp = _sceneService.GetAllSceneOfProject(queryModel.ProjectID, 9999, 1, out amount);
                    //temp = temps.Where(s => s.Name.Contains(queryModel.SceneName)&&(queryModel.Status==Status.All?true:s.Status==queryModel.Status)).ToList();
                    //if (queryModel.SceneName.Equals("") && queryModel.Status == Status.All) searchMode = false;
                    //if (_departmentID == null || HasFunction(Functions.Root_ProjectManagement_SceneListManagement_ShowAll))
                    //{
                    //    //Root_SystemSetting_SceneListManagement_ShowAll
                    //    temp = _sceneService.SearchSceneOnEnterprise(queryModel.SceneName, queryModel.ProjectName, _enterpriseID, queryModel.rows, queryModel.page, out amount);
                    //}
                    //else
                    //{
                    //    temp = _sceneService.SearchSceneOnDepartment(queryModel.SceneName, queryModel.ProjectName, _departmentID, _enterpriseID, queryModel.rows, queryModel.page, out amount);
                    //}
                }
                //
                //某项目的全部现场 列表
                else
                {
                    var projList = new List<ProjectDto>();
                    ProjectDto firstProj;
                    if (_departmentID == null || HasFunction(Functions.Root_ProjectManagement_SceneListManagement_ShowAll))
                    {
                        projList = _sceneService.SearchProjectOnEnterprise("", "", "", Status.All, _enterpriseID, 1, 1, out amount);
                        firstProj = projList[0];
                        if (firstProj != null)
                        {
                            temp = _sceneService.GetAllSceneOfProject(firstProj.ProjectID, 9999, 1, out amount);
                        }
                        else
                        {
                            temp = new List<ScenesDto>();
                        }
                    }
                    else
                    {
                        projList = _sceneService.SearchProjectOnDepartment("", "", "", Status.All, _departmentID, _enterpriseID, 1, 1, out amount);
                        firstProj = projList[0];
                        if (firstProj != null)
                        {
                            temp = _sceneService.GetAllSceneOfProject(firstProj.ProjectID, 9999, 1, out amount);
                        }
                        else
                        {
                            temp = new List<ScenesDto>();
                        }
                    }
                    //amount = queryModel.rows;
                }


                //处理结果
                result.Value = new SceneResultModel();
                result.Value.rows = new List<ScenesViewModel>();
                SceneTreeHelper _helper = new SceneTreeHelper();
                foreach (var dto in temp)
                {
                    var vm = (ScenesViewModel)dto;
                    //vm.WokerNames = new List<string>();
                    //foreach (var uid in vm.Workers)
                    //{
                    //    //vm.WokerNames.Add(_userService.GetFrontUserByUserID(uid).Name ?? "无此人员");
                    //}

                    vm.id = _helper.processSID(vm.SceneID);
                    //if (searchMode) { vm._parentId = 0; } else { 
                    vm._parentId = _helper.processSID(vm.ParentSceneID);
                    //}
                    //vm.Workers = _sceneService.GetRoleUserListOfSceneInEnterprise(_enterpriseID, vm.ProjectID, vm.SceneID);
                    vm.Workers = processGroupedUsers(dto.Wokers);
                    result.Value.rows.Add(vm);
                }
                result.Value.total = amount;
            });


            return new OringinalJsonResult<SceneResultModel> { Value = result.Value };

        }
        //扩展分组的用户信息 添加名字信息
        private List<KeyValuePair<RoleIdName, List<UserIdName>>> processGroupedUsers(List<GroupedUser> users)
        {
            var result = new List<KeyValuePair<RoleIdName, List<UserIdName>>>();
            foreach (var gpuser in users)
            {
                if (gpuser.UserID == null) continue;


                string RoleId = gpuser.roleId + "";
                string RoleName;
                try
                {
                    RoleName = _roleService.GetEnterpriseRoleByRoleID(gpuser.roleId).Name;
                }
                catch (Exception)
                {
                    RoleName = "(" + RoleId + ")角色不存在";

                }
                RoleIdName rin = new RoleIdName
                {
                    RoleId = RoleId,
                    RoleName = RoleName
                };
                List<UserIdName> list = new List<UserIdName>();
                foreach (var userId in gpuser.UserID)
                {
                    if (userId == null || userId.Equals("")) continue;
                    string UserName;
                    try
                    {
                        UserName = _userService.GetFrontUserByUserID(userId).Name;
                    }
                    catch (Exception)
                    {
                        UserName = "(" + userId + ")用户不存在";
                    }
                    var uin = new UserIdName
                    {
                        UserId = userId,
                        UserName = UserName
                    };
                    list.Add(uin);
                }
                KeyValuePair<RoleIdName, List<UserIdName>> kv = new KeyValuePair<RoleIdName, List<UserIdName>>(rin, list);
                result.Add(kv);
            }
            return result;
        }
        //精简以键值对形式分组的用户信息 去掉名字信息
        private List<ML.BC.Services.GroupedUser> processKVGroupedUsers(List<KeyValuePair<RoleIdName, List<UserIdName>>> users)
        {
            var result = new List<GroupedUser>();
            foreach (var kv in users)
            {
                if (kv.Key == null || kv.Value == null) continue;
                GroupedUser gu = new GroupedUser();
                gu.UserID = new List<string>();
                gu.roleId = Convert.ToInt32(kv.Key.RoleId);
                foreach (var user in kv.Value)
                {
                    gu.UserID.Add(user.UserId);
                }
                result.Add(gu);
            }
            return result;
        }
        [PermissionControlAttribute(Functions.Root_ProjectManagement_SceneListManagement_Add)]
        public ActionResult AddScene(ScenesNewModel model)
        {
            if(model.SceneTypestring!="")
            model.SceneType = model.SceneTypestring; 
            string _enterpriseID = BCSession.User.EnterpriseID;
            int? _departmentID = BCSession.User.DepartmentID;
            string _userID = BCSession.User.UserID;
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                if (model.EndDate < model.BeginDate)
                {
                    throw new KnownException("结束时间需要大于开始时间");
                }
                var dto = (ScenesDto)model;
                if (dto.Address == null || dto.Address.Equals(""))
                {
                    dto.LatitudeAndLongitude = "|";
                }
                else
                {
                    dto.Address = model.Address;
                    Geocoding gd = new Geocoding(new GeocodingParam()
                    {
                        address = dto.Address,
                        ak = "",
                        callback = "",
                        city = "",
                        output = "json",
                        sn = ""
                    });
                    GeocoderResultModel gdresult = new GeocoderResultModel();
                    try
                    {
                        gdresult = gd.GetGeocoderResult();
                    }

                    finally
                    {
                        double[] ss = GetLocation(gdresult);
                        dto.LatitudeAndLongitude = ss[0] + "|" + ss[1];
                    }


                }
                dto.EnterpriseID = _enterpriseID;
                dto.RegistDate = DateTime.Now;
                if (model.RoleWorkers == null) dto.Wokers = new List<GroupedUser>();

                else
                {
                    List<GroupedUser> list = new List<GroupedUser>();
                    foreach (var u in model.RoleWorkers)
                    {
                        list.Add(u);
                    }
                    dto.Wokers = list;
                }
                if (dto.ParentSceneID == null || dto.ParentSceneID.Equals("")) dto.ParentSceneID = "-1";
                result.Value = _sceneService.AddScene(dto) != null;

            });

            return result;
        }

        static private double[] GetLocation(GeocoderResultModel model)
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
        [PermissionControlAttribute(Functions.Root_ProjectManagement_SceneListManagement_Delete)]
        public ActionResult DeleteScene(string SceneID)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                result.Value = _sceneService.DeleteScene(SceneID);

            });

            return result;
        }
        [PermissionControlAttribute(Functions.Root_ProjectManagement_SceneListManagement_Edit)]
        public ActionResult UpdateScene(SceneUpdateModdel model)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                if (model.EndDate < model.BeginDate)
                {
                    throw new KnownException("结束时间需要大于开始时间");
                }
                if (model.SceneTypestring != "")
                    model.SceneType = model.SceneTypestring;
                var dto = (ScenesDto)model;
                if (dto.Address == null || dto.Address.Equals(""))
                {
                    dto.LatitudeAndLongitude = "";
                }
                else
                {
                    Geocoding gd = new Geocoding(new GeocodingParam()
                    {
                        address = dto.Address,
                        ak = "",
                        callback = "",
                        city = "",
                        output = "json",
                        sn = ""
                    });
                    GeocoderResultModel gdresult = gd.GetGeocoderResult();
                    double[] ss = GetLocation(gdresult);
                    dto.LatitudeAndLongitude = ss[0] + "|" + ss[1];
                }
                if (model.RoleWorkers == null) dto.Wokers = new List<GroupedUser>();
                else
                {
                    List<GroupedUser> list = new List<GroupedUser>();
                    foreach (var u in model.RoleWorkers)
                    {
                        list.Add(u);
                    }
                    dto.Wokers = list;
                }
                result.Value = _sceneService.UpdateScene(dto);

            });
            if (!result.Value && result.Message==null)
            {
                result.Message = "没有内容更新";

            }
            return result;
        }

        public ActionResult GetProjList()
        {
            string _enterpriseID = BCSession.User.EnterpriseID;
            int? _departmentID = BCSession.User.DepartmentID;
            string _userID = BCSession.User.UserID;
            var result = new StandardJsonResult<List<SimpleListModel>>();
            List<SimpleListModel> list = new List<SimpleListModel>();
            result.Try(() =>
            {
                List<ProjectDto> temp;
                int amount;
                if (_departmentID == null || HasFunction(Functions.Root_ProjectManagement_ProjectListManagement_ShowAll))
                {
                    temp = _sceneService.SearchProjectOnEnterprise("", "", "", Status.All, _enterpriseID, 999, 1, out amount);

                }
                else
                {
                    temp = _sceneService.SearchProjectOnDepartment("", "", "", Status.All, _departmentID, _enterpriseID, 999, 1, out amount);
                }


                foreach (var dto in temp)
                {
                    list.Add(new SimpleListModel
                    {
                        Id = dto.ProjectID,
                        Name = dto.Name
                    });
                }
                if (list != null && list[0] != null) { list[0].selected = true; }
                result.Value = list;
            });

            return result;

        }


        //public JsonResult GetWorkerList()
        //{
        //    var result = new StandardJsonResult<List<SimpleListModel>>();
        //    result.Try(() =>
        //    {
        //        var dto = new FrontUserDto
        //        {
        //            DepartmentID = _departmentID
        //        };
        //        int amount;
        //        List<FrontUserDto> list;
        //        //如果有权限 获取所有
        //        if (_departmentID == null || HasFunction(Functions.Root_SystemSetting_UserManagement_ShowAll))
        //        {
        //            list = _userService.GetAllFrontUser(_enterpriseID, 9999, 1, out amount);
        //        }
        //        else
        //        //获取部门所属部分
        //        {
        //            list = _userService.GetFrontUserByPartialInfo(dto, 999, 1, out amount);
        //        }

        //        result.Value = new List<SimpleListModel>();
        //        foreach (var udto in list)
        //        {
        //            var ent = new SimpleListModel
        //            {
        //                Id = udto.UserID,
        //                Name = udto.Name
        //            };
        //            result.Value.Add(ent);
        //        }
        //    });
        //    if (result.Success)
        //    {

        //    }
        //    else
        //    {
        //        var errorList = new List<SimpleListModel>();
        //        errorList.Add(new SimpleListModel
        //        {
        //            Id = "",
        //            Name = "service发生错误"
        //        });


        //    }
        //}
        ////获取某现场的已有人员 todo--
        //public JsonResult GetWorkerListOfScene(string SceneID, string ProjectID)
        //{
        //    var result = new StandardJsonResult<List<KeyValuePair<RoleIdName, List<UserIdName>>>>();
        //    result.Value = new List<KeyValuePair<RoleIdName, List<UserIdName>>>();
        //    result.Try(() =>
        //    {
        //        if (_departmentID == null || HasFunction(Functions.Root_SystemSetting_UserManagement_ShowAll) || HasFunction(Functions.Root_ProjectManagement_SceneListManagement_ShowAll))
        //        {
        //            result.Value = _sceneService.GetRoleUserListOfSceneInEnterprise(_enterpriseID, ProjectID, SceneID);


        //        }
        //        else
        //        {
        //            result.Value = _sceneService.GetRoleUserListOfSceneInDepartment(_enterpriseID, _departmentID, ProjectID, SceneID);

        //        }
        //    });

        //    return result;

        //}

        //获取某角色的侯选人员 todo--
        public ActionResult GetWorkerListOfRole(string RoleID, string ProjectID)
        {
            string _enterpriseID = BCSession.User.EnterpriseID;
            int? _departmentID = BCSession.User.DepartmentID;
            string _userID = BCSession.User.UserID;
            var result = new StandardJsonResult<List<UserIdName>>();
            result.Value = new List<UserIdName>();
            result.Try(() =>
            {
                if (_departmentID == null || HasFunction(Functions.Root_SystemSetting_UserManagement_ShowAll) || HasFunction(Functions.Root_ProjectManagement_SceneListManagement_ShowAll))
                {
                    var t = _sceneService.GetRoleUserListOfEnterprise(_enterpriseID, ProjectID);
                    foreach (var ur in t)
                    {

                        if (ur.Key != null && ur.Key.RoleId.Equals(RoleID))
                        {
                            result.Value = ur.Value;
                        }

                    }
                }
                else
                {
                    var t = _sceneService.GetRoleUserListOfDepartment(_enterpriseID, _departmentID, ProjectID);
                    foreach (var ur in t)
                    {
                        if (ur.Key.RoleId.Equals(RoleID))
                        {
                            result.Value = ur.Value;
                        }

                    }
                }
            });
            return new OringinalJsonResult<List<UserIdName>> { Value = result.Value };

        }

        //获取现场类型
        public ActionResult GetSceneTypeList()
        {
            ISceneTypeManagementService _typeService = Ioc.GetService<ISceneTypeManagementService>();
            string _enterpriseID = BCSession.User.EnterpriseID;

            var result = new StandardJsonResult<List<SceneTypeDto>>();
            result.Value = new List<SceneTypeDto>();
            result.Try(() =>
            {
                var list = _typeService.GetAllSceneTypeList(_enterpriseID);
                foreach (var type in list)
                {
                    if (type.ParentID == null) type.ParentID = 0;
                }
                result.Value = list;
            });
            return result;

        }


        //获取某项目的可用角色详单 todo
        public ActionResult GetRoleListOfProject(string ProjectID)
        {
            string _enterpriseID = BCSession.User.EnterpriseID;
            int? _departmentID = BCSession.User.DepartmentID;
            string _userID = BCSession.User.UserID;
            var result = new StandardJsonResult<List<RoleIdName>>();
            result.Value = new List<RoleIdName>();
            result.Try(() =>
            {
                //var list = new List<RoleIdName>();
                //var r = new RoleIdName
                //{
                //    RoleId = "1",
                //    RoleName = "施工员"
                //};
                //list.Add(r);
                //r.RoleId = "1";
                //r.RoleName = "质检员";
                //list.Add(r);
                //result.Value = list;
                var t = _sceneService.GetRoleUserListOfEnterprise(_enterpriseID, ProjectID);
                foreach (var roleUsers in t)
                {
                    RoleIdName item = roleUsers.Key;
                    if (item != null)
                    {
                        result.Value.Add(item);
                    }
                }
                result.Value = result.Value.Distinct().ToList();

            });

            return result;

        }

        //现场临检等数据相关

        //获取现场的临检数据
        [PermissionControlAttribute(Functions.Root_ProjectManagement_SceneListManagement_ViewSceneData)]
        public ActionResult GetSceneItemList(SceneItemSearchModel model)
        {
            var result = new StandardJsonResult<SceneItemResultModel>();
            bool sucessFlag = true;
            if (!ModelState.IsValid)
            {
                throw new KnownException(ModelState.GetFirstError());
            }
            result.Try(() =>
            {
                if (model.page < 1) model.page = 1;
                if (model.rows < 1) model.rows = 10;
                int amount;
                //判断有无权限查看完工数据
                try
                {
                    if ((int)_sceneService.GetOneScene(model.SceneID).Status == 3 && !HasFunction(Functions.Root_AppPermission_SceneManage_ViewAchievedScene))
                    {
                        result.Value = new SceneItemResultModel();
                        result.Fail("服务器:没有权限查看完工数据");
                        sucessFlag = false;
                        return;
                    }
                }
                catch (Exception)
                {
                    result.Value = new SceneItemResultModel();
                    result.Fail("服务器:发生异常");
                    sucessFlag = false;
                    return;
                }
                var temp = _sceneItemService.GetAllByStatus(model.SceneID, ItemStatus.All, model.rows, model.page, out amount);
                result.Value = new SceneItemResultModel();
                result.Value.rows = new List<SceneItemViewModel>();
                foreach (var item in temp)
                {
                    SceneItemViewModel sceneItem = (SceneItemViewModel)item;
                    foreach (var comment in sceneItem.comments)
                    {
                        if (string.IsNullOrEmpty(comment.UserName))
                        {
                            try
                            {
                                var userc = _userService.GetFrontUserByUserID(comment.UserID);
                                if (userc == null) comment.UserName = "用户不存在";
                                else comment.UserName = userc.Name;
                            }
                            catch (Exception)
                            {
                                comment.UserName = "用户名获取失败";
                            }
                        }
                        comment.Stime = comment.Time.ToString("yyyy-MM-dd HH:mm");
                    }
                    result.Value.rows.Add(sceneItem);
                }
                result.Value.total = amount;

            });
            if (!sucessFlag) result.Success = false;
            return result;
        }

        //删除现场的临检数据
        [PermissionControlAttribute(Functions.Root_ProjectManagement_SceneListManagement_DeleteSceneData)]
        public ActionResult DeleteSceneItem(string SceneItemID)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                result.Value = _sceneItemService.DeleteSceneItem(SceneItemID);

            });
            return result;
        }

        //审核现场的临检数据
        [PermissionControlAttribute(Functions.Root_ProjectManagement_SceneListManagement_VerifySceneData)]
        public ActionResult CheckSceneItem(string SceneItemID, int status, string content)
        {
            string _enterpriseID = BCSession.User.EnterpriseID;
            int? _departmentID = BCSession.User.DepartmentID;
            string _userID = BCSession.User.UserID;
             
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                //result.Value = _sceneItemService.SetSceneItemStatus(SceneItemID, (ItemStatus)status, DateTime.Now);
                Examine examine = new Examine() { 
                   UserID=BCSession.User.UserID,
                   UserName=BCSession.User.UserName,
                    ExamineStatus=(ItemStatus)status,
                };
                var comment = new Comment
                {
                    UserID = _userID,
                    Content = (status == 1 ? "通过：" : "整改：") + content ?? "",
                };
                    result.Value = _sceneItemService.AddCommentItem(SceneItemID, comment, examine) != null;
            });
            return result;
        }

        //评论现场的临检数据
        [PermissionControlAttribute(Functions.Root_ProjectManagement_SceneListManagement_CommentSceneData)]
        public ActionResult AddSceneItemComment(string SceneItemID,string status, string comment)
        {
            string _enterpriseID = BCSession.User.EnterpriseID;
            int? _departmentID = BCSession.User.DepartmentID;
            string _userID = BCSession.User.UserID;
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                comment = comment.Replace("\n", " ");
                var cmt = new Comment
                {
                    CommentGuid = Guid.NewGuid(),
                    Content = comment,
                    Time = DateTime.Now,
                    UserID = _userID,
                    UserName = BCSession.User.UserName
                };

                Examine exmine = new Examine() { 
                 ExamineStatus=(ItemStatus)Convert.ToInt32( status),
                  UserID=BCSession.User.UserID,
                 UserName=BCSession.User.UserName
                };
                try
                {
                    result.Value = !(_sceneItemService.AddCommentItem(SceneItemID, cmt, exmine) == null);
                }
                catch (Exception e)
                {
                    result.Message = "信息:" + e.Message;
                    result.Value = false;
                }


            });
            return result;
        }


        //删除现场临检的评论

        public ActionResult DeleteSceneItemComment(string SceneItemID, string CommentGuid)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {

                try
                {
                    result.Value = _sceneItemService.DeleteCommentItem(SceneItemID, new Guid(CommentGuid), DateTime.Now);
                }
                catch (Exception e)
                {
                    result.Message = "信息:" + e.Message;
                    result.Value = false;
                }
            });
            return result;
        }


        //增加现场的临检数据
        [PermissionControlAttribute(Functions.Root_ProjectManagement_SceneListManagement_AddSceneData)]
        public ActionResult AddSceneItem(SceneItemNewModel model,string prevSceneItemId)
        {
            string _enterpriseID = BCSession.User.EnterpriseID;
            int? _departmentID = BCSession.User.DepartmentID;
            string _userID = BCSession.User.UserID;
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                var files = Request.Files;

                //if (model.files == null || model.files[0] == null) model.files = new List<HttpPostedFileBase>();
                //HttpFileCollectionBase files = HttpContext.Request.Files;
                int fileCount = files.Count;
                if (model.SceneItemType == SceneItemType.Checkin || model.SceneItemType == SceneItemType.Checkout) model.content = " ";
                if (string.IsNullOrEmpty(model.content) && files.Count == 0)
                {
                    result.Fail("没有图片且没有内容.");
                }
                else
                {
                    Guid guid = Guid.NewGuid();
                    List<string> imagesUrl = new List<string>();
                    if (fileCount > 0)
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            var file = files[i];
                            int idx = file.FileName.LastIndexOf(".") + 1;
                            string suffix = file.FileName.Substring(idx);
                            //判断图片格式
                            if (idx > 1 && suffix.Equals("jpg") || suffix.Equals("jpeg") || suffix.Equals("png") || suffix.Equals("gif"))
                            {
                                //TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                string fileName = DateTime.Now.ToString("yyyyMMddHHmmssffff") + "." + suffix;//Convert.ToInt64(ts.TotalMilliseconds).ToString() + "." + suffix;
                                Picture pic = new Picture();
                                pic.PictureGuid = guid;
                                pic.PictureName = fileName;
                                byte[] s = new byte[file.InputStream.Length];
                                file.InputStream.Read(s, 0, s.Length);
                                MemoryStream ms = new MemoryStream(s);
                                pic.PictureStream = ms;
                                try
                                {
                                    var r = _sceneItemService.SavePicture(pic);
                                }
                                catch (Exception e)
                                {
                                    result.Message = e.Message;
                                }
                            }
                            else
                            {
                                fileCount = fileCount - 1;
                            }

                        }
                    }
                    var cmt = new SceneItem
                    {
                        Address = "电脑发布",
                        Comments = new List<Comment>(),
                        Description = model.content,
                        CreateTime = DateTime.Now,
                        Type = model.SceneItemType,
                        SceneID = model.SceneID,
                        UserID = _userID,
                        GPS = "",
                        Count = fileCount,
                        Relation=prevSceneItemId
                    };
                    
                    if (fileCount > 0) cmt.PictureGuid = guid;
                    result.Value = _sceneItemService.Add(cmt) != null;
                }

            });
            return result;
        }

        //归档现场的临检数据
        [PermissionControlAttribute(Functions.Root_ProjectManagement_SceneListManagement_ArchiveSceneData)]
        public ActionResult ArchiveSceneItem(string SceneItemID)
        {
            string _enterpriseID = BCSession.User.EnterpriseID;
            int? _departmentID = BCSession.User.DepartmentID;
            string _userID = BCSession.User.UserID;
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {

                var comment = new Comment
                {
                    UserID = _userID,
                    Content = "归档了该条目.",
                    Time = DateTime.Now
                };
                var user = GetSession();
                var examine = new Examine()
                {
                    UserID = user.User.UserID,
                    UserName = user.User.UserName,
                    ExamineStatus = ItemStatus.Final,
                };
                //_sceneItemService.AddCommentItem(SceneItemID, comment,ItemStatus.Final);
                result.Value = _sceneItemService.SetSceneItemStatus(SceneItemID,examine);
            });
            return result;
        }
        [PermissionControlAttribute(Functions.Root_ProjectManagement_SceneListManagement_VerifySceneData)]
        
        public ActionResult DeleteImg(string imgName,string sid)
        {
            var result = new StandardJsonResult();
            result.Try(() => {
                 _sceneItemService.DeleteSceneItemPicture(sid,imgName);
            });
            if (result.Success)
            {
                result.Message = "图片删除成功！";
            }
            else
                result.Message = "图片删除失败";
            return result;
        }
        public ActionResult AddScan(string sceneId,int type)
        {
            var scanservice=ML.BC.Infrastructure.Ioc.GetService<IScan>();
            var result = new StandardJsonResult();
            result.Try(()=>{
                ScanDto para = new ScanDto()
                {
                 ObjectID=sceneId,
                 Type=(ScanType)type,
                 UserID=BCSession.User.UserID
                };
                scanservice.AddScan(para);
            });
            if (!result.Success)
            {
                result.Message = "计数统计失败！";
            }
            return result;
        }

        public FileResult downPickture(string SceneID,string FileName)
        {
            var result = new StandardJsonResult<byte[]>();
            
            result.Try(() => {
                byte[] ret = _sceneItemService.DownloadPicturePack(SceneID);
               result.Value=ret;
            });
            return File(result.Value, "text/plain",FileName+".zip");
        }
        //static public List<string> processString(string org)
        //{
        //    var list = new List<string>();
        //    list = org.Split('|').ToList();
        //    return list;
        //}
        //static public string processList(List<string> org)
        //{
        //    StringBuilder s = new StringBuilder();
        //    foreach (var str in org)
        //    {
        //        s.Append(str + "|");
        //    }
        //    return s.ToString();
        //}
    }
}
