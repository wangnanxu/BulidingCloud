using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Services.Unit.Dtos;
using ML.BC.Services.Common;
using ML.BC.EnterpriseData.Model;
namespace ML.BC.Services
{
   public class MaterialTypeManagementService:IMaterialTypeManagementService
    {
        public List<MaterialTypeDto> GetAllMaterialType()
        {
            try
            {
                using (var db = new BCEnterpriseContext())
                {
                    var query = db.MaterialTypes.Select(m => new MaterialTypeDto()
                    {
                        Avaliable = m.Available,
                        MaterialTypeID = m.MaterialTypeID,
                        Name = m.Name
                    });
                    return query.ToList();
                }

            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public List<MaterialTypeDto> GetAllMaterialType(string Name, int pageSize, int pageIndex, out int amount)
        {
            try
            {
                if (string.IsNullOrEmpty(Name))
                {
                    Name = "";
                }
                using (var db = new BCEnterpriseContext())
                {
                    var query = db.MaterialTypes.Where(obj => obj.Name.Contains(Name));
                    amount = query.Count();

                    int pageTotal;
                    if (pageSize > 0)
                    {
                        pageTotal = (amount + pageSize - 1) / pageSize;
                    }
                    else
                    {
                        pageSize = 10;
                        pageTotal = (amount + pageSize - 1) / pageSize;
                    }

                    if (pageIndex > pageTotal)
                        pageIndex = pageTotal;
                    if (pageIndex < 1)
                        pageIndex = 1;

                    return
                        query.OrderBy(obj => obj.MaterialTypeID)
                            .Skip(pageSize * (pageIndex - 1))
                            .Take(pageSize)
                            .Select(obj => new MaterialTypeDto()
                            {
                                Avaliable = obj.Available,
                                MaterialTypeID = obj.MaterialTypeID,
                                Name = obj.Name
                            }).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
       public bool Add(MaterialTypeDto model)
       {
           try
           {
               if ( string.IsNullOrEmpty(model.Name))
               {
                   throw new KnownException("信息不完善");
               }
               using (var db = new BCEnterpriseContext())
               {
                   if (db.MaterialTypes.Any(n => n.Name.Equals(model.Name)))
                   {
                       throw new KnownException("已存在该名字");
                   }
                   var profession = new ML.BC.EnterpriseData.Model.MaterialType()
                   {
                       
                       Name = model.Name,
                       Available = model.Avaliable
                   };
                   db.MaterialTypes.Add(profession);
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
       public bool Update(MaterialTypeDto model)
       {
           try
           {
               if (string.IsNullOrEmpty(model.Name))
               {
                   throw new KnownException("信息不完善");
               }
               using (var db = new BCEnterpriseContext())
               {
                   var temp = db.MaterialTypes.First(
                           x => x.MaterialTypeID == model.MaterialTypeID);
                   if (null == temp)
                       throw new KnownException("没有该记录！");
                   temp.Available = model.Avaliable;
                   temp.Name = model.Name;
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
       
       public bool Delete(int ID)
       {
           try
           {

               using (var db = new BCEnterpriseContext())
               {
                   var temp = db.MaterialTypes.FirstOrDefault(
                       obj => obj.MaterialTypeID.Equals(ID));
                   db.MaterialTypes.Attach(temp);
                   db.MaterialTypes.Remove(temp);
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
