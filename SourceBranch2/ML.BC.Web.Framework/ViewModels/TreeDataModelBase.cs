using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Web.Framework.ViewModels
{
    /// <summary>
    /// 树菜单Model基类
    /// </summary>
    public class TreeDataModelBase
    {
        public string id { get; set; }
        public string text { get; set; }
        public string state { get; set; }
        public bool @checked { get; set; }
        public string iconCls { get; set; }
        private IEnumerable<TreeDataModelBase> _children;
        public IEnumerable<TreeDataModelBase> children
        {
            get
            {
                if (this._children == null)
                { return new List<TreeDataModelBase>(); }
                else { return this._children; }
            }
            set { _children = value; }
        }
    }
}
