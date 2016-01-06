using ML.BC.EnterpriseData.Common;
using ML.BC.EnterpriseData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class ProjectStatisDto
    {
        public string projectId;
        public string projectName;
        public int[] typeCount = new int[7];
    }

}
