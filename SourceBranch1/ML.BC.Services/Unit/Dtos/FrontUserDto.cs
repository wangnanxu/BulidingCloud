using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services.Account.Dtos
{
    public class FrontUserDto
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string EnterpiseID { get; set; }
        public string Mobile { get; set; }
        public System.DateTime RegistDate { get; set; }
        public Nullable<System.DateTime> LastDate { get; set; }
        public string LastIP { get; set; }
        public bool LoginByDesktop { get; set; }
        public bool LoginByMobile { get; set; }
        public bool Closed { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public int DepartmentID { get; set; }
    }
}
