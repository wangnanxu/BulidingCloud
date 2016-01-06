using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ML.BC.EnterpriseWeb.Areas.Unit.Models;
namespace ML.BC.EnterpriseWeb.Areas.Account.Models
{
    public class MessageModel
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public string SendTime { get; set; }
        public int Status { get; set; }
        public List<string> UserList { get; set; }
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
    public class UserTree
    {
        public string id { get; set; }
        public string text { get; set; }
        public int type { get; set; } //0是部门 1是用户
        public string iconCls { get; set; }
        public List<UserTree> children { get; set; }
       
    }
    public class UserTreeResult
    {
        public int total { get; set; }
        public List<UserTree> rows { get; set; }
    }
}