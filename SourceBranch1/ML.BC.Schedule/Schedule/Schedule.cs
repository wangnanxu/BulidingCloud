using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ML.BC.Schedule
{
    public class Schedule : IDisposable
    {
        private log4net.ILog _ilog = log4net.LogManager.GetLogger("LOG");
        private static Schedule _instance = null;
        private Timer _Timer = null;
        private string _erro;
        private Schedule()
        {
            _Timer = new Timer(1000);
            _Timer.Elapsed += _Timer_Elapsed;
        }
        private void _Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if(null != Timer_Elapsed_Begin)                   
                    Timer_Elapsed_Begin(new TimerEventArgs { Message = "定时器触发,开始处理逻辑.." + DateTime.Now});

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
                //recorde log
                _erro = ex.Message;
                _ilog.Info(ex);
            }
            finally
            {
                if (string.IsNullOrEmpty(_erro))
                    _erro = "处理逻辑结束...";
                    if (null == _instance)
                        _instance = new Schedule();
                    Timer_Elapsed_End(new TimerEventArgs { Message = _erro + DateTime.Now });
                _erro = "";
            }
        }

        public static Schedule Instance
        {
            get
            {
                return _instance == null ? _instance = new Schedule() : _instance;
            }
        }
        public void pauseTimer()
        {
            _ilog.Info("暂停..");
            _Timer.Stop();
        }
        public void StartTimer()
        {
            if (_Timer != null)
                _ilog.Info("恢复..");
            else
            {
                _ilog.Info("启动..");
            }
            Instance._Timer.Start();
        }
        public void Dispose()
        {
            _ilog.Info("停止..");
            if (_Timer != null)
            {
                if (_Timer.Enabled == true)
                    _Timer.Stop();
                _Timer.Close();
                _Timer.Dispose();                
            }
            _instance = null;
        }

        public event TimerHandler Timer_Elapsed_Begin;
        public event TimerHandler Timer_Elapsed_End;
    }
    public class TimerEventArgs
    {
        public string Message { get; set; }
    }
    public delegate void TimerHandler(TimerEventArgs e);

}
