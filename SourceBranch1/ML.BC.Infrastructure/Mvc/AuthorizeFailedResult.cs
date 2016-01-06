using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Infrastructure.Mvc
{
    public class AuthorizeFailedResult : StandardJsonResult, IStandardResult
    {
        public bool AuthorizeFailed { get { return true; } }
    }
    public class AuthorizeFailedResult<T> : StandardJsonResult, IStandardResult<T>
    {
        public bool AuthorizeFailed { get { return true; } }
        public T Value { get; set; }
        protected override IStandardResult ToCustomResult()
        {
            var result = new StandardResult<T>();
            result.Success = this.Success;
            result.Message = this.Message;
            result.Value = this.Value;
            return result;
        }
    }
}
