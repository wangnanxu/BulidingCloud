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
    
    public partial class KnowledgeBase
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int KnowledgeType { get; set; }
        public int DocumentType { get; set; }
        public string EnterpriseID { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public bool Deleted { get; set; }
        public double DocumentSize { get; set; }
        public System.Guid FileGUID { get; set; }
        public int FileNumber { get; set; }
        public double FileAllSize { get; set; }
    }
}
