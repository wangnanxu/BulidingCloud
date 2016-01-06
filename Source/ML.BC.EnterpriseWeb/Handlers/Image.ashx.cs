using ML.BC.EnterpriseData.MongoDb;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ML.BC.EnterpriseWeb.Handlers
{
    public class Image : IHttpHandler
    {
        private const string PictureDbName = "Picture";
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = System.Web.MimeMapping.GetMimeMapping(".jpg");
            var fileName = context.Request.QueryString["FileName"];
            if (string.IsNullOrEmpty(fileName)) return;

            var db = new MongoDbProvider<ML.BC.EnterpriseData.Model.SceneItem>();
            var imgData = db.GetFileAsStream(fileName, PictureDbName);
            if (imgData.Length == 0) return;

            context.Response.OutputStream.Write(imgData, 0, imgData.Length);
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