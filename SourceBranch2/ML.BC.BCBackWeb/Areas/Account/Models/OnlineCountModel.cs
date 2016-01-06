using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Areas.Account.Models
{
    public class OnlineCountModel
    {
        public long UserLoginStateID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string LoginIP { get; set; }
        public string Device { get; set; }
        public string LoginToken { get; set; }
        public string LoginTime { get; set; }
    }
    public class OnlineUserResult
    {
        public int total { get; set; }
        public List<OnlineCountModel> rows { get; set; }
    }
}