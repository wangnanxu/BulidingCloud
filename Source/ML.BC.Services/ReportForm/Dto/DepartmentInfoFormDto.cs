using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class DepartmentInfoFormDto
    {
        public string DepartmentName { get; set; }
        public int UsersCount { get; set; }
        public int ProjectCount { get; set; }
        public int SceneCount { get; set; }
        public int PictureCount { get; set; }
        public long PictureByte { get; set; }
        //public int DepartmentID { get; set; }
        //public int ParentID { get; set; }
    }
}
