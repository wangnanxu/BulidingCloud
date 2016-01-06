using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Services;
using ML.BC.Infrastructure.Model;
using ML.BC.Infrastructure.Mvc;
using ML.BC.EnterpriseWeb.Areas.Account.Models;
using ML.BC.EnterpriseData.Common;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Web.Framework.Security;
using ML.BC.EnterpriseWeb.Areas.Unit.Models;
namespace ML.BC.EnterpriseWeb.Areas.Account.Controllers
{
    
    public class MessageManagementController : BCControllerBase
    {
        //
        // GET: /Account/MessageManagement/
        private IChatMessageService serrvice;
        public ActionResult MessageIndex()
        {
            return View();
        }
        public MessageManagementController()
        {
            serrvice = ML.BC.Infrastructure.Ioc.GetService<IChatMessageService>();
        }
        public ActionResult Getd(string jsonstring,int page)
        {
            return View();
        }
        public ActionResult GetList(string jsonstring, int rows, int page)
        {
            var result = new StandardJsonResult<MessageResult>();
            result.Try(() =>
            {
               
                List<MessageModel> mylist = new List<MessageModel>();
                List<ChatDto> list = new List<ChatDto>();
                int amount = 0;
                //初始化模式
                if (jsonstring == null)
                {
                    list = serrvice.SearchChatMessages(BCSession.User.UserID, null, null, "", ReadStatus.All, rows, page, out amount);
                  
                }
                else//搜索模式
                {
                    MessageSearchModel model = ML.BC.Infrastructure.Serializer.FromJson<MessageSearchModel>(jsonstring);
                    if (model.Status == null)
                    {
                        model.Status = 4;
                    }
                    list = serrvice.SearchChatMessages(BCSession.User.UserID, model.BeginDate, model.EndDate, model.Sender, (ReadStatus)model.Status, rows, page, out amount);
                }
                foreach (var a in list)
                {
                    MessageModel m= new MessageModel()
                    {
                        Message = a.Message,
                        Sender = a.SenderName,
                        SendTime = a.SendTime.ToString(),
                        Status = (int)a.IsRead,
                    
                    };
                    m.UserList = new List<string>();
                    foreach (var i in a.UserList)
                    {
                        m.UserList.Add(i.Key+","+i.Value);
                    }
                    mylist.Add(m);
                }
                result.Value = new MessageResult();
                result.Value.rows = mylist;
                result.Value.total = amount;
            });
            if (!result.Success)
            {
                result.Value = new MessageResult();
            }
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadMessage(string messageid)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() => {
                Guid myguid = new Guid();
                if (Guid.TryParse(messageid, out myguid))
                {
                    result.Value = serrvice.ChangeReadStatus(myguid,BCSession.User.UserID);
                    result.Message = "已成功标记已读！";
                }
                else
                {
                    throw new KnownException("消息编号错误，标记失败");
                }
            });
            if (!result.Success)
                result.Message = "标记失败！";
            return result;
        }
        public ActionResult SendMessage(string userid, string sendtext)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() => {
                Guid messageid = Guid.NewGuid();
                userid = userid.Replace(",", "|");
                userid = userid + "|" + BCSession.User.UserID;
                ChatMessageDto cd = new ChatMessageDto()
                {
                    EnterpriseID = BCSession.User.EnterpriseID,
                    IsRead = ReadStatus.NoRead,
                    Message = sendtext,
                    MessageID = messageid,
                    Recipients = userid,
                    SendTime = DateTime.Now,
                    SendUserID = BCSession.User.UserID,
                    SendUserName = BCSession.User.UserName,
                };
                result.Value = serrvice.ProcessChatMessage(cd);
                if (!result.Value)
                    result.Message = "发送失败";
                result.Message = "发送成功";
            });
            if (!result.Success)
                result.Message = "发送失败";
            return result;
        }
        public ActionResult GetUser()
        {
            var result = new StandardJsonResult<List<UserTree>>();
            result.Try(() => {
                List<FrontUserDto> userlist = GetUserList();
               //List< FrontUserDto> my = userlist.Where(u => u.UserID == BCSession.User.UserID).ToList();
               //if (my.Count > 0)
               //{
               //    userlist.Remove(my[0]);
               //}
               List<UserTree> departmentlist=new List<UserTree>();
               departmentlist.Add(new UserTree()
               {
                   iconCls = "icon-user",
                   children = new List<UserTree>(),
                   id = "",
                   text = "所有人",
                   type = 3

               });
               departmentlist = departmentlist.Concat(GetDepartment(userlist)).ToList();
                //所有人标签
               
                if (departmentlist.Count > 0)
                {
                    result.Value = new List<UserTree>();
                    result.Value = departmentlist;
                }
            });
            if (!result.Success)
                result.Value = new List<UserTree>();
            return Json(result.Value,JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取所有部门转换成树形结构
        /// </summary>
        /// <returns></returns>
        public List<UserTree> GetDepartment(List<FrontUserDto> userlist)
        {
           
                var service = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseDepartmentManagementService>();
                List<DepartmentDto> list = new List<DepartmentDto>();
                if (HasFunction(Functions.Root_SystemSetting_OrganizationManagement_ShowAll))
                    list = service.GetMyDepartment(BCSession.User.EnterpriseID);
                else if (BCSession.User.DepartmentID.HasValue)
                    list = service.GetMyDepartment(BCSession.User.EnterpriseID, BCSession.User.DepartmentID);
                if (list != null && list.Count > 0)
                {
                    List<DepartmentManageModel> mylist = new List<DepartmentManageModel>();
                    foreach (var l in list)
                    {
                        if (l.Available)
                        {
                            DepartmentManageModel m = new DepartmentManageModel()
                            {
                                DepartmentID = l.DepartmentID,
                                Description = l.Description,
                                _parentId = l.ParentID,
                                Deleted = l.Deleted,
                                Available = l.Available,
                                EnterpriseID = l.EnterpriseID,
                                Name = l.Name,
                                ParentID = l.ParentID
                            };
                            mylist.Add(m);
                        }
                    }
                    mylist[0]._parentId = 0;//不在根节点部门下的管理者不能显示
                    List<UserTree> all = new List<UserTree>();
                    
                    var noDepartmentUser = userlist.Where(u => u.DepartmentID == null).ToList();
                    foreach (var nouser in noDepartmentUser)
                    {
                        all.Add(new UserTree() { 
                         id=nouser.UserID,
                         text=nouser.Name,
                         iconCls = "icon-user",
                         type=1,
                         children=new List<UserTree>()
                        });
                    }
                    foreach (var rot in mylist)
                    {
                        if (rot._parentId == 0)
                        {
                            UserTree root = new UserTree();
                            root.id = rot.DepartmentID.ToString();
                            root.text = rot.Name;
                            root.type = 0;
                            root.children = new List<UserTree>();
                            root.children = process2TreeModel(mylist, root,userlist);
                            var res = userlist.Where(u => u.DepartmentID == Convert.ToInt32(root.id));
                            if (res.Count() > 0)
                            {
                                foreach (var u in res)
                                {
                                    root.children.Add(new UserTree()
                                    {
                                        id = u.UserID,
                                        text = u.Name,
                                     type=1,
                                     iconCls="icon-user",
                                        children = new List<UserTree>()
                                    });
                                }
                            }
                            all.Add(root);
                        }
                    }
                    return all;

                }
                else
                    return new List<UserTree>();
          
        }
        /// <summary>
        /// 获取部门下所有用户
        /// </summary>
        /// <returns></returns>
        public List<FrontUserDto> GetUserList()
        {
            var service = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseDepartmentManagementService>();
            var userservice = ML.BC.Infrastructure.Ioc.GetService<IEnterpriseUserManagementService>();
            List<DepartmentDto> list = new List<DepartmentDto>();
            List<FrontUserDto> userlist = new List<FrontUserDto>();
            List<int> departmentlist = new List<int>();
            int amount;
            if (HasFunction(Functions.Root_SystemSetting_OrganizationManagement_ShowAll))
            {
                userlist=userservice.GetUserList(BCSession.User.EnterpriseID,1,1000,out amount);
            }
            else if (BCSession.User.DepartmentID.HasValue)
            {
               
                list = service.GetMyDepartment(BCSession.User.EnterpriseID, BCSession.User.DepartmentID);
                foreach (var id in list)
                {
                    departmentlist.Add(id.DepartmentID);
                }
                userlist = userservice.GetUserByDepartmentIDs(departmentlist, 1000, 1,out amount);
            }
            if (userlist != null && userlist.Count > 0)
                return userlist;
            else
                return new List<FrontUserDto>();
          
        }
        private List<UserTree> process2TreeModel(List<DepartmentManageModel> source, UserTree curRootNode,List<FrontUserDto> userlist)
        {

            List<UserTree> resultlist = new List<UserTree>();
            if (source != null && source.Count() > 0)//出口1
            {
                if (curRootNode == null)
                {
                    return resultlist;//出口2
                }

                //当前节点的直接子节点list
                IEnumerable<DepartmentManageModel> sublist = source.Where(m => m.ParentID == Convert.ToInt32(curRootNode.id));
                if (sublist == null || sublist.Count() == 0) return resultlist;
                // var pmodel=sublist.FirstOrDefault();

                foreach (var dep in sublist)//出口3
                {
                    UserTree t = new UserTree()
                    {
                        id = dep.DepartmentID.ToString(),
                        text = dep.Name,
                      type=0
                    };
                    t.children = new List<UserTree>();
                    t.children = process2TreeModel(source, t,userlist);
                    var result = userlist.Where(u => u.DepartmentID == Convert.ToInt32(t.id));
                    if (result.Count() > 0)
                    {
                        foreach (var u in result)
                        {
                            t.children.Add(new UserTree() { 
                             id=u.UserID,
                             text=u.Name,
                             type=1,
                             iconCls="icon-user",
                             children=new List<UserTree>()
                            });
                        }
                    }
                    curRootNode.children.Add(t);
                }
                return curRootNode.children;
            }
            return new List<UserTree>();
        }

       
        }
}
