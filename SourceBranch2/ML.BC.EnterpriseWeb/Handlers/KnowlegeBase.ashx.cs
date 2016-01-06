using ML.BC.EnterpriseData.MongoDb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ML.BC.EnterpriseWeb.Handlers
{
    public class KnowlegeBase : IHttpHandler
    {
        private const string KnowlegeDbName = "KnowledgeBase";
        public void ProcessRequest(HttpContext context)
        {
            var fileName = context.Request.QueryString["FileName"];
            if (string.IsNullOrEmpty(fileName)) return;

            var db = new MongoDbProvider<EnterpriseData.Model.KnowledgeBaseFile>();
            var knData = db.GetFileAsStream(fileName, KnowlegeDbName);
            if (knData.Length == 0) return;

            context.Response.OutputStream.Write(knData, 0, knData.Length);
            context.Response.Flush();
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}