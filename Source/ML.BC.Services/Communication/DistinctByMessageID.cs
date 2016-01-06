using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class DistinctByMessageID:IEqualityComparer<ChatDto>
    {
        public bool Equals(ChatDto x, ChatDto y)
        {
            return x.MessageID == y.MessageID;
        }

        public int GetHashCode(ChatDto obj)
        {
            return obj.MessageID.ToString().GetHashCode();
        }
    }
}
