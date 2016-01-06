using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Common;

namespace ML.BC.Services
{
    public class ScanDto
    {
        public string UserID { get; set; }
        public string ObjectID { get; set; }
        public ScanType Type { get; set; }
        public System.DateTime Time { get; set; }
    }
    public class ScanReportDto
    {
        public int ProjectAll { get; set; }
        public int ProjectScan { get; set; }
        public int SceneAll { get; set; }
        public int SceneScan { get; set; }
    }
}
