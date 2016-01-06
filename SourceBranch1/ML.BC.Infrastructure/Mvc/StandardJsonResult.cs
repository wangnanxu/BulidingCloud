﻿using ML.BC.Infrastructure.Exceptions;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ML.BC.Infrastructure.Mvc
{
    public class StandardJsonResult : ActionResult, IStandardResult
    {
        log4net.ILog log = log4net.LogManager.GetLogger("StandardJsonResult");

        public string ContentType { get; set; }

        #region Implementation of ICustomResult

        public bool Success { get; set; }

        public string Message { get; set; }

        public void Succeed()
        {
            this.Success = true;
        }

        public void Fail()
        {
            this.Success = false;
        }

        public void Succeed(string message)
        {
            this.Success = true;
            this.Message = message;
        }

        public void Fail(string message)
        {
            this.Success = false;
            this.Message = message;
        }

        public void Try(Action action)
        {
            try
            {
                action();
                this.Succeed();
            }
            catch (Exception ex)
            {
                log.Error("错误:", ex);
                if (AppContext.IsTestServer || HttpContext.Current.Request.IsLocal)
                {
                    if (ex is DbEntityValidationException)
                    {
                        var dbEx = ex as DbEntityValidationException;
                        var errors = dbEx.EntityValidationErrors.Where(x => !x.IsValid)
                            .SelectMany(x => x.ValidationErrors)
                            .Select(x => x.PropertyName + ": " + x.ErrorMessage);
                        this.Fail("数据验证错误：<br/>" + string.Join("<br/>", errors));
                    }
                    else
                    {
                        this.Fail(ex.GetAllMessages());
                    }
                }
                else
                {
                    if (ex is KnownException)
                    {
                        this.Fail(ex.GetAllMessages());
                    }
                    else if (ex is DbEntityValidationException)
                    {
                        this.Fail("数据验证错误，请修改数据重试。");
                    }
                    else
                    {
                        var error = "服务器未知错误，请重试。如果该问题一直存在，请联系管理员。感谢您的支持。";
#if DEBUG
                        error = ex.GetAllMessages();
#endif
                        this.Fail(error);


                    }
                }
            }
        }

        #endregion

        public override void ExecuteResult(ControllerContext context)
        {
            WriteToResponse(context.HttpContext);
        }

        protected virtual IStandardResult ToCustomResult()
        {
            var result = new StandardResult();
            result.Success = this.Success;
            result.Message = this.Message;
            return result;
        }

        public void ValidateModelState(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                var message = "<h5>Please clear the errors first:</h5><ul>";
                foreach (var error in modelState.Values.SelectMany(x => x.Errors))
                {
                    if (!string.IsNullOrEmpty(error.ErrorMessage))
                    {
                        message += "<li>" + error.ErrorMessage + "</li>";
                    }
                }
                message += "</ul>";
                this.Fail(message);
            }
            else
            {
                this.Succeed();
            }
        }

        public void WriteToResponse(HttpContextBase httpContext)
        {
            var response = httpContext.Response;
            if (string.IsNullOrEmpty(this.ContentType))
            {
                response.ContentType = "application/json";
            }
            else
            {
                response.ContentType = this.ContentType;
            }

            var browserId = httpContext.Request.Browser.Id.ToLower();
            var browser = httpContext.Request.Browser.Browser.ToLower();

            if ((browserId.Contains("internetexplorer")
                || browser.Contains("internetexplorer")
                || browserId.Contains("ie")
                || browser.Contains("ie")
                || httpContext.Request.Browser.ActiveXControls)
                && !string.IsNullOrEmpty(response.ContentType)
                && response.ContentType.ToLower() == "application/json")
            {
                response.ContentType = "text/html";
            }

            response.ContentEncoding = Encoding.UTF8;
            response.Write(Serializer.ToJson(this.ToCustomResult()));
        }
    }

    public class StandardJsonResult<T> : StandardJsonResult, IStandardResult<T>
    {
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