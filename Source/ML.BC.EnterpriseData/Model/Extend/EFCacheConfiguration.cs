using EFCache;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.EnterpriseData.Model.Extend
{
    //https://msdn.microsoft.com/en-us/data/jj680699#Using
    //public class EFCacheConfiguration : DbConfiguration
    //{
    //    internal static readonly InMemoryCache Cache = new InMemoryCache();

    //    public EFCacheConfiguration()
    //    {
    //        var transactionHandler = new CacheTransactionHandler(Cache);
    //        AddInterceptor(transactionHandler);

    //        Loaded +=
    //          (sender, args) => args.ReplaceService<DbProviderServices>(
    //            (s, _) => new CachingProviderServices(s, transactionHandler));
    //    }

    //    public static int GetCountOfCache()
    //    {
    //        Cache.Purge();
    //        return Cache.Count;
    //    }

    //    public static void CachePurge()
    //    {
    //        Cache.Purge();
    //    }

    //    public static void InvalidateSetsCache(IEnumerable<string> dataSets)
    //    {
    //        Cache.InvalidateSets(dataSets);
    //    }
    //    public static void InvalidateSetsCache(string dataSet)
    //    {
    //        var dataSets = new List<string> { dataSet };
    //        Cache.InvalidateSets(dataSets);
    //    }
    //}
}
