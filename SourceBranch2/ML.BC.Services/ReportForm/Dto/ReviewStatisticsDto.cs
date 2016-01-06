using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class ReviewStatisticsDto
    {
        public int ProjectAll { get; set; }
        public int ProjectScan { get; set; }
        public int SceneAll { get; set; }
        public int SceneScan { get; set; }
    }
}
