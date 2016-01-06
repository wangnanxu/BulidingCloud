
using ML.BC.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace ML.BC.EnterpriseWeb.Areas.Account.Models
{
    //获取列表 搜索用模型
    public class EnterpriseUserViewModel
    {
        [Required]
        [DefaultValue(1)]
        public int page { get; set; }

        [DefaultValue(10)]
        public int rows { get; set; }

        [DefaultValue("")]
        public string name { get; set; }

        [DefaultValue("")]
        public string depart { get; set; }

        public int? departID { get; set; }

    }

    //新建 更新用模型
    public class EnterpriseUserNewModel
    {

        public string UserID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Password { get; set; }   
        public string EnterpiseID { get; set; }
        [Required]
        public string Mobile { get; set; }
        public System.DateTime RegistDate { get; set; }

        [DefaultValue(false)]
        public bool Closed { get; set; }
        public System.DateTime UpdateTime { get; set; }    

        [Required]
        public int? DepartmentID { get; set; }

        [Required]
        public List<int> Roles { set; get; }
        static public implicit operator FrontUserDto(EnterpriseUserNewModel model)
        {
            return new FrontUserDto
            {
                UserID = model.UserID,
                Name = model.Name,
                Mobile = model.Mobile,
                Closed = model.Closed,
                DepartmentID = model.DepartmentID,
                Password = model.Password,
                RegistDate = model.RegistDate,
                UpdateTime = model.UpdateTime,
                EnterpiseID = model.EnterpiseID,
                UserRoleIDs = model.Roles
            };
        }
    }


    //单个结果用模型
    public class EnterpriseUserJsonItemModel
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        //public string EnterpiseID { get; set; }
        public string Mobile { get; set; }
        public string RegistDate { get; set; }
        public string LastDate { get; set; }
        public string LastIP { get; set; }
        //public bool LoginByDesktop { get; set; }
        //public bool LoginByMobile { get; set; }
        public bool Closed { get; set; }
        public string UpdateTime { get; set; }
        public int? DepartmentID { get; set; }

        //-----------
        public string DepartmentName { get; set; }
        public List<int> Roles { set; get; }

        static public implicit operator EnterpriseUserJsonItemModel(FrontUserDto dto)
        {
            return new EnterpriseUserJsonItemModel
            {
                UserID = dto.UserID,
                Name = dto.Name,
                Mobile = dto.Mobile,
                RegistDate = dto.RegistDate.ToString("yyyy-MM-dd HH:mm"),

                LastDate = dto.LastDate==null?"":dto.LastDate.Value.ToString("yyyy-MM-dd HH:mm"),
                LastIP = dto.LastIP,
                Closed = dto.Closed,
                UpdateTime = dto.UpdateTime.ToString("yyyy-MM-dd HH:mm"),
                DepartmentID = dto.DepartmentID
                //departmentname = ?????
            };
        }

    }

    //返回数据模型
    public class EnterpriseUserResultModel
    {
        public int total;
        private List<EnterpriseUserJsonItemModel> _rows;
        public List<EnterpriseUserJsonItemModel> rows
        {
            set
            {
                _rows = value;

            }
            get
            {
                if (_rows == null) return new List<EnterpriseUserJsonItemModel>(); else return _rows;
            }
        }
    }

    //传到前台的角色数据模型
    public class EnterpriseRoleViewModel
    {
        public string RoleName;
        public string RoleID;
    }
    //传到前台的树形部门结构
    public class DepartmentTreeModel
    {
        public int id { set; get; }//部门id
        public string text { set; get; }//部门名称


        public List<DepartmentTreeModel> children;

        static public implicit operator DepartmentTreeModel(DepartmentDto dto)
        {
            return new DepartmentTreeModel
            {
                id = dto.DepartmentID,
                text = dto.Name,
                children = new List<DepartmentTreeModel>()

            };
        }

    }
}