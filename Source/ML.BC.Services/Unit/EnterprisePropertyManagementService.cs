using ML.BC.EnterpriseData.Model;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Services.Unit.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ML.BC.Services
{
  public  class EnterprisePropertyManagementService : IEnterprisePropertyManagementService
    {
        public List<EnterprisePropertyDto> SearchEnterprisePropertyByName(string nameKeyword, int pageSize, int pageNumber, out int amountx)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    if(string.IsNullOrEmpty(nameKeyword))
                    {
                        nameKeyword = "";
                    }
                    var list = db.EnterprisePropertys.Where(obj => obj.Name.Contains(nameKeyword));
                    amountx = list.Count();
                    int count;
                    if (pageSize > 0)
                    {
                        // 获取总共页数
                        count = (list.Count() + pageSize - 1) / pageSize;
                    }
                    else
                    {
                        count = 0;
                    }
                    //页码判断，小于1则为1，大于最大页码则为最大页码
                    if (pageNumber > count)
                        pageNumber = count;
                    if (pageNumber < 1)
                        pageNumber = 1;
                    return list.OrderBy(x=>x.EnterprisePropertyID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).Select(obj => new EnterprisePropertyDto
                    {
                        Available = obj.Available,
                        EnterprisePropertyID = obj.EnterprisePropertyID,
                        Name = obj.Name
                    }).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<EnterprisePropertyDto> GetAllEnterpriseProperty(int pageSize, int pageNumber, out int amount)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var list = db.EnterprisePropertys.Select(obj => new EnterprisePropertyDto
                    {
                        Available = obj.Available,
                        EnterprisePropertyID = obj.EnterprisePropertyID,
                        Name = obj.Name
                    });
                    amount = list.Count();
                    int count;
                    if (pageSize > 0)
                    {
                        // 获取总共页数
                        count = (list.Count() + pageSize - 1) / pageSize;
                    }
                    else
                    {
                        count = 0;
                    }
                    //页码判断，小于1则为1，大于最大页码则为最大页码
                    if (pageNumber < 1)
                        pageNumber = 1;
                    if (pageNumber > count)
                        pageNumber = count;

                    return list.OrderBy(x=>x.EnterprisePropertyID).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string AddEnterpriseProperty(EnterprisePropertyDto enterpriseProperty)
        {
           try
           {            
               using (var db = new BCEnterpriseContext())
            {
                if (db.EnterprisePropertys.Any(n => n.Name.Equals(enterpriseProperty.Name)))
                {
                    throw new KnownException("已存在该名字");
                }
                if (string.IsNullOrEmpty(enterpriseProperty.Name)||string.IsNullOrEmpty(enterpriseProperty.EnterprisePropertyID))
                {
                    throw new KnownException("信息不完善");
                }

                var property = new ML.BC.EnterpriseData.Model.EnterpriseProperty
                {
                    Available = enterpriseProperty.Available,
                    EnterprisePropertyID = enterpriseProperty.EnterprisePropertyID,
                    Name = enterpriseProperty.Name
                };
                db.EnterprisePropertys.Add(property);
                if (db.SaveChanges() > 0)
                {
                    return property.EnterprisePropertyID;
                }
                else
                {
                    return null;
                }
            }

           }
            catch(Exception e)
           {
               throw e;
           }

        }

        public bool DeleteEnterpriseProperty(string enterprisePropertyId)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    ML.BC.EnterpriseData.Model.EnterpriseProperty temp = db.EnterprisePropertys.FirstOrDefault(obj => obj.EnterprisePropertyID.Equals(enterprisePropertyId));
                    db.EnterprisePropertys.Attach(temp);
                    db.EnterprisePropertys.Remove(temp);
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

        public bool UpdateEntProperty(EnterprisePropertyDto enterpriseProperty)
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var temp = db.EnterprisePropertys.First(x => x.EnterprisePropertyID == enterpriseProperty.EnterprisePropertyID);
                    if (null == temp)
                        throw new KnownException("该对象不存在");
                    temp.Available = enterpriseProperty.Available;
                    temp.Name = enterpriseProperty.Name;
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


        public List<EnterprisePropertyDto> GetAllEnterpriseProperty()
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    return db.EnterprisePropertys.Where(x=>x.Available).Select(obj => new EnterprisePropertyDto
                    {
                        Available = obj.Available,
                        EnterprisePropertyID = obj.EnterprisePropertyID,
                        Name = obj.Name
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
