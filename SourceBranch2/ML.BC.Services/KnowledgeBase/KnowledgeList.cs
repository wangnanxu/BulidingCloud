using System;
using System.Collections.Generic;
using System.Configuration;
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
        public bool UpdateFileType(string IDName, int knowledgeType, string fileName)
        {
            try
            {
                if (null == IDName)
                    return false;
                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.KnowledgeBase.FirstOrDefault(x => x.ID.Equals(IDName));
                    if (null != temp)
                    {
                        temp.KnowledgeType = knowledgeType;
                        temp.Name = fileName;
                        temp.UpdateTime = DBTimeHelper.DBNowTime();
                    }
                    return 0 < db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool DeleteFile(string fileName)
        {
            try
            {
                if (null == fileName)
                    return false;
                var mgdb = new MongoDbProvider<KnowledgeBaseFile>();
                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.KnowledgeBase.FirstOrDefault(x => x.ID.Equals(fileName));
                    if (null != temp)
                    {
                        temp.Deleted = true;
                        mgdb.DeleteFileByName(fileName, KnowlegeDbName);
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

        public byte[] DownLoading(string fileName)
        {
            try
            {
                var mgdb = new MongoDbProvider<KnowledgeBaseFile>();
                return mgdb.GetFileAsStream(fileName, KnowlegeDbName);
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

                    return list.OrderByDescending(x => x.UpdateTime).Skip(pageSize * (pageNumber - 1)).Take(pageSize).Select(n => new KnowlegeDto()
                    {
                        ID = n.ID,
                        Name = n.Name,
                        EnterpriseID = n.EnterpriseID,
                        Deleted = n.Deleted,
                        DocumentSize = n.DocumentSize,
                        DocumentType = (EnterpriseData.Common.FileType)n.DocumentType,
                        KnowledgeType = n.KnowledgeType,
                        UpdateTime = n.UpdateTime
                    }).ToList();
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
                        UpdateTime = n.UpdateTime
                    }).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
