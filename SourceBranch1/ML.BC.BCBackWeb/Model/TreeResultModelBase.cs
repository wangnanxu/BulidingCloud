using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace ML.BC.BCBackWeb.Model
{
   /// <summary>
   /// 树菜单结果基类
   /// </summary>
    public class TreeResultModelBase
    {
        public string id { get; set; }
        public string text { get; set; }
        public string state { get; set;}
        public bool @checked { get; set; }
        public string iconCls { get; set; }
        private IEnumerable<TreeResultModelBase> _children;
        public IEnumerable<TreeResultModelBase> children
        {
            get { if(this._children==null)
            { return new List<TreeResultModelBase>(); }
            else { return this._children; }
            }
            set { _children = value; }
        }
    
    }
 }