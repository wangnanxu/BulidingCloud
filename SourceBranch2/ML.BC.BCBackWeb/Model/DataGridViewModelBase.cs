using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Model
{
    /// <summary>
    /// 视图model基类
    /// </summary>
    public class DataGridViewModelBase
    {
        /// <summary>
        /// 页索引
        /// </summary>
        ///
        [DefaultValue(1)]
        public int page { get; set; }
        /// <summary>
        /// 每页大小
        /// </summary>
        /// 
            [DefaultValue(30)]
        public int rows { get; set; }

    }
}