using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Infrastructure.MsmqHelper
{
    /// <summary>
    /// 消息队列配置
    /// </summary>
    public class MsmqQueueConfig
    {
        /// <summary>
        /// 队列Id
        /// </summary>
        public int QueueId { get; set; }

        /// <summary>
        /// 队列路径名称，例如：@".\private$\myQueue"
        /// </summary>
        public String QueuePathName { get; set; }

        /// <summary>
        /// 队列描述
        /// </summary>
        public String QueueDescription { get; set; }

        /// <summary>
        /// 是否本地配置
        /// </summary>
        public bool IsLocal { get; set; }
    }
}
