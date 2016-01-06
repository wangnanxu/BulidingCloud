using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services
{
    public interface IKnowledgaeList
    {
        List<KnowlegeDto> GetKnowlegeList(string name, EnterpriseData.Common.FileType filetype, int? knowlegeType, string enterpriseID, int pageSize, int pageNumber, out int amount);
        bool UpdateFileType(string IDName, int knowledgeType,string fileName);
        bool DeleteFile(string fileName);
        bool UpLoading(KnowlegeDto newFile);
        byte[] DownLoading(string fileName);
        string MakeUrlWithFileName(string fileName);
        List<KnowlegeDto> GetAllKnowlege(string enterpriseID);
    }
}
