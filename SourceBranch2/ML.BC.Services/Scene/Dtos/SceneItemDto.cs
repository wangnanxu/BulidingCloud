using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model;

namespace ML.BC.Services
{
    public class SceneItemDto : SceneItem
    {
        public string UserName { get; set; }
        public string UserPicture { get; set; }
    }
}
