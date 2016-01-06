using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class KnowlegeDto
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int KnowledgeType { get; set; }
        public EnterpriseData.Common.FileType DocumentType { get; set; }
        public string EnterpriseID { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool Deleted { get; set; }
        public double DocumentSize { get; set; }
        public Stream FileStream { get; set; }
        public System.Guid FileGUID { get; set; }
        public int FileNumber { get; set; }
        public double FileAllSize { get; set; }
    }
}
