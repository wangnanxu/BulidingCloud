using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services.ReportForm.Dto
{
    public class UsersInfoFormDto
    {
        public string UserName { get; set; }
        public int ProjectCount { get; set; }
        public int SceneCount { get; set; }
        public int PictureCount { get; set; }
    }
}
