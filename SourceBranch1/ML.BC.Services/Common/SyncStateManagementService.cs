using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model;
using ML.BC.EnterpriseData.Model.Extend;
using ML.BC.Infrastructure.Exceptions;

namespace ML.BC.Services
{
    public class SyncStateManagementService : ISyncStateManagementService
    {
        public int Add(SyncStateDto syncStateDto)
        {
            try
            {
                if (null == syncStateDto) throw new KnownException("对象为空，无法添加！");
                if (string.IsNullOrEmpty(syncStateDto.UserID) || string.IsNullOrEmpty(syncStateDto.DeviceID) || syncStateDto.ActionType < 0)
                    throw new KnownException("缺少必要信息，无法添加！");
                using (var db = new BCEnterpriseContext())
                {
                    syncStateDto.SyncTime = DateTime.Now;
                    db.SyncStates.Add(syncStateDto);
                    if (0 <= db.SaveChanges()) throw new KnownException("添加失败！");
                    return syncStateDto.SyncStateID;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public bool Update(SyncStateDto updateDto)
        {
            try
            {
                if (null == updateDto) throw new KnownException("对象为空，无法更新！");
                if (string.IsNullOrEmpty(updateDto.UserID) || string.IsNullOrEmpty(updateDto.DeviceID))
                    throw new KnownException("缺少必要信息，无法更新！");
                using (var db = new BCEnterpriseContext())
                {
                    var query = db.SyncStates.FirstOrDefault(obj => (obj.UserID == updateDto.UserID)
                                                                        && (obj.DeviceID == updateDto.DeviceID)
                                                                        && (obj.ActionType == updateDto.ActionType));
                    if (null == query) throw new KnownException("未能查询到相关记录，无法更新！");
                    if (query.SyncTime > updateDto.SyncTime) return false;
                    query.SyncTime = updateDto.SyncTime;
                    db.SyncStates.AddOrUpdate(query);
                    if (0 <= db.SaveChanges()) throw new KnownException("服务器更新失败！");
                    return true;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public SyncStateDto Search(SyncStateDto seachDto)
        {
            try
            {
                if (null == seachDto) throw new KnownException("对象为空，无法查询！");
                if (string.IsNullOrEmpty(seachDto.UserID) && string.IsNullOrEmpty(seachDto.DeviceID))
                    throw new KnownException("缺少必要信息，无法查询！");
                using (var db = new BCEnterpriseContext())
                {
                    var query = db.SyncStates.FirstOrDefault(obj => (obj.UserID == seachDto.UserID)
                                                                    && (obj.DeviceID == seachDto.DeviceID)
                                                                    && (obj.ActionType == seachDto.ActionType));
                    if (null == query) throw new KnownException("未能查询到相关记录！");
                    return query;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private const string SETSYNCSTATELOCK = "SETSYNCSTATELOCK";
        /// <summary>
        /// 设置同步记录,如果存在就更新
        /// </summary>
        /// <param name="syncStateDtos"></param>
        /// <returns></returns>
        public bool SetSyncState(params SyncStateDto[] syncStateDtos)
        {
            lock (SETSYNCSTATELOCK)
            {
                try
                {
                    if (null == syncStateDtos || syncStateDtos.Length == 0) throw new ArgumentNullException("syncStateDtos");
                    for (int i = 0; i < syncStateDtos.Length; i++)
                    {
                        if (string.IsNullOrEmpty(syncStateDtos[i].UserID) || string.IsNullOrEmpty(syncStateDtos[i].DeviceID) || syncStateDtos[i].ActionType < 0)
                            throw new KnownException("对象索引:" + i + " 缺少必要信息，无法添加或更新！");
                    }

                    using (var db = new BCEnterpriseContext())
                    {
                        syncStateDtos.ToList().ForEach(m => m.SyncTime = DBTimeHelper.DBNowTime(db));
                        var objs = syncStateDtos.Select(n => new SyncState
                        {
                            SyncStateID = n.SyncStateID,
                            UserID = n.UserID,
                            DeviceID = n.DeviceID,
                            ActionType = n.ActionType,
                            SyncTime = n.SyncTime
                        }).ToList();

                        List<string> userids = objs.Select((n) => n.UserID).ToList();
                        List<string> deviceIDs = objs.Select((n) => n.DeviceID).ToList();
                        List<byte> actionTypes = objs.Select((n) => n.ActionType).ToList();
                        var oldlist = db.SyncStates.Where(m => userids.Contains(m.UserID) && deviceIDs.Contains(m.DeviceID) && actionTypes.Contains(m.ActionType));
                        oldlist.ToList().ForEach(n =>
                        {
                            db.SyncStates.Remove(n);
                        });

                        objs.ForEach(n =>
                        {
                            db.SyncStates.Add(n);
                        });
                        db.SaveChanges();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
