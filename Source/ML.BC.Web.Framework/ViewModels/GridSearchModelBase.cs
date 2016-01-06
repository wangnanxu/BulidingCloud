using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Web.Framework.ViewModels
{
    public class GridSearchModelBase
    {
        /// <summary>
        /// Grid搜索分页回调数据的基类。
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
            [DefaultValue(15)]
            public int rows { get; set; }
        }
    }
}
