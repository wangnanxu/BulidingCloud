using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class PersonStatisDto
    {
        public string userName { get; set; }
        public int projCount { get; set; }
        public int sceneCount { get; set; }
        public int imageCount { get; set; }
        public long imageSize { get; set; }//byte
    }
}
