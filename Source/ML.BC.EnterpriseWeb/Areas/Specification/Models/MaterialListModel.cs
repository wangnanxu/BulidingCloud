using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ML.BC.EnterpriseData.Common;
using ML.BC.Services;
using System.IO;
namespace ML.BC.EnterpriseWeb.Areas.Specification.Models
{
    public class MaterialListModel
    {
        public string fileID { get; set; }
        public string fileName { get; set; }
        public string fileSize { get; set; }
        public string updateTime { get; set; }
        public string materialType { get; set; }
        public string materialTypeName { get; set; }
        public string fileTypeName { get; set; }
        public FileType fileType { get; set; }
        public Guid guid { get; set; }
        
        static public implicit operator MaterialListModel(KnowlegeDto dto)
        {
            if (dto == null)
            {
                return new MaterialListModel();
            }
            else {
                return new MaterialListModel() { 
                 fileID=dto.ID,
                 fileName=dto.Name,
                 fileSize=dto.FileAllSize.ToString(),
                 fileType=dto.DocumentType,
                 materialType=dto.KnowledgeType.ToString(),
                 updateTime=dto.UpdateTime.ToString(),
                 guid=dto.FileGUID
                };
            }
        }
    }
    public class MaterialListResult
    {
        public List<MaterialListModel> rows { get; set; }
        public int total { get; set; }
      
    }
    public class MaterialPara
    {
       public int rows { get; set; }
      public int page { get; set; }
       public string name { get; set; }
       public string fileType { get; set; }
       public int? materialType { get; set; }
    }
    public class FileTypeModel
    {
        public FileType value { get; set; }
        public string text { get; set; }
    }
    public class MaterialTypeModel
    {
        public int value { get; set; }
        public string text { get; set; }
    }
    public class AddMaterialListModel
    {
        public int materialType { get; set; }
        public string fileName { get; set; }
        public int chunk { get; set; }
        public int chunks { get; set; }
        public bool isChunk { get; set; }
        public int byteLength { get; set; }
        public string ext { get; set; }
        public int chunkSize { get; set; }
        public Guid guid { get; set; }
    }
    public class FileList
    {
        public byte[] filebt { get; set; }
        public int chunk { get; set; }
        public int start { get; set; }
        public int end { get; set; }
    }
}