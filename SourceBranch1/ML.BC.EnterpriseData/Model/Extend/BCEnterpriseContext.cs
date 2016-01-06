using ML.BC.EnterpriseData.Model.Extend;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.EnterpriseData.Model
{
    public partial class BCEnterpriseContext
    {
        public override int SaveChanges()
        {
            List<ML.BC.Infrastructure.MsmqHelper.MessageItem> changeList = null;

            if (ChangeTracker.HasChanges())
            {
                changeList = new List<Infrastructure.MsmqHelper.MessageItem>();

                var list = ChangeTracker.Entries()
                    .Where(n => n.State == EntityState.Added || n.State == EntityState.Modified || n.State == EntityState.Deleted)
                    .ToList();
                list.ForEach(entry =>
                {
                    SetUpdateTime(entry);
                    if (entry.Entity is IEntityHooks)
                    {
                        var temp = ((IEntityHooks)entry.Entity).GetChangedEntryData(entry);
                        if(temp!=null)
                        {
                            changeList.Add(temp);
                        }
                    }
                });
            }

            var result = base.SaveChanges();

            if (result != 0 && changeList != null && changeList.Any())
            {
                using (var msmqProvider = ML.BC.Infrastructure.Ioc.GetService<ML.BC.Infrastructure.MsmqHelper.IMsmqProvider>())
                {
                    msmqProvider.Send<ML.BC.Infrastructure.MsmqHelper.MessageItem>(new System.Messaging.Message(changeList));
                }
            }
            return result;
        }

        private void SetUpdateTime(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            if (entry.State != EntityState.Added && entry.State != EntityState.Modified) return;

            var entity = entry.Entity;
            Type type = entity.GetType();
            var pi = type.GetProperty("UpdateTime");
            if (pi != null)
            {
                pi.SetValue(entity, DBTimeHelper.DBNowTime(this));
            }
        }

        public int ExcuteNonQuerySql(IEnumerable<string> cacheSets, string commandText, params object[] parameters )
        {
            var connection = this.Database.Connection;
            try
            {
                //open the connection for use
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                //create a command object
                using (var cmd = connection.CreateCommand())
                {
                    //command to execute
                    cmd.CommandText = commandText;
                    cmd.CommandType = CommandType.Text;

                    // move parameters to command object
                    if (parameters != null)
                        foreach (var p in parameters)
                            cmd.Parameters.Add(p);

                    //database call
                    return cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

                if (cacheSets != null)
                {
                    //EFCacheConfiguration.InvalidateSetsCache(cacheSets);
                }
            }
        }
    }
}
