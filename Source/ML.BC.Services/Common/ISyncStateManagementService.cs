using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface ISyncStateManagementService : IServiceBase
    {
        int Add(SyncStateDto syncStateDto);
        bool Update(SyncStateDto updateDto);
        SyncStateDto Search(SyncStateDto seachDto);

        /// <summary>
        /// 设置同步记录,如果存在就更新
        /// </summary>
        /// <param name="syncStateDto"></param>
        /// <returns></returns>
        bool SetSyncState(params SyncStateDto[] syncStateDtos);
    }
}
