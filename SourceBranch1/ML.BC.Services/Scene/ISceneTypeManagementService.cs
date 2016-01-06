using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface ISceneTypeManagementService : IServiceBase
    {
        int AddSceneType(SceneTypeDto sceneType);
        bool DeleteSceneType(int sceneTypeId);
        bool UpdateSceneType(SceneTypeDto sceneType);
        List<SceneTypeDto> GetAllSceneTypeList(string enterpriseId);
    }
}
