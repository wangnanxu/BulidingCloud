using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Web.Framework.ViewModels
{
    /// <summary>
    /// DataGrid列表结果基类
    /// </summary>
    public class GridDataModelBase
    {
        /// <summary>
        /// 总页数
        /// </summary>
        ///
        [DefaultValue(1)]
        public int total { get; set; }
        /// <summary>
        /// 列表集合,Json序列化后的Json字符串
        /// </summary>
        /// 
        [DefaultValue("")]
        public string rows { get; set; }

    }
    /// <summary>
    ///  DataGrid结果基类<T>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GridDataModelBase<T> where T : class
    {
        /// <summary>
        /// 总个数
        /// </summary>
        ///

        public int total { get; set; }
        /// <summary>
        /// 列表集合
        /// </summary>
        /// 

        public List<T> rows { get; set; }

    }
}
