using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Areas.System.Models
{
    public class SystemLogViewModel :  ML.BC.Web.Framework.ViewModels.GridSearchModelBase.DataGridViewModelBase
    {
        /// <summary>
        /// 0全部;1错误,2调试,3详情
        /// </summary>
        public int LogType { get; set; }
        /// <summary>
        /// 创建时间 开始
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 创建时间 结束
        /// </summary>
        public DateTime? EndDate { get; set; }

    }
}