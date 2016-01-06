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
        bool UpdateFileType(string Name, int knowledgeType, Guid fileGuid);
        bool DeleteFile(Guid fileGuid);
        bool UpLoading(KnowlegeDto newFile);
        byte[] DownLoading(Guid fileName);
        string MakeUrlWithFileName(string fileName);
        List<KnowlegeDto> GetAllKnowlege(string enterpriseID);
    }
}
