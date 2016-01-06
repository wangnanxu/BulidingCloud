using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ML.BC.EnterpriseWeb.Areas.Account.Models
{
    public class MessageModel
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public string SendTime { get; set; }
        public int Status { get; set; }
        public Guid MessageID { get; set; }
    }
    public class MessageResult
    {
        public int total { get; set; }
        public List<MessageModel> rows { get; set; }
    }
    public class MessageSearchModel
    {
        public string Sender { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Status { get; set; }
        public Guid MessageID { get; set; }
    }
}