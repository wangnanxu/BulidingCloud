//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ML.BC.EnterpriseData.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserLoginLog
    {
        public int UserLoginLogID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string IP { get; set; }
        public System.DateTime Time { get; set; }
        public string Device { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
    }
}
