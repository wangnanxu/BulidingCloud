using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ML.BC.Services;
using ML.BC.Web.Framework;

namespace ML.BC.EnterpriseWeb.Areas.APIs.Models
{
    public class ChatMessageModel : ModelBase
    {
        /// <summary>
        /// 多个用户ID用  |  分隔
        /// </summary>
        [Required]
        public string Recipients { get; set; }

        [Required]
        public string Message { get; set; }
        [Required]
        public DateTime SendTime { get; set; }
        [Required]
        public Guid MessageID { get; set; }

    }

    public class GetChatMessageModel : ModelBase
    {
        public DateTime QueryTime { get; set; }
        public int PageSize { get; set; }
    }
}