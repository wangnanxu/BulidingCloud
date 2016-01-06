using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ML.BC.Services;
using ML.BC.Web.Framework.ViewModels;
namespace ML.BC.EnterpriseWeb.Areas.Account.Models
{
 
    /// <summary>
    /// 数据model
    /// </summary>
    public class EnterpriseRoleDataModel
    {
        public int RoleID { get; set; }
        public string Name { get; set; }
        public string OwnerID { get; set; }
        public string EnterpriseName { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
        public string FunctionIDs { get; set; }

        static public implicit operator EnterpriseRoleDto(EnterpriseRoleDataModel M)
        {
            return new EnterpriseRoleDto {
                Available = M.Available,
                Description = M.Description,
                EnterpriseName = M.EnterpriseName,
                FunctionIDs = M.FunctionIDs,
                Name = M.Name,
                OwnerID = M.OwnerID,
                RoleID = M.RoleID
            };
        }
    }
}