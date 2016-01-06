using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.IO;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ML.BC.Web.Framework
{
    public static class HtmlExtensions
    {
        public static IHtmlString TimeFrameSelector(this HtmlHelper helper)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<select class=\"date-options\">");
            sb.AppendLine("<option value=\"Today\">今天</option>");
            sb.AppendLine("<option value=\"Yesterday\">昨天</option>");
            sb.AppendLine("<option value=\"ThisWeek\">本周</option>");
            sb.AppendLine("<option value=\"LastWeek\">上周</option>");
            sb.AppendLine("<option value=\"Last7Days\">最近7天</option>");
            sb.AppendLine("<option value=\"ThisMonth\">本月</option>");
            sb.AppendLine("<option value=\"LastMonth\">上月</option>");
            sb.AppendLine("<option value=\"Last30Days\">最近30天</option>");
            sb.AppendLine("<option value=\"Custom\">自定义</option></select>");
            return helper.Raw(sb.ToString());
        }

        public static string Url(this HtmlHelper helper, string routeName, string action, string controller)
        {
            RouteValueDictionary dic = null;
            dic = new RouteValueDictionary();

            var str = helper.Raw(UrlHelper.GenerateUrl(routeName, action, controller, dic, RouteTable.Routes, helper.ViewContext.RequestContext, false)).ToString();
            return str;
        }
        public static string Url(this HtmlHelper helper, string routeName, string action, string controller, object routeValues)
        {
            RouteValueDictionary dic = new RouteValueDictionary(routeValues);
            var str = helper.Raw(UrlHelper.GenerateUrl(routeName, action, controller, dic, RouteTable.Routes, helper.ViewContext.RequestContext, false)).ToString();
            return str;
        }

        public static IHtmlString Button(this HtmlHelper helper, Expression<Func<bool>> checkPermission, string text, object authorHtmlAttributes = null, object bHtmlAttributes = null, bool isIncludeTitle = false, bool isToolBtn = true,bool  isHaveLine=true)
        {

            if (!checkPermission.Compile().Invoke())
            {
                return MvcHtmlString.Create(string.Empty);
            }

            var authorDic = new Dictionary<string, object>();
            if (isToolBtn)
            {
                authorDic.Add("class", "tools_btn");
            }
            if (!isIncludeTitle)
            {
                authorDic.Add("title", text);
            }
            if (authorHtmlAttributes != null)
            {
                //Merge htmlAttributes
                authorDic = authorDic.Union(authorHtmlAttributes.ToDictionary())
                    .ToLookup(pair => pair.Key, pair => pair.Value)
                    .ToDictionary(group => group.Key, group => (object)string.Join(" ", group.ToArray().Cast<string>()));
            }
            var authorAttr = string.Join(" ", authorDic.Select(n => string.Format("{0}=\"{1}\"", n.Key, n.Value)));

            var bAttr = string.Empty;
            if (bHtmlAttributes != null)
            {
                var bDic = bHtmlAttributes.ToDictionary();
                bAttr = string.Join(" ", bDic.Select(n => string.Format("{0}=\"{1}\"", n.Key, n.Value)));
            }
            string LineHtml = "";
            if (isHaveLine == true) {
                LineHtml = "<div class='tools_separator'> </div>";
            }
            var str = string.Format("<a {0}><span><b {1}>{2}</b></span></a>{3}", authorAttr, bAttr, text,LineHtml);

            return MvcHtmlString.Create(str);

            //<a title="刷新当前页面" onclick="Replace();" class="tools_btn"><span><b  class="btn_Refresh">刷新</b></span></a>
        }
        /// <summary>
        /// 生成菜单项
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="checkPermission"></param>
        /// <param name="divHtmlAttributes"></param>
        /// <param name="img"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static IHtmlString MenuItem(this HtmlHelper helper, Expression<Func<bool>> checkPermission,object divHtmlAttributes = null, string img = "", string text="")
        {

            if (!checkPermission.Compile().Invoke())
            {
                return MvcHtmlString.Create(string.Empty);
            }

            var divDic = new Dictionary<string, object>();
          
            if (divHtmlAttributes != null)
            {
                divDic = divDic.Union(divHtmlAttributes.ToDictionary())
                    .ToLookup(pair => pair.Key, pair => pair.Value)
                    .ToDictionary(group => group.Key, group => (object)string.Join(" ", group.ToArray().Cast<string>()));
            }
            var divAttr = string.Join(" ", divDic.Select(n => string.Format("{0}=\"{1}\"", n.Key, n.Value)));
            string imgHtml = "<img src='/content/Images/32/{0}'/>";
            imgHtml = string.Format(imgHtml, img);
            var str = string.Format("<div {0}>{1} {2}</div>", divAttr, imgHtml, text);

            return MvcHtmlString.Create(str);

           /*
             <div onclick="AddTabMenu('7183d9c5-d48b-436a-9f62-7f30f5a02c5c', 'url', '企业列表', 'icon-EnterpriseList', 'true');">
                <img src="/content/Images/32/EnterpriseList.png" />
                企业列表
            </div>
            */
        }

        #region private functions
        private static IDictionary<string, object> ToDictionary(this object data)
        {
            if (data == null) return null;

            BindingFlags publicAttributes = BindingFlags.Public | BindingFlags.Instance;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();

            foreach (PropertyInfo property in
                     data.GetType().GetProperties(publicAttributes))
            {
                if (property.CanRead)
                {
                    dictionary.Add(property.Name, property.GetValue(data, null));
                }
            }
            return dictionary;
        }
        #endregion
    }
}