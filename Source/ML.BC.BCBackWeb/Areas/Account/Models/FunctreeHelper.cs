using ML.BC.Services.Account.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Areas.Account.Models
{
    //每一条json数据的格式
    public class JsonItem : FunctionDto
    {
        public int id;//自行生成
        public int _parentId;//自行生成
        public string state = "open";
        public string text;

    }
    //需要的最终json数据格式       //前端easyui需要的数据格式
    public class JsonResults
    {
        public int total;
        public List<JsonItem> rows;
    } 
    public class FunctreeHelper
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