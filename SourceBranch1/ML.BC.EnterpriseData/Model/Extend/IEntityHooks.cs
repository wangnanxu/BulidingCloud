using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.EnterpriseData.Model
{
    public interface IEntityHooks
    {
        //void OnAddedHook(DbEntityEntry entry);
        //void OnModifiedHook(DbEntityEntry entry);
        //void OnDeletedHook(DbEntityEntry entry);
        Infrastructure.MsmqHelper.MessageItem GetChangedEntryData(DbEntityEntry entry);
    }
}
