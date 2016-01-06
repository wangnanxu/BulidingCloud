using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Infrastructure.MsmqHelper
{
    public interface IMsmqProvider : IDisposable
    {
        /// <summary>
        /// 队列中消息的个数
        /// </summary>
        int Count();

        /// <summary>
        /// 判断队列是否已经存在
        /// </summary>
        /// <returns></returns>
        bool IsExistQueue(string queuePathName);

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">消息对象</param>
        void Send(Message msg);

        void Send<T>(Message msg);
        /// <summary>
        /// 接收消息方法
        /// </summary>
        Message Receive();

        T Receive<T>(IMessageFormatter formatter) where T : new();
        T Receive<T>() where T : new();

        Message ReceiveByID(string id, TimeSpan time);
        /// <summary>
        /// 以指定格式获取所有消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatter">格式：XML，Binary</param>
        /// <returns></returns>
        IEnumerable<T> GetAllMessages<T>(IMessageFormatter formatter);

        /// <summary>
        /// Xml格式获取所有消息
        /// </summary>
        /// <typeparam name="T">消息结构类</typeparam>
        /// <returns></returns>
        IEnumerable<T> GetAllMessagesByXml<T>();

        /// <summary>
        /// 清空队列中的消息
        /// </summary>
        void ClearMessage();

        /// <summary>
        /// 删除队列
        /// </summary>
        /// <param name="queuePathName"></param>
        void DeleteQueue(string queuePathName);


        /// <summary>
        /// 释放资源
        /// </summary>
        new void Dispose();
    }
}
