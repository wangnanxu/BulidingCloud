using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ML.BC.Web.Framework.Schedule
{
    public class Schedule : IDisposable
    {
        private log4net.ILog _ilog = log4net.LogManager.GetLogger("LOG");
        private static Schedule _instance = null;
        private Timer _Timer = null;
        private Schedule()
        {
            _Timer = new Timer(1000);
            _Timer.Elapsed += _Timer_Elapsed;
        }

        private void _Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                SyncEFCache();
#if DEBUG
                ProcessAppSyncMessage();
#endif
            }
            catch { }
        }

        private void ProcessAppSyncMessage()
        {
            try
            {
                List<ML.BC.Infrastructure.MsmqHelper.MessageItem> messageItems = null;
                using (var msmqProvider = ML.BC.Infrastructure.Ioc.GetService<ML.BC.Infrastructure.MsmqHelper.IMsmqProvider>())
                {
                    messageItems = msmqProvider.Receive<List<ML.BC.Infrastructure.MsmqHelper.MessageItem>>();
                }

                if (messageItems.Count != 0)
                {
                    ML.BC.Web.Framework.Schedule.MsmqSync.DistributeMessage(messageItems);
                }
            }
            catch (Exception ex)
            {
                _ilog.Error("ProcessAppSyncMessage throw exception", ex);
            }
        }

        private void SyncEFCache()
        {
        }

        public static Schedule Instance
        {
            get
            {
                return _instance ?? new Schedule();
            }
        }
        public void StartTimer()
        {
            _Timer.Start();
        }
        public void Dispose()
        {
            if (_Timer != null)
            {
                if (_Timer.Enabled == true)
                    _Timer.Stop();
                _Timer.Close();
                _Timer.Dispose();
            }
        }
    }
}
