using ML.BC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Models
{
    public class PersonStatisGetModel
    {
        public int page { get; set; }
        public int rows { get; set; }

        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }

        public string UserName { get; set; }
    }

    public class PersonStatisResultModel
    {
        public int total { get; set; }
        public List<PersonStatisDto> rows { get; set; }

        public List<PersonStatisDto> footer { get; set; }
    }
}