using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ML.BC.Services;
namespace ML.BC.EnterpriseWeb.Areas.Scene.Models
{
    public class ProjectModel
    {
       public string ProjectID { get; set; }
       public string Name { get; set; }
       public string EnterpriseID { get; set; }
       public string []Departments { get; set; }
       public string []Managers { get; set; }
       public string []Roles { get; set; }
       public DateTime RegistDate { get; set; }
       public DateTime BeginDate { get; set; }
       public DateTime EndDate { get; set; }
       public int Status { get; set; }
       public bool Deleted { get; set; }
    }
    public class ProjectListResult {
        public List<ProjectDto> rows;
        public int total;
    }
}