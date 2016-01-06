using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using ML.BC.EnterpriseData.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model.Extend;
using ML.BC.EnterpriseData.MongoDb;
using ML.BC.Infrastructure.Exceptions;

namespace ML.BC.Services
{
    public class KnowledgeList : IKnowledgaeList, IServiceBase
    {
        private const string KnowlegeDbName = "KnowledgeBase";
        public bool UpdateFileType(string Name, int knowledgeType, Guid fileGuid)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var list = db.KnowledgeBase.Where(x => x.FileGUID == fileGuid);
                    if (list.Count() > 0)
                    {
                        foreach (var temp in list)
                        {
                            temp.KnowledgeType = knowledgeType;
                            temp.Name = Name;
                            temp.UpdateTime = DBTimeHelper.DBNowTime();
                        }
                    }
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool DeleteFile(Guid fileGuid)
        {
            try
            {
                if (null == fileGuid)
                    return false;
                var mgdb = new MongoDbProvider<KnowledgeBaseFile>();
                using (var db = new BCEnterpriseContext())
                {
                    var list = db.KnowledgeBase.Where(x => x.FileGUID == fileGuid);
                    if (list.Count() > 0)
                    {
                        foreach (var file in list)
                        {
                            file.Deleted = true;
                            mgdb.DeleteFileByName(file.ID, KnowlegeDbName);
                        }
                    }
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string MakeUrlWithFileName(string fileName)
        {
            if (null == fileName) return null;
            var mgdb = new MongoDbProvider<KnowledgeBaseFile>();
            using (var db = new BCEnterpriseContext())
            {
                var fileGuid = db.KnowledgeBase.First(x => x.ID.Equals(fileName)).FileGUID;
                var fileNameList = db.KnowledgeBase.Where(x => x.FileGUID == fileGuid && !x.Deleted).OrderBy(n => n.FileNumber);
                var newobj = fileNameList.First();
                if (1 < fileNameList.Count())
                {
                    byte[] filearry = new byte[Convert.ToInt64(fileNameList.First().FileAllSize)];
                    byte[] tempobj = null;
                    double flag = 0;
                    foreach (var file in fileNameList)
                    {
                        tempobj = mgdb.GetFileAsStream(file.ID, KnowlegeDbName);
                        if (0 != tempobj.Length)
                        {                           
                            tempobj.CopyTo(filearry, Convert.ToInt64(flag));
                            flag = flag + file.DocumentSize;
                        }
                    }
                    if (filearry.Length > 0)
                    {
                        Stream stream = new MemoryStream(filearry);
                        var newFileName = fileName.Replace(fileName.Split('.')[0], DateTime.Now.ToFileTime().ToString());
                        mgdb.SaveFileByStream(stream, newFileName, KnowlegeDbName);
                        var temp = new KnowledgeBase();
                        temp.ID = newFileName;
                        temp.Name = newobj.Name;
                        temp.EnterpriseID = newobj.EnterpriseID;
                        temp.Deleted = newobj.Deleted;
                        temp.DocumentSize = newobj.FileAllSize;
                        temp.DocumentType = (int)newobj.DocumentType;
                        temp.KnowledgeType = newobj.KnowledgeType;
                        temp.UpdateTime = newobj.UpdateTime;
                        temp.FileGUID = newobj.FileGUID;
                        temp.FileNumber = newobj.FileNumber;
                        temp.FileAllSize = newobj.FileAllSize;
                        db.KnowledgeBase.Add(temp);
                        foreach (var file in fileNameList)
                        {
                            mgdb.DeleteFileByName(file.ID, KnowlegeDbName);
                            db.KnowledgeBase.Remove(file);
                        }
                        fileName = newFileName;
                    }
                }
            }
            var baseUrl = ConfigurationManager.ConnectionStrings["FileBaseUrlByName"].ToString();
            return baseUrl + fileName;
        }

        public bool UpLoading(KnowlegeDto newFile)
        {
            try
            {
                var mgdb = new MongoDbProvider<KnowledgeBaseFile>();
                using (var db = new BCEnterpriseContext())
                {
                    var temp = new KnowledgeBase();
                    temp.ID = newFile.ID;
                    temp.Name = newFile.Name;
                    temp.EnterpriseID = newFile.EnterpriseID;
                    temp.Deleted = newFile.Deleted;
                    temp.DocumentSize = newFile.DocumentSize;
                    temp.DocumentType = (int)newFile.DocumentType;
                    temp.KnowledgeType = newFile.KnowledgeType;
                    temp.UpdateTime = newFile.UpdateTime;
                    temp.FileGUID = newFile.FileGUID;
                    temp.FileNumber = newFile.FileNumber;
                    temp.FileAllSize = newFile.FileAllSize;
                    db.KnowledgeBase.Add(temp);
                    mgdb.SaveFileByStream(newFile.FileStream, newFile.ID, KnowlegeDbName);
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public byte[] DownLoading(Guid fileGuid)
        {
            try
            {
                var mgdb = new MongoDbProvider<KnowledgeBaseFile>();
                using (var db = new BCEnterpriseContext())
                {
                    var filelist = db.KnowledgeBase.Where(x => x.FileGUID == fileGuid && !x.Deleted).OrderBy(n => n.FileNumber).ToList();
                    if (!(filelist.Count()>0))
                    {
                        throw new Exception("没有在数据库中找到该Guid");
                    }
                    byte[] filearry = new byte[Convert.ToInt32(filelist.First().FileAllSize)];
                    byte[] temp = null;
                    double flag = 0;
                    foreach (var file in filelist)
                    {
                        temp = mgdb.GetFileAsStream(file.ID, KnowlegeDbName);
                        temp.CopyTo(filearry,Convert.ToInt64(flag));
                        flag = flag + file.DocumentSize;
                    }
                    return filearry;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<KnowlegeDto> GetKnowlegeList(string name, EnterpriseData.Common.FileType filetype, int? knowlegeType, string enterpriseID, int pageSize, int pageNumber, out int amount)
        {
            try
            {
                if (string.IsNullOrEmpty(name)) name = "";
                using (var db = new BCEnterpriseContext())
                {
                    IQueryable<KnowledgeBase> list;
                    if (filetype == EnterpriseData.Common.FileType.All)
                    {
                        list = db.KnowledgeBase.Where(x =>
                                 !x.Deleted && x.Name.Contains(name) && x.EnterpriseID.Equals(enterpriseID) &&
                                 (knowlegeType ?? x.KnowledgeType) == x.KnowledgeType);
                    }
                    else
                    {
                        list =
                            db.KnowledgeBase.Where(x =>
                                    !x.Deleted && x.Name.Contains(name) && x.EnterpriseID.Equals(enterpriseID) &&
                                    (knowlegeType ?? x.KnowledgeType) == x.KnowledgeType &&
                                    (int)filetype == x.DocumentType);
                    }
                    int pagecount;
                    amount = list.Count();
                    if (pageSize > 0)
                    {
                        pagecount = (list.Count() + pageSize - 1) / pageSize;
                    }
                    else
                    {
                        pagecount = 0;
                    }
                    if (pageNumber > pagecount)
                        pageNumber = pagecount;
                    if (pageNumber < 1)
                        pageNumber = 1;

                    var retlist= list.Select(n => new KnowlegeDto()
                    {
                        ID = n.ID,
                        Name = n.Name,
                        EnterpriseID = n.EnterpriseID,
                        Deleted = n.Deleted,
                        DocumentSize = n.DocumentSize,
                        DocumentType = (EnterpriseData.Common.FileType)n.DocumentType,
                        KnowledgeType = n.KnowledgeType,
                        UpdateTime = n.UpdateTime,
                        FileAllSize = n.FileAllSize,
                        FileGUID = n.FileGUID,
                        FileNumber = n.FileNumber
                    }).ToList().Distinct(new DistinctByFileGuid()).OrderByDescending(x => x.UpdateTime).ToList();
                    amount = retlist.Count;

                    return retlist.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<KnowlegeDto> GetAllKnowlege(string enterpriseID)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    return db.KnowledgeBase.Where(x => x.EnterpriseID.Equals(enterpriseID ?? "") && !x.Deleted).Select(n => new KnowlegeDto()
                    {
                        ID = n.ID,
                        Name = n.Name,
                        EnterpriseID = n.EnterpriseID,
                        Deleted = n.Deleted,
                        DocumentSize = n.DocumentSize,
                        DocumentType = (EnterpriseData.Common.FileType)n.DocumentType,
                        KnowledgeType = n.KnowledgeType,
                        UpdateTime = n.UpdateTime,
                        FileAllSize = n.FileAllSize,
                        FileGUID = n.FileGUID,
                        FileNumber = n.FileNumber
                    }).ToList().Distinct(new DistinctByFileGuid()).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
