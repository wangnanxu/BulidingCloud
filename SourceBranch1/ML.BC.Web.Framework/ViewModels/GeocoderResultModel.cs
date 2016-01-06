using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Web.Framework.ViewModels
{
    /// <summary>
    /// 坐标
    /// </summary>
    public class Coordinate
    {
        /// <summary>
        /// 纬度值
        /// </summary>
        public double lat { get; set; }
        /// <summary>
        /// 经度值
        /// </summary>
        public double lng { get; set; }

    }
    /// <summary>
    /// 复合地址对象
    /// </summary>
    public class AddressComponent
    {
        /// <summary>
        /// 国家
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// 省名:广东省
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 城市 
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 区县名 
        /// </summary>
        public string district { get; set; }
        /// <summary>
        /// 街道名 
        /// </summary>
        public string street { get; set; }
        /// <summary>
        /// 街道门牌号  
        /// </summary>
        public string street_number { get; set; }
        /// <summary>
        /// 国家code   
        /// </summary>
        public string country_code { get; set; }
        /// <summary>
        /// 和当前坐标点的方向 如:东,西，当有门牌号的时候返回数据   
        /// </summary>
        public string direction { get; set; }
        /// <summary>
        /// 和当前坐标点的距离，当有门牌号的时候返回数据 
        /// </summary>
        public int? distance { get; set; }


    }
    /// <summary>
    /// 点对象
    /// </summary>
    public class Point 
    {
        /// <summary>
        ///  经度
        /// </summary>
        public double x { get; set; }
         /// <summary>
        ///  纬度
        /// </summary>
        public double y { get; set; }


    }
    /// <summary>
    /// 周边点对象
    /// </summary>
    public class PoiModel
    {
        /// <summary>
        /// 地址信息 
        /// </summary>
        public string addr { get; set; }
        /// <summary>
        /// 数据来源 
        /// </summary>
        public string cp { get; set; }
        /// <summary>
        /// 和当前坐标点的方向  :南,北,
        /// </summary>
        public string direction { get; set; }
        public int? distance { get; set; }
        /// <summary>
        ///poi类型，如’ 办公大厦,商务大厦’ 
        /// </summary>
        public int name { get; set; }

        /// <summary>
        /// poi名称 如:海淀剧院
        /// </summary>
        public string poiType { get; set; }
        /// <summary>
        /// 点
        /// </summary>
        public Point point { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string tel { get; set; }
        public string uid { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string zip { get; set; }
    }


    /// <summary>
    /// 地理编码响应结果
    /// </summary>
    public class GeocoderResultModel
    {
        /*
         {
 status: 0,
 result: 
 {
 location: 
 {
 lng: 116.30814954222,
 lat: 40.056885091681
},
precise: 1,
confidence: 80,
level: "商务大厦"
}
}
         */

        /// <summary>
        /// 结果状态:0  正常  
        /*1  服务器内部错误  
        2  请求参数非法  
        3  权限校验失败  
        4  配额校验失败  
        5  ak不存在或者非法  
        101  服务禁用  
        102  不通过白名单或者安全码不对  
        2xx  无权限  
        3xx  配额错误  */
        /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 结果
        /// </summary>
        public Result result { get; set; }

    }
    /// <summary>
    /// 解析结果 result
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 坐标
        /// </summary>
        public Coordinate location { get; set; }
        /// <summary>
        /// 位置的附加信息，是否精确查找。1为精确查找，0为不精确。
        /// </summary>
        public int? precise { get; set; }
        /// <summary>
        /// 可信度
        /// </summary>
        public int? confidence { get; set; }
        /// <summary>
        /// 地址类型
        /// </summary>
        public string level { get; set; }
    }
    /// <summary>
    /// 逆解析结果 result
    /// </summary>
    public class ReverseResult
    {
        public Coordinate location { get; set; }
        /// <summary>
        /// 结构化地址信息 
        /// </summary>
        public string formatted_address { get; set; }
        /// <summary>
        /// 所在商圈信息，如 "解放碑,观音桥" 
        /// </summary>
        public string business { get; set; }

        /// <summary>
        ///  复合地址
        /// </summary>
        public AddressComponent addressComponent { get; set; }
        /// <summary>
        /// 周边点集合
        /// </summary>
        public List<PoiModel> pois { get; set; }
        /// <summary>
        /// 当前位置结合POI的语义化结果描述。
        /// </summary>
        public string sematic_description { get; set; }
        /// <summary>
        /// 城市代码
        /// </summary>
        public string cityCode { get; set; }
    }
    /// <summary>
    /// 逆地理编码响应结果
    /// </summary>
    public class ReverseGeocoderResultModel
    {


      /// <summary>
        /// 结果状态:0  正常  1  服务器内部错误  2  请求参数非法 3  权限校验失败  4  配额校验失败  5  ak不存在或者非法  101  服务禁用  102  不通过白名单或者安 全码不对2xx  无权限  3xx  配额错误
      /// </summary>
        public int? status { get; set; }
        /// <summary>
        /// 坐标
        /// </summary>
        public ReverseResult result { get; set; }
         
    }
}
