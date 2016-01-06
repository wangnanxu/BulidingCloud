using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ML.BC.EnterpriseData.Common;
using ML.BC.EnterpriseData.Model;

namespace ML.BC.Services
{
    public class ChatMessageDto
    {
        public string SendUserID { get; set; }
        public string SendUserName { get; set; }
        public string SendUserPicture { get; set; }
        public string EnterpriseID { get; set; }
        public string Recipients { get; set; }
        public string Message { get; set; }
        public DateTime SendTime { get; set; }
        public int[] DepartMentIDs { get; set; }
        public Guid MessageID { get; set; }
        public ReadStatus IsRead { get; set; }
        public DateTime Time { get; set; }

        public static implicit operator ChatMessageDto(ChatMessage obj)
        {
            return new ChatMessageDto()
            {
                SendUserID = obj.SendUserID,
                SendUserName = obj.SendUserName,
                SendUserPicture = obj.SendUserPicture,
                EnterpriseID = obj.EnterpriseID,
                Recipients = obj.Recipients,
                Message = obj.Message,
                MessageID = obj.MessageID,
                IsRead = obj.IsRead,
                SendTime = obj.SendTime

            };
        }

        public override bool Equals(object obj)
        {
            var temp = obj as ChatMessageDto;
            if (temp == null) return false;
            return this.MessageID == temp.MessageID;

        }

        public override int GetHashCode()
        {
            return this.MessageID.ToString().GetHashCode();
        }
    }
    public class ChatDto
    {
        public string SenderName { get; set; }
        public DateTime SendTime { get; set; }
        public string Message { get; set; }
        public ReadStatus IsRead { get; set; }
        public Guid MessageID { get; set; }
    }
    public class uidnameDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
