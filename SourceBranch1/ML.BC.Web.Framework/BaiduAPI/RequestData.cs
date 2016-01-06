using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ML.BC.Web.Framework.BaiduAPI
{
    public enum ContentType
    {
        Post,
        Get
    }
    /// <summary>
    /// 
    /// </summary>
    public class RequestData
    {
        static Regex _reCharset = new Regex("charset=['\" ]*(.+?)['\"/ ]*>");

        static string GZipDeCompress(Stream s,Encoding encoding=null)
        {
            byte[] bt = new byte[40000];
            int len = 0;
            using (GZipStream gzips = new GZipStream(s, CompressionMode.Decompress))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    while ((len = gzips.Read(bt, 0, bt.Length)) != 0)
                    {
                        ms.Write(bt, 0, len);
                    }
                    byte[] msb = ms.ToArray();
                    if (encoding == null) encoding = Encoding.UTF8;
                    string html = encoding.GetString(msb);
                    return html;
                }
            }
        }
        static Encoding GetEncoding(string sData)
        {
            Match match = _reCharset.Match(sData);
            if (match.Success)
            {
                try
                {
                    return Encoding.GetEncoding(match.Groups[1].Value);
                }
                catch (Exception e)
                {
                    throw (e);
                }
            }
            return Encoding.UTF8;
        }

        public static string GetResponseStream(string url)
        {
            Uri uri = new Uri(url);
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.CachePolicy = new System.Net.Cache.RequestCachePolicy();
            httpWebRequest.KeepAlive = true;
            httpWebRequest.AllowAutoRedirect = true;
            httpWebRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
            httpWebRequest.Headers.Add("Accept-Language", "zh-CN");
            httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:38.0) Gecko/20100101 Firefox/38.0";
            httpWebRequest.Accept = "*/*";
            httpWebRequest.Headers.Add("Pragma", "no-cache");

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            MemoryStream ms = new MemoryStream();
            //if (httpWebResponse.ContentType != "text/html"){  }
            string resEncoding = httpWebResponse.ContentEncoding;
            string characterSet = "utf-8";
            if (!string.IsNullOrEmpty(httpWebResponse.CharacterSet))
            {
                characterSet = httpWebResponse.CharacterSet;
            }
            var newencodeing = Encoding.GetEncoding(characterSet);
            Stream s = httpWebResponse.GetResponseStream();
            string result = "";

            if (resEncoding.ToLower() == "gzip")
            {
                result = GZipDeCompress(s,newencodeing);
            }
            else
            {
               
                using (StreamReader stred = new StreamReader(s, newencodeing))
                {
                    result = stred.ReadToEnd();
                }
            }
            httpWebResponse.Close();
            httpWebRequest.Abort();
            return result;
        }

    }
}
