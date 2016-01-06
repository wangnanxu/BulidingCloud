using System;

namespace ML.BC.Infrastructure.Caching
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class CacheExtensions
    {
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, 60, acquire);
        }

        public static T Get<T>(this ICacheManager cacheManager, string key, Func<long, T> acquire) where T : class
        {
            return Get(cacheManager, key, 60, acquire);
        }

        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<long , T> acquire) where T : class 
        {
            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }

            if(key.IndexOf("_" )> 0)
            {
                var sid = key.Split('_')[1];
                long id;
                long.TryParse(sid, out id);
                var result = acquire(id);
                cacheManager.Set(key, result, cacheTime);
                return result;
            }

            return null;
        }

        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire) 
        {
            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }
            else
            {
                var result = acquire();
                //if (result != null)
                    cacheManager.Set(key, result, cacheTime);
                return result;
            }
        }
    }
}
