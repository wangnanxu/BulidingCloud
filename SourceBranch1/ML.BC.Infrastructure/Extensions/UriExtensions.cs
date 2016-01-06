using System;

namespace ML.BC.Infrastructure
{
    public static class UriExtensions
    {
        /// <summary>
        /// returns something like: http://www.google.com
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetRoot(this Uri uri)
        {
            var port = uri.Port == 80 ? string.Empty : ":" + uri.Port;
            return string.Format(uri.Scheme + "://" + uri.Host + port);
        }

        public static string GetFullUrl(string relateUrl)
        {
            var config = System.Configuration.ConfigurationManager.ConnectionStrings["UserPictureUrl"];
            if (config == null || string.IsNullOrEmpty(config.ConnectionString)) throw new ML.BC.Infrastructure.Exceptions.KnownException("webconfig缺少UserPictureUrl配置项。");
            if (!config.ConnectionString.Contains("http")) throw new ML.BC.Infrastructure.Exceptions.KnownException("webconfig中UserPictureUrl配置项格式出错，应该以Http开头。");
            var userPictureUrl = config.ConnectionString.Trim("/".ToCharArray());
            
            if (string.IsNullOrEmpty(relateUrl))
                return string.Format("{0}/Content/Images/head.jpg", userPictureUrl);

            relateUrl = relateUrl.Trim("/".ToCharArray());
            return string.Format("{0}/{1}", userPictureUrl, relateUrl);
        }
    }
}
