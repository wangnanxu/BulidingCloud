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
    
    public partial class SyncState
    {
        public int SyncStateID { get; set; }
        public byte ActionType { get; set; }
        public System.DateTime SyncTime { get; set; }
        public string UserID { get; set; }
        public string DeviceID { get; set; }
    }
}
