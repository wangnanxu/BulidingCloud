using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Sql;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Common;
using ML.BC.EnterpriseData.Model;
using ML.BC.EnterpriseData.Model.Extend;
using ML.BC.EnterpriseData.MongoDb;
using ML.BC.Infrastructure.MsmqHelper;
using ML.BC.Infrastructure.Exceptions;
// ReSharper disable All

namespace ML.BC.Services
{
    public class ChatMessageService : IChatMessageService
    {
        public bool ProcessChatMessage(ChatMessageDto msgDto)
        {
            if(string.IsNullOrEmpty(msgDto.Recipients))    throw new KnownException("收信人不能为空");
            if (string.IsNullOrEmpty(msgDto.Message)) throw new KnownException("发送消息不能为空");
            if (!string.IsNullOrEmpty(msgDto.Recipients)) return SendChatMessage(msgDto);
            if (!string.IsNullOrEmpty(msgDto.EnterpriseID)) return AnnounceMessageByEnterpriseID(msgDto);
            if (0 != msgDto.DepartMentIDs.Count()) return AnnounceMessageByDepartMentIDs(msgDto);
            return false;
        }

        public List<ChatMessageDto> GetChatMessages(string userID, string deviceID, int pageSize, DateTime queryTime, out int count)
        {
            try
            {
                var mgdb = new MongoDbProvider<UserMessageQueueItem>();
                var query = mgdb.GetAll(o => o.UserID == userID && o.Time < queryTime && ((ChatMessage)o.Data).Message != null)
                        .Select(o =>
                        {
                            var temp = (ChatMessage)o.Data;
                            return new ChatMessageDto 
                            {
                                SendUserID = temp.SendUserID,
                                SendUserName = temp.SendUserName,
                                SendUserPicture = temp.SendUserPicture,
                                EnterpriseID = temp.EnterpriseID,
                                Recipients = temp.Recipients,
                                Message = temp.Message,
                                MessageID = temp.MessageID,
                                IsRead = temp.IsRead,
                                SendTime = temp.SendTime.ToLocalTime(),
                                Time = o.Time.ToLocalTime()
                            };
                        }).Distinct();

                count = query.Count();
                if (0 == count) return new List<ChatMessageDto>();
                if (pageSize < 1) pageSize = 10;
                var re = query.OrderByDescending(obj => obj.Time)
                    .Take(pageSize)
                    .ToList();

                return re;
            }
            catch (Exception ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(ChatMessageService));
                logger.Error(ex.Message);
                throw;
            }
        }
        private static bool SendChatMessage(ChatMessageDto msgDto)
        {
            try
            {
                var msg = MakeCommonInfo(msgDto);
                msg.Data.Add(new CustomKeyValue { Key = "Recipients", Value = msgDto.Recipients });
                return Send2Msmq(msg);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private static bool AnnounceMessageByEnterpriseID(ChatMessageDto msgDto)
        {
            try
            {
                var msg = MakeCommonInfo(msgDto);
                msg.Data.Add(new CustomKeyValue { Key = "EnterpriseID", Value = msgDto.EnterpriseID });
                return Send2Msmq(msg);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public bool AnnounceMessageByDepartMentIDs(ChatMessageDto msgDto)
        {
            try
            {
                var msg = MakeCommonInfo(msgDto);
                msg.Data.AddRange(msgDto.DepartMentIDs.Select(d => new CustomKeyValue { Key = "DepartMentID", Value = d.ToString() }));
                return Send2Msmq(msg);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private static MessageItem MakeCommonInfo(ChatMessageDto msgDto)
        {
            return new MessageItem
            {
                EntityName = "ChatMessage",
                Type = TypeEnum.Message,
                Operation = OperationEnum.Added,
                ChangeTime = DBTimeHelper.DBNowTime(),
                Data =
                    new List<CustomKeyValue>
                        {
                            new CustomKeyValue{Key = "SendUserID",Value =msgDto.SendUserID },                        
                            new CustomKeyValue {Key = "Text", Value = msgDto.Message},
                            new CustomKeyValue{Key = "SendTime",Value = msgDto.SendTime.ToString()},
                            new CustomKeyValue{Key = "MessageID",Value = msgDto.MessageID.ToString()}
                        }
            };
        }
        private static bool Send2Msmq(MessageItem msg)
        {
            try
            {
                using (var msmq = ML.BC.Infrastructure.Ioc.GetService<IMsmqProvider>())
                {
                    msmq.Send<List<MessageItem>>(new Message(new List<MessageItem> { msg }));
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
                //throw e;
            }
        }
        public List<ChatDto> SearchChatMessages(string userId, DateTime? beginTime, DateTime? endTime, string objectiveName, EnterpriseData.Common.ReadStatus status, int pageSize, int pageNumber, out int amount)
        {
            try
            {
                if (string.IsNullOrEmpty(objectiveName))
                    objectiveName = "";
                //if (beginTime > endTime)
                //    throw new KnownException("起止时间不合法");
                using (var db = new BCEnterpriseContext())
                {
                    var mgdb = new MongoDbProvider<UserMessageQueueItem>();
                    var msg = mgdb.GetAll(x => x.UserID == userId && ((ChatMessage)x.Data).IsRead == ReadStatus.NoRead);
                    foreach (var chat in msg)
                    {
                        ((ChatMessage)chat.Data).IsRead = EnterpriseData.Common.ReadStatus.Read;
                        mgdb.Update(chat);
                    }
                    var uidlist = db.FrontUsers.Where(x => x.Name.Contains(objectiveName)).Select(n => n.UserID).ToList();
                    List<ChatMessageDto> sendlist = new List<ChatMessageDto>();
                    List<ChatMessageDto> recivelist = new List<ChatMessageDto>();
                    sendlist = mgdb.GetAll(o => o.UserID == userId
                            && ((ChatMessage)o.Data).SendTime < (endTime ?? DateTime.MaxValue)
                            && ((ChatMessage)o.Data).SendTime > (beginTime ?? DateTime.MinValue)
                            && (((ChatMessage)o.Data).IsRead == status || (status == EnterpriseData.Common.ReadStatus.All))
                            && uidlist.Contains(((ChatMessage)o.Data).SendUserID)).Select(o =>
                            {
                                var temp = (ChatMessage)o.Data;
                                return new ChatMessageDto
                                {
                                    SendUserID = temp.SendUserID,
                                    SendUserName = temp.SendUserName,
                                    SendUserPicture = temp.SendUserPicture,
                                    EnterpriseID = temp.EnterpriseID,
                                    Recipients = temp.Recipients,
                                    Message = temp.Message,
                                    MessageID = temp.MessageID,
                                    IsRead = temp.IsRead,
                                    SendTime = temp.SendTime.ToLocalTime(),
                                    Time = o.Time.ToLocalTime()
                                };
                            }).Distinct().ToList();
                    foreach (var chat in sendlist)
                    {
                        List<KeyValuePair<string, string>> userlist = new List<KeyValuePair<string, string>>();
                        var useridlist = mgdb.GetAll(x => ((ChatMessage)x.Data).MessageID == chat.MessageID)
                                .Select(n => n.UserID).Distinct().ToList();
                        foreach (var user in useridlist)
                        {
                            var u = db.FrontUsers.FirstOrDefault(x => x.UserID.Equals(user));
                            userlist.Add(new KeyValuePair<string, string>((u ?? new FrontUser { UserID = "" }).UserID, (u ?? new FrontUser { Name = "" }).Name));
                        }
                        chat.UserList = userlist;
                    }
                    //var msgidlist = templist.Select(x => x.MessageID).ToList();
                    //var useridlist = mgdb.GetAll(x => msgidlist.Contains(((ChatMessage)x.Data).MessageID))
                    //        .Select(n => ((ChatMessage)n.Data).SendUserID)
                    //        .ToList();
                    recivelist = mgdb.GetAll(o => ((ChatMessage)o.Data).SendUserID == userId
                            && ((ChatMessage)o.Data).SendTime < (endTime ?? DateTime.MaxValue)
                            && ((ChatMessage)o.Data).SendTime > (beginTime ?? DateTime.MinValue)
                            && (((ChatMessage)o.Data).IsRead == status || (status == EnterpriseData.Common.ReadStatus.All))
                            && uidlist.Contains(o.UserID)).Select(o =>
                            {
                                var temp = (ChatMessage)o.Data;
                                return new ChatMessageDto
                                {
                                    SendUserID = temp.SendUserID,
                                    SendUserName = temp.SendUserName,
                                    SendUserPicture = temp.SendUserPicture,
                                    EnterpriseID = temp.EnterpriseID,
                                    Recipients = temp.Recipients,
                                    Message = temp.Message,
                                    MessageID = temp.MessageID,
                                    IsRead = temp.IsRead,
                                    SendTime = temp.SendTime.ToLocalTime(),
                                    Time = o.Time.ToLocalTime()
                                };
                            }).Distinct().ToList();
                    foreach (var chat in recivelist)
                    {
                        List<KeyValuePair<string, string>> userlist = new List<KeyValuePair<string, string>>();
                        var useridlist = mgdb.GetAll(x => ((ChatMessage)x.Data).MessageID == chat.MessageID)
                                .Select(n => n.UserID).Distinct().ToList();
                        foreach (var user in useridlist)
                        {
                            var u = db.FrontUsers.FirstOrDefault(x => x.UserID.Equals(user));
                            userlist.Add(new KeyValuePair<string, string>((u ?? new FrontUser { UserID = "" }).UserID, (u ?? new FrontUser { Name = "" }).Name));
                        }
                        chat.UserList = userlist;
                    }
                    var templist = sendlist.Concat(recivelist);
                    //.Select(
                    //o => new ChatDto
                    //{
                    //    SendTime = ((ChatMessage)o.Data).SendTime.ToLocalTime(),
                    //    SenderName = ((ChatMessage)o.Data).SendUserID,
                    //    Message = ((ChatMessage)o.Data).Message,
                    //    IsRead = ((ChatMessage)o.Data).IsRead,
                    //    MessageID = ((ChatMessage)o.Data).MessageID
                    //}).Distinct();

                    var namelist = db.FrontUsers.Where(x => uidlist.Contains(x.UserID)).Select(x => new uidnameDto
                    {
                        UserId = x.UserID,
                        UserName = x.Name
                    }).ToList();
                    var list = from user in namelist
                               join temp in templist on user.UserId equals temp.SendUserID
                               select new
                               {
                                   SendTime = temp.SendTime,
                                   SenderName = user.UserName,
                                   IsRead = temp.IsRead,
                                   Message = temp.Message,
                                   MessageID = temp.MessageID,
                                   UserList = temp.UserList
                               };
                    //var list = templist.Where(x=>namelist.Any(n=>n.UserId.Equals(x.SenderName))).Select(m=>new ChatDto{
                    //SenderName = m.
                    //})
                    int pagecount;
                    amount = list.Count();
                    if (pageSize > 0)
                    {
                        // 获取总共页数
                        pagecount = (list.Count() + pageSize - 1) / pageSize;
                    }
                    else
                    {
                        pagecount = 0;
                    }
                    //页码判断，小于1则为1，大于最大页码则为最大页码
                    if (pageNumber > pagecount)
                        pageNumber = pagecount;
                    if (pageNumber < 1)
                        pageNumber = 1;

                    return list.Select(x => new ChatDto
                    {
                        SendTime = x.SendTime,
                        SenderName = x.SenderName,
                        IsRead = x.IsRead,
                        Message = x.Message,
                        MessageID = x.MessageID,
                        UserList = x.UserList
                    }).OrderByDescending(x => x.SendTime).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public bool ChangeReadStatus(Guid messageId, string userId)
        {
            try
            {
                var mgdb = new MongoDbProvider<UserMessageQueueItem>();
                var msg = mgdb.GetAll(x => x.UserID == userId && ((ChatMessage)x.Data).IsRead == ReadStatus.NoRead && ((ChatMessage)x.Data).MessageID == messageId);
                foreach (var chat in msg)
                {
                    ((ChatMessage)chat.Data).IsRead = EnterpriseData.Common.ReadStatus.Read;
                    mgdb.Update(chat);
                }
                return ((ChatMessage)msg.FirstOrDefault().Data).IsRead == EnterpriseData.Common.ReadStatus.Read;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int GetMessageCount(string userId)
        {
            try
            {
                var mgdb = new MongoDbProvider<UserMessageQueueItem>();
                var msg = mgdb.GetAll(x => x.UserID == userId && ((ChatMessage)x.Data).SendUserID != userId && ((ChatMessage)x.Data).IsRead == ReadStatus.NoRead);
                var count = msg.Select(x => ((ChatMessage)x.Data).MessageID).Distinct().Count();
                return count;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
