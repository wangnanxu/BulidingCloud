using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Sql;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
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
                var query = mgdb.GetAll(o => o.UserID == userID && o.Time < queryTime && ((ChatMessage) o.Data).Message != null)
                        .Select(o =>
                        {
                            var temp = (ChatMessage) o.Data;
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
        public List<ChatDto> SearchChatMessages(string userId, DateTime? beginTime, DateTime? endTime, string senderName, EnterpriseData.Common.ReadStatus status, int pageSize, int pageNumber, out int amount)
        {
            try
            {
                if (string.IsNullOrEmpty(senderName))
                    senderName = "";
                //if (beginTime > endTime)
                //    throw new KnownException("起止时间不合法");
                using (var db = new BCEnterpriseContext())
                {
                    var mgdb = new MongoDbProvider<UserMessageQueueItem>();
                    var uidlist = db.FrontUsers.Where(x => x.Name.Contains(senderName)).Select(n => n.UserID).ToList();
                    var templist = mgdb.GetAll(o => o.UserID == userId
                            && ((ChatMessage)o.Data).SendTime < (endTime ?? DateTime.MaxValue)
                            && ((ChatMessage)o.Data).SendTime > (beginTime ?? DateTime.MinValue)
                            && (((ChatMessage)o.Data).IsRead == status || (status == EnterpriseData.Common.ReadStatus.All))
                            && uidlist.Contains(((ChatMessage)o.Data).SendUserID))
                            .Select(o => new ChatDto
                            {
                                SendTime = ((ChatMessage)o.Data).SendTime.ToLocalTime(),
                                SenderName = ((ChatMessage)o.Data).SendUserID,
                                Message = ((ChatMessage)o.Data).Message,
                                IsRead = ((ChatMessage)o.Data).IsRead,
                                MessageID = ((ChatMessage)o.Data).MessageID
                            }).ToList();
                    var namelist = db.FrontUsers.Where(x => uidlist.Contains(x.UserID)).Select(x => new uidnameDto
                    {
                        UserId = x.UserID,
                        UserName = x.Name
                    }).ToList();
                    var list = from user in namelist
                               join temp in templist on user.UserId equals temp.SenderName
                               select new
                               {
                                   SendTime = temp.SendTime,
                                   SenderName = user.UserName,
                                   IsRead = temp.IsRead,
                                   Message = temp.Message,
                                   MessageID = temp.MessageID
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
                        MessageID = x.MessageID
                    }).OrderBy(x => x.MessageID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public bool ChangeReadStatus(Guid messageId,string userId)
        {
            try
            {
                var mgdb = new MongoDbProvider<UserMessageQueueItem>();
                var msg = mgdb.GetByCondition(x =>x.UserID == userId &&((ChatMessage)x.Data).MessageID == messageId);
                ((ChatMessage)msg.Data).IsRead = EnterpriseData.Common.ReadStatus.Read;
                var flag = mgdb.Update(msg);
                return ((ChatMessage)flag.Data).IsRead == EnterpriseData.Common.ReadStatus.Read;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
