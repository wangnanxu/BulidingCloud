using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model;

namespace ML.BC.Services
{
    public class SyncStateDto
    {
        public int SyncStateID { get; set; }
        public byte ActionType { get; set; }
        public DateTime SyncTime { get; set; }
        public string UserID { get; set; }
        public string DeviceID { get; set; }

        public static implicit operator SyncState(SyncStateDto dto)
        {
            return new SyncState {
                SyncStateID = dto.SyncStateID,
                UserID = dto.UserID,
                DeviceID = dto.DeviceID,
                ActionType = dto.ActionType,
                SyncTime = dto.SyncTime
            };
        }
      
        public static implicit operator SyncStateDto(SyncState obj)
        {
            return new SyncStateDto {
                SyncStateID = obj.SyncStateID,
                UserID = obj.UserID,
                DeviceID = obj.DeviceID,
                ActionType = obj.ActionType,
                SyncTime = obj.SyncTime
            };
        }
    }
}
