using ML.BC.EnterpriseData.Model;
using ML.BC.EnterpriseData.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ML.BC.Services
{
    public class PersonStatisticsService : IPersonStatisticsService
    {
        public List<PersonStatisDto> GetPersonStatisInfo(string enterpriseId, string userName, DateTime startTime, DateTime endTime, int pageSize, int pageIndex, out int amount)
        {

            try
            {
                if (startTime == null) startTime = DateTime.MinValue;
                if (endTime == null) endTime = DateTime.MaxValue;
                if (userName == null) userName = "";
                var mgdb = new MongoDbProvider<SceneItem>();
                using (var db = new BCEnterpriseContext())
                {
                    var userQuery = db.FrontUsers.Where(u => u.EnterpiseID.Equals(enterpriseId) && !u.Closed && (userName.Equals("") || u.Name.Contains(userName)));
                    amount = userQuery.Count();
                    var userList = userQuery.OrderBy(u => u.UserID).Skip(pageSize * (pageIndex - 1)).Take(pageSize).Select(u => new { UserID = u.UserID, Name = u.Name }).ToList();
                    List<PersonStatisDto> result = new List<PersonStatisDto>();
                    foreach (var user in userList)
                    {
                        PersonStatisDto dto = new PersonStatisDto();
                        dto.userName = user.Name;
                        var sceneProjList = db.Scenes.Where(s => s.EnterpriseID.Equals(enterpriseId) && !s.Deleted && s.Woker.Contains(user.UserID) && startTime <= s.RegistDate && s.RegistDate <= endTime).Select(s => s.ProjectID);
                        dto.projCount = db.Projects.Where(p => p.EnterpriseID.Equals(enterpriseId) && !p.Deleted && (p.Managers.Contains(user.UserID) || sceneProjList.Contains(p.ProjectID)) && startTime <= p.RegistDate && p.RegistDate <= endTime).Count();
                        dto.sceneCount = sceneProjList.Count();
                        var imageCountTemp = mgdb.GetAll(si => si.UserID.Equals(user.UserID) && startTime <= si.CreateTime && si.CreateTime <= endTime).Select(si => new { count = si.Count, @byte = si.TotalOrgImageBytes });
                        dto.imageCount = imageCountTemp.Select(t => t.count).Sum();
                        dto.imageSize = imageCountTemp.Select(t => t.@byte).Sum();
                        result.Add(dto);
                    }
                    return result;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public PersonStatisDto GetPersonStatisSummaryInfo(string enterpriseId)
        {
            try
            {
                var mgdb = new MongoDbProvider<SceneItem>();
                using (var db = new BCEnterpriseContext())
                {
                    PersonStatisDto dto = new PersonStatisDto();
                    var userIdQuery = db.FrontUsers.Where(u => u.EnterpiseID.Equals(enterpriseId) && !u.Closed).Select(u => u.UserID).ToList();
                    dto.userName = "合计" + userIdQuery.Count() + "个用户";
                    dto.sceneCount = db.Scenes.Where(s => s.EnterpriseID.Equals(enterpriseId) && !s.Deleted).Count();
                    dto.projCount = db.Projects.Where(p => p.EnterpriseID.Equals(enterpriseId) && !p.Deleted).Count();
                    var imageCountTemp = mgdb.GetAll(si => userIdQuery.Contains(si.UserID)).Select(si => new { count = si.Count, @byte = si.TotalOrgImageBytes });
                    dto.imageCount = imageCountTemp.Select(t => t.count).Sum();
                    dto.imageSize = imageCountTemp.Select(t => t.@byte).Sum();
                    return dto;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
