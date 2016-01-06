using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ML.BC.BCBackWeb.Areas.System.Models;
using ML.BC.BCBackWeb.Model;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Web.Framework;
using System.IO;
using System.Configuration;
using ML.BC.Web.Framework.Security;
using ML.BC.BCBackData.Common;
using ML.BC.Web.Framework.Controllers;

namespace ML.BC.BCBackWeb.Areas.System.Controllers
{
    [Authorize]
    public class SystemLogController : BCControllerBase
    {
        /// <summary>
        /// 日志的根文件夹
        /// </summary>
        string logDir = ConfigurationManager.AppSettings["LogDir"];
        /// <summary>
        /// 后缀名
        /// </summary>
        string logSuffixname = ConfigurationManager.AppSettings["LogSuffixName"];
        string errorPath = ConfigurationManager.AppSettings["Log_Error"];
        string debugPath = ConfigurationManager.AppSettings["Log_Debug"];
        string infoPath = ConfigurationManager.AppSettings["Log_Info"];
        /// <summary>
        /// log的 物理路径
        /// </summary>
        string LogPhysicalPath = "";//log的 物理路径

        public SystemLogController()
        {
            if (string.IsNullOrEmpty(logDir))
            {
                logDir = "~/Log/";
            }
            else
            {
                logDir = logDir.Replace("\\", "/");
                logDir = logDir.TrimEnd('/') + "/";
            }

            if (string.IsNullOrEmpty(logSuffixname))
            {
                logSuffixname = ".log";
            }
            errorPath = string.IsNullOrEmpty(errorPath) ? "Error" : errorPath;
            debugPath = string.IsNullOrEmpty(debugPath) ? "Debug" : debugPath;
            infoPath = string.IsNullOrEmpty(infoPath) ? "Info" : infoPath;
        }
        //
        // GET: /System/SystemLog/
        [PermissionControlAttribute(Functions.Root_SysManagement_SysLogManagement)]
        public ActionResult SystemLogIndex()
        {
            return View();
        }
        [PermissionControlAttribute(Functions.Root_SysManagement_SysLogManagement)]
        public ActionResult GetSystemLogList(SystemLogViewModel model)
        {

            var result = new StandardJsonResult<DataGridResultModelBase<SystemLogResultModel>>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                LogPhysicalPath = Server.MapPath(logDir);
                int logSearchOption = 0; //搜索模式 0当前目录,1包括子目录
                int.TryParse(ConfigurationManager.AppSettings["LogSearchOption"], out logSearchOption);
                string LogsearchPattern = "*" + logSuffixname;
                SearchOption searopt = (SearchOption)logSearchOption;
                if (model.LogType == 0)
                {
                    searopt = SearchOption.AllDirectories;//获取全部
                }
                else
                {
                    LogPhysicalPath = PreHandlerLogDir(LogPhysicalPath, model.LogType);
                }
                var service = new FolderHelp();
                int count = 0;
                List<SystemLogResultModel> list = new List<SystemLogResultModel>();
                var filelist = service.GetFileList(LogPhysicalPath, LogsearchPattern, searopt, model.StartDate, model.EndDate, model.page, model.rows, out count);
                list = FileInfoToSystemLogResultModel(filelist);
                result.Value = new DataGridResultModelBase<SystemLogResultModel>();
                result.Value.total = count;//赋值
                result.Value.rows = list;
            });
            if (!result.Success)
            {
                result.Value = new DataGridResultModelBase<SystemLogResultModel>();
            }
            return new OringinalJsonResult<DataGridResultModelBase<SystemLogResultModel>> { Value = result.Value };
        }

          [PermissionControlAttribute(Functions.Root_SysManagement_SysLogManagement_ViewDetail)]
        public ActionResult GetSystemLogInfo(int LogType, string FileName)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }

                LogPhysicalPath = Server.MapPath(logDir);
                LogPhysicalPath = PreHandlerLogDir(LogPhysicalPath, LogType);

                string FilePath = LogPhysicalPath + FileName;
                 result.Value = GetSysLogStr(FilePath);

            });
            return Content(result.Value);
        }
          [PermissionControlAttribute(Functions.Root_SysManagement_SysLogManagement_ViewDetail)]
        public ActionResult GetSystemLogInfoByFullName(string FullName)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                result.Value = GetSysLogStr(FullName);
            });
           // string resultstr = "{\"Success\":{Success},\"Value\":\"{Value}\",\"Message\":\"{Message}\"}";
            // resultstr = resultstr.Replace("{Success}", result.Success.ToString().ToLower()).Replace("{Value}",result.Value).Replace("{Message}",result.Message);
            return Content(result.Value);

        }
          [PermissionControlAttribute(Functions.Root_SysManagement_SysLogManagement_Down)]
        public ActionResult SystemLogDown(string FullName)
        {

            if (!ModelState.IsValid)
            {
                throw new KnownException(ModelState.GetFirstError());
            }
            try
            {
                preHandlerDown(FullName);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
          
            return new EmptyResult();
        }
          [PermissionControlAttribute(Functions.Root_SysManagement_SysLogManagement_Down)]
     public ActionResult SystemLogDown2(int LogType, string FileName)
        {

            if (!ModelState.IsValid)
            {
                throw new KnownException(ModelState.GetFirstError());
            }
            try
            {
                LogPhysicalPath = Server.MapPath(logDir);
             LogPhysicalPath=   PreHandlerLogDir(LogPhysicalPath,LogType);
                string FilePath = LogPhysicalPath+ FileName;
                preHandlerDown(FilePath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new EmptyResult();
        }
          [PermissionControlAttribute(Functions.Root_SysManagement_SysLogManagement_Delete)]
     public ActionResult DeleteSystemLog(int LogType, string FileName)
        {
            var result = new StandardJsonResult();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                LogPhysicalPath = Server.MapPath(logDir);
                LogPhysicalPath = PreHandlerLogDir(LogPhysicalPath, LogType);
                string FilePath = LogPhysicalPath + FileName;

                FileHelp flp = new FileHelp();
                flp.DeleteFile(FilePath);
            });

            return result;
        }
     private string GetSysLogStr(string FullName)
     {
         FileHelp flp = new FileHelp();
         string str=flp.FileToStr(FullName);
         str = str.Replace("\r\n","<br/>");
         return str;
     }
     private void preHandlerDown(string FullName)
        {
            if (string.IsNullOrEmpty(FullName)) return;
            FileHelp flp = new FileHelp();
            MemoryStream ms = flp.FileToStream(FullName);
            Response.ContentType = "application/octet-stream";
            long length = 0;
            if (ms != null && ms.Length > 0)
            {
                length = ms.Length;
                Response.BinaryWrite(ms.ToArray());
            }
            else {
                Response.ContentType = "text/html";
                Response.Write("<script>alert('日志不存在,或者无内容!');window.opener = null;window.close();</script>");
                Response.End();
                return;
            }
            string fileNmae = "";
            // flp.GetFileNameforFullName(FullName);
            FileInfo fileinfo = flp.Getfile(FullName);
            if (fileinfo != null)
            {
                fileNmae = fileinfo.Name;
                if (fileinfo.Extension.ToLower() != ".log")
                {
                    fileNmae += ".log";
                }
            }
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileNmae + ";Content-Length=" + length);
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// 处理日志类型对应的目录
        /// </summary>
        /// <param name="LogType"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private string PreHandlerLogDir(string LogDir, int LogType)
        {
            if (string.IsNullOrEmpty(LogDir)) return "";
            LogDir = LogDir.Replace("\\", "/");
            LogDir = LogDir.TrimEnd('/') + "/";
            if (LogType == 1)
            {
                //错误信息
                LogDir = LogDir + errorPath;
            }
            else if (LogType == 2)
            {
                LogDir = LogDir + debugPath;
            }
            else if (LogType == 3)
            {
                LogDir = LogDir + infoPath;
            }
            LogDir = LogDir + "/";
            return LogDir;
        }

        private List<SystemLogResultModel> FileInfoToSystemLogResultModel(List<FileInfo> list)
        {
            List<SystemLogResultModel> resultlist = new List<SystemLogResultModel>();
            if (list == null || list.Count == 0)
            {
                return resultlist;
            }
            foreach (var item in list)
            {

                var M = new SystemLogResultModel() {
                    CreateTime = item.CreationTime,
                     LogType=GetLogTypeforFullName(item.FullName),
                    DownUrl = "",
                    FileName = item.Name,
                    FileSize = item.Length,
                    FullName = item.FullName,
                    UpdateTime = item.LastWriteTime
                };
                resultlist.Add(M);
            }
           resultlist= resultlist.OrderByDescending(m => m.UpdateTime).ToList();
            return resultlist;
        }
        private int GetLogTypeforFullName(string FullName)
        {
            int logtype = 0;
            FileInfo fle = new FileHelp().Getfile(FullName);
            string dirName = "";
            if (fle != null)
            {
                dirName = fle.Directory.Name;
                dirName = dirName.ToLower();
                if (dirName == errorPath.ToLower())
                {
                    logtype = 1;
                }
                else if (dirName == debugPath.ToLower())
                { 
                    logtype = 2;
                }   else if (dirName ==infoPath.ToLower())
                { 
                    logtype = 3;
                }
             }

            return logtype;
        }
    }
}
