using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ML.BC.Web.Framework.ViewModels;
using ML.BC.Infrastructure;

namespace ML.BC.Web.Framework.BaiduAPI
{
    /// <summary>
    /// 逆地址编码
    /// </summary>
    public class ReverseGeocoding
    {
        string host = "http://api.map.baidu.com/geocoder/v2/?";
        string Parameter="";
        private string Url { get{ return host+Parameter;} set{ Url=value;} }
        private ReverseGeocodingParam ParamModel { get; set; }
        public ReverseGeocoding(ReverseGeocodingParam model)
        {
            string _host = System.Web.Configuration.WebConfigurationManager.AppSettings["BaiduGeocoderHost"];
            string _BaiduGeocoderAK = System.Web.Configuration.WebConfigurationManager.AppSettings["BaiduGeocoderAK"];

            if (!string.IsNullOrEmpty(_host))
            {
                host = _host;
            }
            if (!string.IsNullOrEmpty(_BaiduGeocoderAK))
            {
                if (string.IsNullOrEmpty(model.ak))
                {
                    model.ak = _BaiduGeocoderAK;
                }
            }
            if (model != null)
            {
                ParamModel = model;
                Parameter += "ak="+model.ak+"&";
             if(!string.IsNullOrEmpty(model.callback))
                Parameter += "callback=" + model.callback + "&";
             if (!string.IsNullOrEmpty(model.coordtype))
             { 
                Parameter += "coordtype=" + model.coordtype + "&";
             }
                 Parameter += "location=" + model.location + "&";
                 Parameter += "pois=" + model.pois + "&";
                Parameter += "output=" + model.output + "&";
                if(!string.IsNullOrEmpty(model.sn))
                Parameter += "sn=" + model.sn + "&";

            }
        }
        /// <summary>
        /// 获取解析结果
        /// </summary>
        /// <returns></returns>
        public ReverseGeocoderResultModel GetGeocoderResult()
        {
            try
            {
                string str = RequestData.GetResponseStream(Url);
                ReverseGeocoderResultModel model = new ReverseGeocoderResultModel();
                if (!string.IsNullOrEmpty(str))
                {
                    if (ParamModel != null)
                    {
                        if (ParamModel.output.ToString().ToLower() == "xml")
                        {
                            model = Serializer.FromXml<ReverseGeocoderResultModel>(str);
                        }
                        else
                        {
                            model = Serializer.FromJson<ReverseGeocoderResultModel>(str);
                        }
                    }

                }
                return model;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
 }
