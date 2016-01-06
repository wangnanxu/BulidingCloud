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
    public class EnterpriseRoleSearchModel : GridSearchModelBase.DataGridViewModelBase
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }
    }

 }