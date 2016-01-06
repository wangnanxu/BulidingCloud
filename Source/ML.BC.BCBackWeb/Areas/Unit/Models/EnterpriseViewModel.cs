using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ML.BC.BCBackWeb.Model;

namespace ML.BC.BCBackWeb.Areas.Unit.Models
{
    /// <summary>
    /// 企业视图model
    /// </summary>
    public class EnterpriseViewModel : DataGridViewModelBase
    {
  /// <summary>
  /// 行业id
  /// </summary>
     public  string ProfessionID{ get; set; }
        /// <summary>
        /// 性质id
        /// </summary>
      public string PropertyID { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>

      public string Name { get; set; }

    }
}