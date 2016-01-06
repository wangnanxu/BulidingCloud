using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ML.BC.Services;
using ML.BC.Web.Framework.ViewModels;
namespace ML.BC.EnterpriseWeb.Areas.Account.Models
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class UserLoginStateViewModel : GridSearchModelBase.DataGridViewModelBase
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
    }

 }