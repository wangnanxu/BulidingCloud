using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.EnterpriseData.Model.Extend
{
    public class DBTimeHelper
    {
        public static DateTime DBNowTime()
        {
            using (var db = new BCEnterpriseContext())
            {
                return DBNowTime(db);
            }
        }
        public static DateTime DBNowTime(BCEnterpriseContext db)
        {
            try
            {
                var cmd = db.Database.Connection.CreateCommand();
                cmd.CommandText = "select getdate()";
                db.Database.Connection.Open();
                var dbNow = (DateTime)cmd.ExecuteScalar();
                dbNow = dbNow.AddSeconds(1);
                return dbNow;
            }
            finally 
            {
                if (db.Database.Connection.State == System.Data.ConnectionState.Open)
                {
                    db.Database.Connection.Close();
                }                
            }
        }
    }
}
