//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ML.BC.EnterpriseData.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserLoginState
    {
        public long UserLoginStateID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string LoginIP { get; set; }
        public string Device { get; set; }
        public string LoginToken { get; set; }
        public Nullable<System.DateTime> LoginTime { get; set; }
        public System.DateTime UpdateTime { get; set; }
    }
}
