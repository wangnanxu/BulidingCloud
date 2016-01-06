using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ML.BC.EnterpriseWeb.Areas.ReportForm.Models
{
    public class DepartmentTableModel
    {
        public string Department { get; set; }
        public int People { get; set; }
        public int Project { get; set; }
        public int Scene { get; set; }
        public int Picture { get; set; }
        public int ParentID { get; set; }
        public string PictureSpace { get; set; }
        public float PictureMb { get; set; }
        public int id { get; set; }
        public int _parentId { get; set; }
    }
    public class DepartmentTableResult
    {
        public int total { get; set; }
        public List<DepartmentTableModel> rows { get; set; }
        public List<DepartmentTableModel> footer { get; set; }
    }
}