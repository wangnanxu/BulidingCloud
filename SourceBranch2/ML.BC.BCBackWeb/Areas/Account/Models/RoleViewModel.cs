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
    /// 角色视图model
    /// </summary>
    public class RoleViewModel : DataGridViewModelBase
    {
        /// <summary>
        /// RoleID
        /// </summary>
        /// 
        public int? RoleID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  角色拥有者
        /// </summary>
        public string OwnerID { get; set; }

        public string Description { get; set; }

    }
}