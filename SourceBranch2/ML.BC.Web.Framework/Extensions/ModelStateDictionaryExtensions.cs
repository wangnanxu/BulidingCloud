using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ML.BC.Web.Framework
{
    public static class ModelStateDictionaryExtensions
    {
        public static string GetAllError(this ModelStateDictionary modelState)
        {
            if (modelState.IsValid) return string.Empty;

            var messages = new StringBuilder();
            modelState.ToList().ForEach(n => {
                if (n.Value.Errors != null && n.Value.Errors.Any())
                {
                    messages.AppendFormat(",{0}",string.Join(",",n.Value.Errors.Select(e=>e.ErrorMessage)));
                }
            });
           
            var str = messages.ToString().Trim(",".ToCharArray());
            return str;
        }

        public static string GetFirstError(this ModelStateDictionary modelState)
        {
            if (modelState.IsValid) return string.Empty;

            var str = modelState.Where(n=>n.Value!=null && n.Value.Errors.Any(e=>!string.IsNullOrEmpty(e.ErrorMessage)))
                .Select(n=>n.Value.Errors.First().ErrorMessage)
                .First();
            return str;
        }
    }
}
