using ML.BC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Areas.Account.Models
{
    public class LoginLogShowModel
    {
        public string UserLoginLogID { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }//
        public string EnterpriseName { get; set; }//
        public string DepartmentName { get; set; }
        public string IP { get; set; }
        public string Time { get; set; }// //
        public string Device { get; set; }
        public int Status { get; set; }//
        public string Description { get; set; }

        static public implicit operator LoginLogShowModel(UserLoginLogDto dto)
        {
            return new LoginLogShowModel
            {
                UserLoginLogID = dto.UserLoginLogID + "",
                UserID = dto.UserID,
                Status = (int)dto.Status,
                DepartmentName = dto.DepartmentName,
                Description = dto.Description ?? "",
                Device = dto.Device,
                EnterpriseName = dto.EnterpriseName,
                IP = dto.IP,
                Time = dto.Time.ToString(),
                UserName = dto.UserName
            };
        }
    }

    public class ResultModel
    {
        public int total;
        public List<LoginLogShowModel> rows;
    }

    public class LoginLogQueryModel
    {
        public int page { get; set; }
        public int rows { get; set; }
        public string UserName { get; set; }
        public string EnterpriseName { get; set; }
        public int? Status { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}