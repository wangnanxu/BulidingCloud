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
    
    public partial class Scene
    {
        public string SceneID { get; set; }
        public string Name { get; set; }
        public string EnterpriseID { get; set; }
        public string ProjectID { get; set; }
        public string ParentSceneID { get; set; }
        public string Woker { get; set; }
        public System.DateTime RegistDate { get; set; }
        public Nullable<System.DateTime> BeginDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public byte Status { get; set; }
        public bool Deleted { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string LatitudeAndLongitude { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public bool HasData { get; set; }
    }
}
