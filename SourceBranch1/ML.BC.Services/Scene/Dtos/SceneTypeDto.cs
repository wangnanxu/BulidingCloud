using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class SceneTypeDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentID { get; set; }
        public bool Available { get; set; }
        public string EnterpriseID { get; set; }
    }
}
