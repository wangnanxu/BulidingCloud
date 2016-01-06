using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Areas.System.Models
{
    public class SystemLogResultModel
    {
        public string FileName { get; set; }
        public string FullName { get; set; }
        public int LogType { get; set; }
        /// <summary>
        /// 下载路径
        /// </summary>
        public string DownUrl { get; set; }
        public long FileSize { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }

}