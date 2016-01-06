﻿using System;
using System.Drawing.Imaging;
using System.Web.Mvc;

namespace ML.BC.Infrastructure
{
    public class CaptchaController : Controller
    {
        public virtual void Refresh()
        {
            var img = new ImageBuilder();
            const string chars = "23456789qwertyupkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";
            var code = string.Empty;
            var ramdon = new Random();
            for (var i = 0; i < 4; i++)
            {
                code += chars.Substring(ramdon.Next(0, chars.Length - 1), 1);
            }
            var bmp = img.Generate(code);
            Session[Config.SessionKey] = code;
            bmp.Save(HttpContext.Response.OutputStream, ImageFormat.Jpeg);
        }
    }
}
