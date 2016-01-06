using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public class DistinctByFileGuid:IEqualityComparer<KnowlegeDto>
    {
        public bool Equals(KnowlegeDto x, KnowlegeDto y)
        {
            return x.FileGUID == y.FileGUID;
        }

        public int GetHashCode(KnowlegeDto obj)
        {
            return obj.FileGUID.ToString().GetHashCode();
        }
    }
}
