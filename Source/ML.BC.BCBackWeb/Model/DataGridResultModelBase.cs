﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Model
{
   /// <summary>
   /// DataGrid列表结果基类
   /// </summary>
    public class DataGridResultModelBase
    {
        /// <summary>
        /// 总页数
        /// </summary>
        ///
        [DefaultValue(1)]
        public int total { get; set; }
        /// <summary>
        /// 列表集合
        /// </summary>
        /// 
            [DefaultValue("")]
        public string rows { get; set; }

    }
    /// <summary>
    ///  DataGrid结果基类<T>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataGridResultModelBase<T> where T : class
    {
        /// <summary>
        /// 总个数
        /// </summary>
        ///
        [DefaultValue(1)]
        public int total { get; set; }
        /// <summary>
        /// 列表集合
        /// </summary>
        /// 
            [DefaultValue(null)]
        public List<T> rows { get; set; }



    }
}