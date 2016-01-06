using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ML.BC.BCBackWeb.Model;

namespace ML.BC.BCBackWeb.Areas.Account.Models
{
    /// <summary>
    /// 企业角色视图model
    /// </summary>
    public class EnterpriseRoleViewModel : DataGridViewModelBase
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  企业名称
        /// </summary>
        public string EnterpriseName { get; set; }

    }
}