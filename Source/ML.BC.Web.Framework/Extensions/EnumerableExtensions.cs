using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Model;

namespace MvcSolution
{
    public static class EnumerableExtensionsWeb
    {
        public static List<SelectListItem> ToSelectList(this List<SimpleEntity> entities, string optionalLabel = null)
        {
            return entities.ToSelectList(x => x.Name, x => x.Id, optionalLabel);
        } 
    }
}