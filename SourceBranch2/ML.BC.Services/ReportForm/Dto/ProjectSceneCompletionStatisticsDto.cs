using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class ProjectSceneCompletionStatisticsDto
    {
        public CompletionStatistics ProjectData { get;set;}
        public CompletionStatistics SceneData { get; set; }
    }
    public class CompletionStatistics
    {
        public int EndCount { get; set; }
        public int IngCount { get; set; }
        public int ReadyCount { get; set; }
    }
}
