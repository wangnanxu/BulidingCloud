using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Web.Framework.ViewModels
{
    /// <summary>
    /// 地理编码请求参数基类
    /// </summary>
    public class GeocoderRequestParamBase
    {
        /// <summary>
        /// 返回数据格式 json or xml 可选
        /// </summary>
        [DefaultValue("json")]
        public string output { get; set; }
        /// <summary>
        /// api 密匙 必须
        /// </summary>
        /// 
        public string ak { get; set; }
        /// <summary>
        /// sn 算法 可选的
        /// </summary>
        public string sn { get; set; }
        /// <summary>
        /// 回调函数 可选
        /// </summary>
        /// 
        [DefaultValue("")]
        public string callback { get; set; }
    }
    /// <summary>
    /// 地理编码服务请求参数
    /// </summary>
    public class GeocodingParam : GeocoderRequestParamBase
    {
        /// <summary>
        /// 地址 必须的 如:北京市海淀区上地十街10号
        /// 可以输入三种样式的值，分别是： •标准的地址信息，如北京市海淀区上地十街十号; •名胜古迹、标志性建筑物，如天安门，百度大厦; • 支持 “*路与*路交叉口”描述方式，如北一环路和阜阳路的交叉路口 注意：后两种方式并不总是有返回结果，只有当地址库中存在该地址描述时才有返回
        /// </summary>
        [DefaultValue("")]
        public string address { get; set; }
        /// <summary>
        /// 城市 可选的如:重庆市
        /// 地址所在的城市名 该参数是可选项，用于指定上述地址所在的城市，当多个城市都有上述地址时，该参数起到过滤作用。 
        /// </summary>
        /// 
        public string city { get; set; }
        /// <summary>
        /// sn 算法 可选的
        /// </summary>
        new public string sn { get; set; }
    }
    /// <summary>
    /// 逆地理编码服务请求参数
    /// </summary>
    public class ReverseGeocodingParam : GeocoderRequestParamBase
    {
        /// <summary>
        /// 
        /// 坐标的类型可选的，目前支持的坐标类型包括：bd09ll（默认百度经纬度坐标）、gcj02ll（国测局经纬度坐标）、wgs84ll（ GPS经纬度）
        /// 
        /// </summary>
        /// 
    
     public   string coordtype { get; set; }
   /// <summary>
     /// 39.983424,116.322987 lat纬度,lng经度
   /// </summary>
      public  string location{ get; set; }
        /// <summary>
        /// 是否显示指定位置周边的poi，0为不显示，1为显示。当值为1时，显示周边100米内的poi。
        /// </summary>
      public  int pois  { get; set; }
    }

}
