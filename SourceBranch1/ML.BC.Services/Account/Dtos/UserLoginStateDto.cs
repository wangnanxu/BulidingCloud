using ML.BC.EnterpriseData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.Infrastructure;

namespace ML.BC.Services.Account.Dtos
{
    /// <summary>
    /// 用户登录状态
    /// </summary>
    public  class UserLoginStateDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long UserLoginStateID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string LoginIP { get; set; }
        /// <summary>
        /// 设备
        /// </summary>
        public string Device { get; set; }
        /// <summary>
        /// 令牌
        /// </summary>
        public string LoginToken { get; set; }
        public Nullable<System.DateTime> LoginTime { get; set; }
        static public implicit operator UserLoginStateDto(ML.BC.EnterpriseData.Model.UserLoginState obj)
        {
            return new UserLoginStateDto {
                UserID = obj.UserID,Device=obj.Device, LoginIP=obj.LoginIP, LoginTime=obj.LoginTime,LoginToken=obj.LoginToken, UserLoginStateID=obj.UserLoginStateID, UserName=obj.UserName 
            };
        }

        static public implicit operator ML.BC.EnterpriseData.Model.UserLoginState(UserLoginStateDto obj)
        {
            return new ML.BC.EnterpriseData.Model.UserLoginState {
                UserID = obj.UserID,
                Device = obj.Device,
                LoginIP = obj.LoginIP,
                LoginTime = obj.LoginTime,
                LoginToken = obj.LoginToken,
                UserLoginStateID = obj.UserLoginStateID,
                UserName = obj.UserName 
            };
        }

    }
}
