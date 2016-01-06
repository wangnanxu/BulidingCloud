using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
namespace ML.BC.Infrastructure.Log
{
    public class Log4NetProvider : ILog4NetProvider
    {
        private static ILog _logger ;
        /// <summary>
        /// 指定loggerName构造方法
        /// </summary>
        /// <param name="loggerName">自定义logger名称</param>
        public Log4NetProvider(string loggerName)
        {
            if (string.IsNullOrEmpty(loggerName))
            {
                //  输出字符串为空，logger获取失败
            }
            else
            {
                try
                {
                    _logger = LogManager.GetLogger(loggerName);
                }
                catch (Exception ex)
                {                   
                    throw   new Exception(ex.Message);
                }
                
            }      
        }
        /// <summary>
        /// 指定loggerName构造方法
        /// </summary>
        /// <param name="loggerName">Type类型名称</param>
        public Log4NetProvider(Type loggerName)
        {
            if (string.IsNullOrEmpty(loggerName.ToString()))
            {
                //  输出字符串为空，logger获取失败
            }
            else
            {
                try
                {
                    _logger = LogManager.GetLogger(loggerName);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// _logger.Info
        /// </summary>
        /// <param name="msg">需要记录的信息</param>
        public virtual bool Info(string msg)
        {
            if (!string.IsNullOrEmpty(msg) && _logger.IsInfoEnabled)
            {
                _logger.Info(msg);
                return true;
            }
            else
            {
                //  输出字符串为空或logger级别错误
                return false;
            }
        }
        /// <summary>
        /// _logger.Error
        /// </summary>
        /// <param name="msg">需要记录的信息</param>
        public virtual bool Error(string msg)
        {
            if (!string.IsNullOrEmpty(msg)&&_logger.IsErrorEnabled)
            {
                _logger.Error(msg);
                return true;
            }
            else
            {
                //  输出字符串为空或logger级别错误
                return false;
            }
        }
        /// <summary>
        /// _logger.Debug
        /// </summary>
        /// <param name="msg">需要记录的信息</param>
        public virtual bool Debug(string msg)
        {
            if (!string.IsNullOrEmpty(msg) && _logger.IsDebugEnabled)
            {
                _logger.Debug(msg);
                return true;
            }
            else
            {
                //  输出字符串为空或logger级别错误
                return false;
            }
        }
    }
}
