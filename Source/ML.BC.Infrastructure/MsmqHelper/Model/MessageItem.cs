using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Infrastructure.MsmqHelper
{
    public class MessageItem
    {
        public TypeEnum Type { get; set; }
        public string EntityName { get; set; }
        public OperationEnum Operation { get; set; }
        public List<CustomKeyValue> Data { get; set; }
        public DateTime ChangeTime { get; set; }
    }

    public class CustomKeyValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
