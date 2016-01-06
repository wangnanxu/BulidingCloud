using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Messaging;
using log4net;
using log4net.Config;
using ML.BC.Infrastructure.Log;

namespace ML.BC.Infrastructure.MsmqHelper
{
    public class MsmqProvider : IMsmqProvider, IDisposable
    {
        /// <summary>
        /// 队列对象
        /// </summary>
        private MessageQueue _queue;
        /// <summary>
        /// 消息队列配置信息
        /// </summary>
        private readonly MsmqQueueConfig _msmqQueueConfig;
        /// <summary>
        /// 超时时间，默认500毫秒
        /// </summary>
        private readonly TimeSpan _timeSpan = new TimeSpan(0, 0, 0, 0, 500);
        #region 带参数的构造函数会导致Ioc出错

        ///// <summary>
        ///// 构造函数
        ///// </summary>
        ///// <param name="path">队列路径名称，例如：@".\private$\myQueue"</param>
        ///// <param name="isLocal"></param>
        //public MsmqProvider(string path, bool isLocal)
        //{
        //    var logger = new Log4NetProvider(typeof(MsmqProvider));
        //    logger.Info("Log测试");
        //    _msmqQueueConfig = new MsmqQueueConfig { QueuePathName = path, IsLocal = isLocal };
        //    _queue = GetMessageQueue();
        //
        //}

        #endregion

        public MsmqProvider()
        {
            var con = ConfigurationManager.ConnectionStrings["MsmqPath"].ConnectionString;
            _msmqQueueConfig = new MsmqQueueConfig
            {
                QueuePathName = con.Split('/').First(),
                IsLocal = con.Split('/').Last() == "true"
            };
            _queue = GetMessageQueue();
        }
        /// <summary>
        /// 创建本地队列
        /// </summary>
        private void CreateQueue()
        {
            try
            {
                if (_msmqQueueConfig.IsLocal && !IsExistQueue(_msmqQueueConfig.QueuePathName))
                {
                    MessageQueue.Create(_msmqQueueConfig.QueuePathName);
                }
            }
            catch (Exception ex)
            {

                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
            }


        }
        /// <summary>
        /// 队列中消息的个数
        /// </summary>
        public virtual int Count()
        {
            try
            {
                if (_queue == null)
                {
                    _queue = new MessageQueue(_msmqQueueConfig.QueuePathName);
                }
                var messages = _queue.GetAllMessages();
                return messages.Length;
            }
            catch (MessageQueueException ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
                return 0;
            }
        }
        /// <summary>
        /// 获取一个队列对象 
        /// </summary>
        /// <returns>MessageQueue</returns>
        private MessageQueue GetMessageQueue()
        {
            try
            {
                CreateQueue();
                var queue = new MessageQueue(_msmqQueueConfig.QueuePathName)
                {
                    DefaultPropertiesToSend =
                    {
                        Recoverable = true,                          //  计算机故障是否保证传递消息
                        AttachSenderId = false,                      //  发送方ID是否附在消息中
                        UseAuthentication = false,                   //  发送前是否必须验证消息
                        UseEncryption = false,                       //  是否消息成为私有的
                        AcknowledgeType = AcknowledgeTypes.None,     //  确认消息类型
                        UseJournalQueue = false                      //  是否在计算机日记中保留消息副本
                    }
                };
                return queue;
            }
            catch (MessageQueueException ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
                return null;
            }

        }
        /// <summary>
        /// 判断队列是否已经存在
        /// </summary>
        /// <returns></returns>
        public virtual bool IsExistQueue(string queuePathName)
        {
            try
            {
                return MessageQueue.Exists(queuePathName);
            }
            catch (MessageQueueException ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">消息对象</param>
        public virtual void Send(Message msg)
        {
            try
            {
                _queue.Formatter = new XmlMessageFormatter(new[] { "System.String" });
                _queue.Send(msg);
            }
            catch (MessageQueueException ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
            }
        }

        public virtual void Send<T>(Message msg)
        {
            try
            {
                _queue.Formatter = new XmlMessageFormatter(new[] { typeof(T) });
                _queue.Send(msg);
            }
            catch (MessageQueueException ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
            }
        }
        /// <summary>
        /// 接收消息方法
        /// </summary>
        public virtual Message Receive()
        {
            try
            {
                using (Message message = _queue.Receive(_timeSpan))
                {
                    return message;
                }
            }
            catch (MessageQueueException ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
                return null;
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        public virtual T Receive<T>(IMessageFormatter formatter) where T : new()
        {
            try
            {
                _queue.Formatter = formatter;
                using (Message msg = _queue.Receive(_timeSpan))
                {
                    if (null != msg)
                        return (T)msg.Body;
                    return new T();
                }
            }
            catch (MessageQueueException ex)
            {
                //var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                //logger.Error(ex.Message);
                return new T();
            }
            catch (InvalidOperationException ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
                throw new InvalidOperationException(ex.Message);
            }
        }

        public virtual T Receive<T>() where T : new()
        {
            return Receive<T>(new XmlMessageFormatter(new[] { typeof(T) }));
        }
        public virtual Message ReceiveByID(string id, TimeSpan time)
        {
            try
            {
                using (Message message = _queue.ReceiveById(id, _timeSpan))
                {
                    return message;
                }
            }
            catch (MessageQueueException)
            {
                //if (ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                //    throw new TimeoutException();

                return null;
            }
            catch (InvalidOperationException ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 以指定格式获取所有消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter">格式：XML，Binary</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAllMessages<T>(IMessageFormatter formatter)
        {
            try
            {
                _queue.Formatter = formatter;
                Message[] messages = _queue.GetAllMessages();
                if (messages.Length > 0)
                {
                    return messages.Select(x => (T)x.Body);
                }
                return new List<T>();
            }
            catch (MessageQueueException ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Xml格式获取所有消息
        /// </summary>
        /// <typeparam name="T">消息结构类</typeparam>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAllMessagesByXml<T>()
        {
            return GetAllMessages<T>(new XmlMessageFormatter(new[] { typeof(T) }));
        }
        /// <summary>
        /// 清空队列中的消息
        /// </summary>
        public virtual void ClearMessage()
        {
            _queue.Purge();
        }
        /// <summary>
        /// 删除队列
        /// </summary>
        /// <param name="queuePathName"></param>
        public virtual void DeleteQueue(string queuePathName)
        {
            try
            {
                if (IsExistQueue(queuePathName))
                {
                    //  队列不存在
                }
                else
                {
                    MessageQueue.Delete(queuePathName);
                }
            }
            catch (MessageQueueException ex)
            {
                var logger = log4net.LogManager.GetLogger(typeof(MsmqProvider));
                logger.Error(ex.Message);
            }
        }

        #region IDisposable 成员
        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            _queue.Dispose();
        }

        #endregion
    }
}
