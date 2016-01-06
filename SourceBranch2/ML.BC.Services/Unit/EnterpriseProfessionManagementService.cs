using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Services.Unit.Dtos;

namespace ML.BC.Services
{
    public class EnterpriseProfessionManagementService : IEnterpriseProfessionManagementService
    {
        public List<EnterpriseProfessionDto> GetEnterpriseProfessionList(string nameKeyword, int pageSize, int pageIndex, out int count)
        {
            try
            {
                if (string.IsNullOrEmpty(nameKeyword))
                {
                    nameKeyword = "";
                }
                using (var db = new BCEnterpriseContext())
                {
                    var query = db.EnterpriseProfessions.Where(obj => obj.Name.Contains(nameKeyword));
                    count = query.Count();

                    int pageTotal;
                    if (pageSize > 0)
                    {
                        pageTotal = (count + pageSize - 1) / pageSize;
                    }
                    else
                    {
                        pageSize = 10;
                        pageTotal = (count + pageSize - 1) / pageSize;
                    }

                    if (pageIndex > pageTotal)
                        pageIndex = pageTotal;
                    if (pageIndex < 1)
                        pageIndex = 1;

                    return
                        query.OrderBy(obj => obj.EnterpriseProfessionID)
                            .Skip(pageSize * (pageIndex - 1))
                            .Take(pageSize)
                            .Select(obj => new EnterpriseProfessionDto()
                            {
                                Available = obj.Available,
                                EnterpriseProfessionID = obj.EnterpriseProfessionID,
                                Name = obj.Name
                            }).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<EnterpriseProfessionDto> GetAllEnterpriseProfession()
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var query = db.EnterpriseProfessions.Where(o => o.Available).Select(obj => new EnterpriseProfessionDto()
                    {
                        Available = obj.Available,
                        EnterpriseProfessionID = obj.EnterpriseProfessionID,
                        Name = obj.Name
                    });

                    return query.ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<EnterpriseProfessionDto> GetAllEnterpriseProfession(int pageSize, int pageIndex, out int count)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var query = db.EnterpriseProfessions;

                    count = query.Count();
                    int pageTotal;
                    if (pageSize > 0)
                    {
                        pageTotal = (count + pageSize - 1) / pageSize;
                    }
                    else
                    {
                        pageSize = 10;
                        pageTotal = (count + pageSize - 1) / pageSize;
                    }

                    if (pageIndex > pageTotal)
                        pageIndex = pageTotal;
                    if (pageIndex < 1)
                        pageIndex = 1;

                    return
                        query.OrderBy(obj => obj.EnterpriseProfessionID)
                            .Skip(pageSize * (pageIndex - 1))
                            .Take(pageSize)
                            .Select(obj => new EnterpriseProfessionDto()
                            {
                                Available = obj.Available,
                                EnterpriseProfessionID = obj.EnterpriseProfessionID,
                                Name = obj.Name
                            }).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string AddEnterpriseProfession(string enterpriseProfessionKey, string enterpriseProfessionName, bool available)
        {
            try
            {
                if (string.IsNullOrEmpty(enterpriseProfessionKey) ||
                           string.IsNullOrEmpty(enterpriseProfessionName))
                {
                    throw new KnownException("信息不完善");
                }
                using (var db = new BCEnterpriseContext())
                {
                    if (db.EnterpriseProfessions.Any(n => n.Name.Equals(enterpriseProfessionName)))
                    {
                        throw new KnownException("已存在该名字");
                    }
                    var profession = new ML.BC.EnterpriseData.Model.EnterpriseProfession()
                    {
                        EnterpriseProfessionID = enterpriseProfessionKey,
                        Name = enterpriseProfessionName,
                        Available = available,
                        UpdateTime = DateTime.Now
                    };
                    db.EnterpriseProfessions.Add(profession);
                    if (db.SaveChanges() > 0)
                    {
                        return profession.EnterpriseProfessionID;
                    }
                    else
                    {
                        return null;
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public bool DeleteEnterpriseProfession(string enterpriseProfessionId)
        {
            try
            {
                if (string.IsNullOrEmpty(enterpriseProfessionId))
                {
                    throw new KnownException("输入Id不能为空！");
                }
                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.EnterpriseProfessions.FirstOrDefault(
                        obj => obj.EnterpriseProfessionID.Equals(enterpriseProfessionId));
                    db.EnterpriseProfessions.Attach(temp);
                    db.EnterpriseProfessions.Remove(temp);
                    if (db.SaveChanges() > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool UpdateEnterpriseProfession(string enterpriseProfessionKey, string enterpriseProfessionName, bool available)
        {
            try
            {
                if (string.IsNullOrEmpty(enterpriseProfessionKey) ||
                           string.IsNullOrEmpty(enterpriseProfessionName))
                {
                    throw new KnownException("信息不完善");
                }
                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.EnterpriseProfessions.First(
                            x => x.EnterpriseProfessionID == enterpriseProfessionKey);
                    if (null == temp)
                        throw new KnownException("没有该记录！");
                    temp.Available = available;
                    temp.Name = enterpriseProfessionName;
                    if (db.SaveChanges() > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
