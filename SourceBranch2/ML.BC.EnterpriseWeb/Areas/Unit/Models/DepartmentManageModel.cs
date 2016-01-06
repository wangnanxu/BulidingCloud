using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ML.BC.Services;
using ML.BC.Services.Unit.Dtos;
namespace ML.BC.EnterpriseWeb.Areas.Unit.Models
{
    public class DepartmentManageModel
    {
        public int DepartmentID { get; set; }
        public string Name { get; set; }
        public string EnterpriseID { get; set; }
        public int ParentID { get; set; }
        public int _parentId { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
        public bool Deleted { get; set; }
    }
    public class DepartmentManageResultModel
    {
        public int total { get; set; }
        public List<DepartmentList> rows { get; set; }
    }
    public class DepartmentList : DepartmentDto
    {
        public int _parentId { get; set; }
        public int id { get; set; }
    }
    public class DepartmentTree
    {
        public int id { set; get; }//部门id
        public string text { set; get; }//部门名称


        public List<DepartmentTree> children;

        static public implicit operator DepartmentTree(DepartmentManageModel dto)
        {
            return new DepartmentTree
            {
                id = dto.DepartmentID,
                text = dto.Name,
                children = new List<DepartmentTree>()

            };
        }

    }
    public class DepartmentTreeHelper
    {
        private Dictionary<string, int> map = new Dictionary<string, int>();//储存字符串id到数字id的映射

        //将字符串形式id转换为数字id并储存到字典供easyui使用
        //easyui仅支持数字id

        public int processSID(string sid)
        {


            if (sid.Equals("")) return 0;

            if (map.ContainsKey(sid))
            {
                return map[sid];
            }
            else
            {
                int idx = map.Count + 1;
                map.Add(sid, idx);
                return idx;
            }
        }
    }
}