using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
namespace ML.BC.Web.Framework
{
    /// <summary>
    /// 文件夹操作帮助类
    /// </summary>
    public class FolderHelp
    {
        public FolderHelp()
        {
        }

        /// <summary>
        /// 检查文件夹是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool CheckFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取一个文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DirectoryInfo GetDirectory(string path)
        {
            DirectoryInfo dirinfo = null;
            if (!CheckFolder(path))
            {
                return dirinfo;
            }
            dirinfo = new DirectoryInfo(path);
            return dirinfo;
        }
        /// <summary>
        /// 获取文件夹下的所有文件
        /// </summary>
        /// <param name="path">文件夹</param>
        /// <param name="searchPattern">文件搜索字符串:*.txt</param>
        /// <param name="searopt">目录搜索模式</param>
        /// <returns></returns>
        public List<FileInfo> GetFileList(string path, string searchPattern, SearchOption searopt)
        {
            List<FileInfo> list = new List<FileInfo>();
            if (!CheckFolder(path))
            {
                return list;
            }
            try
            {
                if (string.IsNullOrEmpty(searchPattern))
                {
                    searchPattern = "*.*";
                }
                DirectoryInfo dirinfo = new DirectoryInfo(path);
                var files = dirinfo.GetFiles(searchPattern, searopt);
                if (files != null && files.Length > 0)
                {
                    list.AddRange(files);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return list;

        }
         /// <summary>
        /// 分页获取文件列表
        /// </summary>
        /// <param name="path">文件夹</param>
        /// <param name="searchPattern">文件搜索字符串:*.txt</param>
        /// <param name="searopt">目录搜索模式</param>
        /// <param name="PageIndex">页索引</param>
        /// <param name="PageSize">每页大小</param>
        /// <param name="PageSize">输出的总页数</param>
        /// <returns></returns>
        public List<FileInfo> GetFileList(string path, string searchPattern, SearchOption searopt,DateTime? StartDate,DateTime? EndDate, int PageIndex, int PageSize, out int amount)
        {
            List<FileInfo> resultlist = new List<FileInfo>();
            List<FileInfo> Alllist = GetFileList(path, searchPattern, searopt);
            amount = 0;
            if (Alllist != null && Alllist.Count > 0)
            {
                if (StartDate != null)
                {
                    Alllist = Alllist.Where(m => m.CreationTime >= StartDate.Value).ToList();
                }
                if (EndDate != null)
                {
                    if (Alllist != null && Alllist.Count > 0)
                    {
                        Alllist = Alllist.Where(m => m.CreationTime <= EndDate.Value).ToList();
                    }
                }
                if (Alllist != null && Alllist.Count > 0)
                {
                    amount = Alllist.Count;
                }
                int startindex = 0;
                int pageCount = 1;
                PageSize = PageSize <= 0 ? 1 : PageSize;
                pageCount = (amount + PageSize - 1) / PageSize;

                //页码判断，小于1则为1，大于最大页码则为最大页码
                if (PageIndex > pageCount)
                    PageIndex = pageCount;
                if (PageIndex < 1)
                    PageIndex = 1;
                startindex = PageSize * (PageIndex - 1);

                var tmplist = Alllist.Skip(startindex).Take(PageSize);
              int cc=  tmplist.Count();
                int cc2=tmplist.ToList().Count;
                if (tmplist != null && tmplist.ToList().Count > 0)
                {
                    resultlist.AddRange(tmplist);
                }

            }
            return resultlist;

        }
    }
}
