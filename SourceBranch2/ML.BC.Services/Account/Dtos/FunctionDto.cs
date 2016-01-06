using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services.Account.Dtos
{
    public class FunctionDto
    {
        public string FunctionID { get; set; }
        public string Name { get; set; }
        public string MyID { get; set; }
        public string ParentID { get; set; }
        public string Desription { get; set; }
        public bool Available { get; set; }
    }
}
