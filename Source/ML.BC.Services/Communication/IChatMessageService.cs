using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model;

namespace ML.BC.Services
{
    public interface IChatMessageService
    {
        bool ProcessChatMessage(ChatMessageDto msgDto);
        bool ChangeReadStatus(Guid messageId,string userId);
        List<ChatMessageDto> GetChatMessages(string userID, string deviceID, int pageSize, DateTime queryTime,out int count);
        List<ChatDto> SearchChatMessages(string userId, DateTime? beginTime, DateTime? endTime, string objectiveName, ML.BC.EnterpriseData.Common.ReadStatus status, int pageSize, int pageNumber, out int amount);
        int GetMessageCount(string userId);
    }
}
