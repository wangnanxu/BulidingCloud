using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ML.BC.Web.Framework
{
    /// <summary>
    /// 文件操作帮助类
    /// </summary>
    public class FileHelp
    {
        public FileHelp()
        {
        }

        /// <summary>
        /// 检查文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool CheckFile(string path)
        {
            if (!File.Exists(path))
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
        public FileInfo Getfile(string path)
        {
            FileInfo file = null;
            if (!CheckFile(path))
            {
                return file;
            }
            file = new FileInfo(path);
            return file;
        }

        /// <summary>
        /// 读取文件到内存流 适合操作小文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public MemoryStream FileToStream(string path)
        {
            MemoryStream ms = new MemoryStream();
            if (CheckFile(path))
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (fs)
                {
                    int buffsLength = 1024;
                    if (fs.CanRead)
                    {
                        byte[] buffs = new byte[buffsLength];
                        int count = 0;
                        while ((count = fs.Read(buffs, 0, buffsLength)) > 0)
                        {
                            ms.Write(buffs, 0, count);
                        }
                    }
                    fs.Close();
                }
            }
            return ms;
        }
        public bool DeleteFile(string path)
        {
            bool f = false;

            try
            {
                if (CheckFile(path))
                {
                   // FileStream fs = new FileStream(path, FileMode.Truncate, FileAccess.ReadWrite, FileShare.ReadWrite);
                  //  fs.Dispose(); fs.Close();
                    File.Delete(path);
                    f = true;
                }
                else
                {
                    f = false;
                }
            }
            catch (Exception ex)
            {
                f = false;
                throw ex;
            }
            return f;
        }

        /// <summary>
        /// 读取文件类容为字符串
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="fileName">返回文件名称</param>
        /// <returns></returns>
        public string FileToStr(string path, out string fileName)
        {
            fileName = "";
            FileInfo fileinfo = this.Getfile(path);
            string strAll = string.Empty;
            if (fileinfo != null)
            {
                fileName = fileinfo.Name;
                using (StreamReader stread = new StreamReader(path, Encoding.Default))
                {
                    strAll = stread.ReadToEnd();
                }
            }
            return strAll;
        }
        /// <summary>
        /// 读取文件类容为字符串 FileStream 方式
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public string FileToStr(string path)
        {
            string strAll = string.Empty;
            if (CheckFile(path))
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (fs)
                {
                    int buffsLength = 1024;
                    if (fs.CanRead)
                    {
                        MemoryStream ms = new MemoryStream();
                        using (ms)
                        {
                            byte[] buffs = new byte[buffsLength];
                            int count = 0;
                            while ((count = fs.Read(buffs, 0, buffsLength)) > 0)
                            {
                                ms.Write(buffs, 0, count);
                            }
                        }
                        strAll = Encoding.Default.GetString(ms.ToArray());
                    }
                    fs.Close();
                }
            }
            return strAll;
        }

        /// <summary>
        /// 写入内容到文件
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="fileName">文件名</param>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public bool FileWrite(string str, string fileName, string path)
        {
            bool flag = false;
            try
            {
                if (string.IsNullOrEmpty(path)) return flag;
                if (string.IsNullOrEmpty(str)) return flag;
                path = path.Replace("\\", "/").Replace("//", "/").TrimEnd('/');
                if (!CheckFile(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filepath = path + "/" + fileName;
                using (FileStream filestream = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    byte[] bytes = Encoding.Default.GetBytes(str);
                    filestream.Write(bytes, 0, bytes.Length);
                    filestream.Flush();
                    filestream.Close();
                }


                flag = true;
            }
            catch (Exception )
            {
                throw;
            }
            return flag;
        }
        /// <summary>
        /// 获取文件名 根据路径
        /// </summary>
        /// <param name="fullNmae">文件路径</param>
        /// <returns></returns>
        public string GetFileNameforFullName(string fullNmae)
        {
            string fileName = "";
            try
            {
                if (string.IsNullOrEmpty(fullNmae))
                {
                    return fileName;
                }
                fullNmae = fullNmae.Replace("//", "/").Replace("\\", "/");
                int index = fullNmae.LastIndexOf("/");
                if (index == fullNmae.Length - 1)
                {
                    return fileName;
                }
                fileName = fullNmae.Substring(index + 1);
            }
            catch (Exception)
            {
                throw;
            }
            return fileName;
        }
    }
}
