using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IRectificationStatistical
    {
        RectificationStatisticalDto GetRectificationStatistical(string enterpriseID,int? departmentID,string userID);
    }
}
