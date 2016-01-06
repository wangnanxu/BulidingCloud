using ML.BC.BCBackData.Model;
using ML.BC.Services.Account.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;
namespace ML.BC.Services
{
    public class FunctionManagementService : IFunctionManagementService
    {
        public List<FunctionDto> GetAllFunctions(string nameKeyword, int psize, int pageidx)
        {
            try
            {
                using (var db = new BCBackContext())
                {
                    if (psize <= 0 || pageidx <= 0) return new List<FunctionDto>();
                    IEnumerable<RFAFunction> tResult;
                    if (nameKeyword==null||nameKeyword.Equals(""))
                    { tResult = db.RFAFunctions; }
                    else
                    { tResult = db.RFAFunctions.Where(obj => obj.Name.Contains(nameKeyword)); }
                    //tResult.c
                    int count = tResult.Count();
                    int pageCount = (count + psize - 1) / psize;
                    if (pageCount < pageidx)
                    {
                        //if pageidx is overflow return the last page
                        return tResult.OrderBy(x=>x.FunctionID).Skip((pageCount - 1) * psize).Select(n => new FunctionDto
                        {
                            FunctionID = n.FunctionID,
                            MyID = n.MyID,
                            ParentID = n.ParentID,
                            Name = n.Name,
                            Desription = n.Desription,
                            Available = n.Available
                        }).ToList(); ;
                    }
                    else
                    {
                        //return tResult.
                        return tResult.OrderBy(x=>x.FunctionID).Skip((pageidx - 1) * psize).Take(psize).Select(n => new FunctionDto
                        {
                            FunctionID = n.FunctionID,
                            MyID = n.MyID,
                            ParentID = n.ParentID,
                            Name = n.Name,
                            Desription = n.Desription,
                            Available = n.Available
                        }).ToList(); ;
                    }
                    //var list = db.RFAFunctions.Where(lambda).AsEnumerable().Select(n => new FunctionDto
                    //{
                    //    FunctionID = n.FunctionID,
                    //    MyID = n.MyID,
                    //    ParentID = n.ParentID,
                    //    Name = n.Name,
                    //    Desription = n.Desription,
                    //    Available = n.Available
                    //}).ToList();                            
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<FunctionDto> GetAllFunctions()
        {

            try
            {
                using (var db = new BCBackContext())
                {
                    var list = db.RFAFunctions.Select(n => new FunctionDto
                    {
                        FunctionID = n.FunctionID,
                        MyID = n.MyID,
                        ParentID = n.ParentID,
                        Name = n.Name,
                        Desription = n.Desription,
                        Available = n.Available
                    }).ToList();
                    return list;     
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //public List<RFAFunction> Select(Expression<Func<RFAFunction, bool>> foo)
        //{
        //    using (var db = new BCBackContext())
        //    {
        //        List<RFAFunction> list = new List<RFAFunction>();
        //        list = db.RFAFunctions.Where(foo).ToList<RFAFunction>();
        //        return list;
        //    }              
        //}

        public int Update(FunctionDto obj)
        {
            using (var db = new BCBackContext())
            {
                RFAFunction temp = db.RFAFunctions.Where(objx => objx.FunctionID.Equals(obj.FunctionID)).First();
                if (temp == null) return 0;
                //temp.FunctionID = obj.FunctionID;
                temp.MyID = obj.MyID;
                temp.Name = obj.Name;
                //temp.ParentID = obj.ParentID;
                temp.Available = obj.Available;
                temp.Desription = obj.Desription;

                return db.SaveChanges();
            }
        }

        public bool Delete(string FunctionID)
        {
            using (var db = new BCBackContext())
            {
                return 0 < DeleteFunctionsons(db, FunctionID);
                //RFAFunction temp = db.RFAFunctions.FirstOrDefault(x => x.FunctionID == FunctionID);

                //if (null == temp)
                //    return 0;

                //db.RFAFunctions.Remove(temp);

                //List<RFAFunction> list = db.RFAFunctions.Where(x => x.ParentID == FunctionID).ToList<RFAFunction>();

                //for (int i = 0; i < list.Count; i++)
                //{
                //    if (0 == Delete(list[i].FunctionID))
                //        break;
                //}
                //                return db.SaveChanges();
            }
        }


        private int DeleteFunctionsons(BCBackContext db, string FunctionID)
        {
            RFAFunction temp = db.RFAFunctions.FirstOrDefault(x => x.FunctionID.Equals(FunctionID));

            if (null == temp)
                return 0;

            db.RFAFunctions.Remove(temp);

            List<RFAFunction> list = db.RFAFunctions.Where(x => x.ParentID.Equals(FunctionID)).ToList<RFAFunction>();

            for (int i = 0; i < list.Count; i++)
            {
                if (0 == DeleteFunctionsons(db, list[i].FunctionID))
                    break;
            }
            return db.SaveChanges();
        }

        //public int Delete(string FunctionID)
        //{
        //    using (var db = new BCBackContext())
        //    {
        //        RFAFunction temp = db.RFAFunctions.FirstOrDefault(x => x.FunctionID == FunctionID);

        //        if (null == temp)
        //            return 0;

        //        db.RFAFunctions.Remove(temp);

        //        Action<string> deleteChildren = null;
        //        deleteChildren = (parentId) =>
        //        {
        //            List<RFAFunction> list = db.RFAFunctions.Where(x => x.ParentID == FunctionID).ToList<RFAFunction>();
        //            for (int i = 0; i < list.Count; i++)
        //            {
        //                var tempFunction = list[i];
        //                db.RFAFunctions.Remove(tempFunction);
        //                deleteChildren(tempFunction.FunctionID);
        //            }
        //        };
        //        deleteChildren(temp.FunctionID);

        //        return db.SaveChanges();
        //    }
        //}

        public int Add(FunctionDto obj)
        {
            using (var db = new BCBackContext())
            {
                RFAFunction temp = new RFAFunction();

                temp.FunctionID = obj.FunctionID;
                temp.MyID = obj.MyID;
                temp.Name = obj.Name;
                temp.ParentID = obj.ParentID;
                temp.Available = obj.Available;
                temp.Desription = obj.Desription;

                db.RFAFunctions.Add(temp);
                //                db.Entry<RFAFunction>(temp).State = EntityState.Added;

                return db.SaveChanges();
            }
        }


        public bool ExistFunction(string funcId)
        {
            try
            {
                using (var db = new BCBackContext())
                {
                    return db.RFAFunctions.Where(obj=>obj.FunctionID.Equals(funcId)).Count()==1;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int GetAmount()
        {
            try
            {
                using (var db = new BCBackContext())
                {
                    return db.RFAFunctions.Count();          
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        //private string CreateFunctionID(string ParentID, string MyID)
        //{
        //  return ParentID + '.' + MyID;
        //}

        //public List<FunctionDto> GetList(Expression<Func<FunctionDto, bool>> foo, int pagesize, int pagenumber)
        //{

        //}
    }
}
