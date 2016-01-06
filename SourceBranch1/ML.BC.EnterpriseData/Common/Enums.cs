using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.EnterpriseData.Common
{
    public enum UserStatusEnum
    {
        Disabled = 0,
        Enabled = 1,
    }
    public enum Status
    {
        Ready = 1,
        Begin = 2,
        End = 3,
        All = 4,
    }

    public enum EnterpriseState
    {
        [Description("正常")]
        Normal = 1,
        [Description("暂停")]
        Paused = 2,
        [Description("关闭")]
        Closed = 4
    }

    public enum ReadStatus
    {
        /// <summary>
        /// 已读
        /// </summary>
        Read = 1,
        /// <summary>
        /// 未读
        /// </summary>
        NoRead = 2,
        /// <summary>
        ///为空，获取全部 
        /// </summary>
        All = 4
    }
    public enum LoginStatus
    {
        /// <summary>
        /// 登录
        /// </summary>
        Login = 1,
        /// <summary>
        /// 登出
        /// </summary>
        Logout = 2,
        /// <summary>
        /// 登录超时
        /// </summary>
        TimeOut =3,
        
        /// <summary>
        /// 同时获取登录登出数据
        /// </summary>
        GetAll = 4
    }
}
