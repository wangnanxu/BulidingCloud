using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Infrastructure.Log
{
    public interface ILog4NetProvider
    {
        /// <summary>
        /// _logger.Info
        /// </summary>
        /// <param name="msg">需要记录的信息</param>
        bool Info(string msg);

        /// <summary>
        /// _logger.Error
        /// </summary>
        /// <param name="msg">需要记录的信息</param>
        bool Error(string msg);

        /// <summary>
        /// _logger.Debug
        /// </summary>
        /// <param name="msg">需要记录的信息</param>
        bool Debug(string msg);
    }
}
