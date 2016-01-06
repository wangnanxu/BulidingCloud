using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Collections;
using ML.BC.Web.Framework.Controllers;
using ML.BC.Web.Framework.Security;
using ML.BC.Services;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using ML.BC.EnterpriseWeb.Areas.Specification.Models;
using ML.BC.Web.Framework;
using ML.BC.EnterpriseData.Common;
namespace ML.BC.EnterpriseWeb.Areas.Specification.Controllers
{
    [AuthorizeCheckAttribute]
    public class MaterialListManagementController : BCControllerBase
    {
        //
        // GET: /Specification/MaterialListManagement/
        IKnowledgaeList service = null;
        public MaterialListManagementController()
        {
            service = ML.BC.Infrastructure.Ioc.GetService<IKnowledgaeList>();
        }
        public ActionResult ListIndex()
        {
            return View();
        }
        [PermissionControlAttribute(Functions.Root_DataManagement_DataListManagement_List)]
        public ActionResult GetList(MaterialPara model)
        {
            var result = new StandardJsonResult<MaterialListResult>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                if (model.rows == 0)
                    model.rows = 10;
                if (model.page == 0)
                    model.page = 1;

                int amount;
                List<KnowlegeDto> list = new List<KnowlegeDto>();
                FileType ftype = FileType.All;
                if (model.fileType != null)
                {
                    ftype = (FileType)Convert.ToInt32(model.fileType);
                }
                list = service.GetKnowlegeList(model.name, ftype, model.materialType, BCSession.User.EnterpriseID, model.rows, model.page, out amount);
                result.Value = new MaterialListResult();
                List<MaterialListModel> mylist = new List<MaterialListModel>();
                List<MaterialTypeModel> typeList = GetMaterialTypeList();
                if (typeList == null)
                {
                    typeList = new List<MaterialTypeModel>();
                }
                foreach (var l in list)
                {
                    MaterialListModel m = l;
                    var typelist = typeList.Where(x => x.value == Convert.ToInt32(m.materialType)).ToList();
                    if (typelist != null)
                    {
                        if (typelist.Count > 0)
                        {
                            m.materialTypeName = typelist[0].text;
                        }
                        else
                        {
                            m.materialTypeName = "无法识别的类型";
                        }
                    }
                    switch (m.fileType)
                    {
                        case FileType.Word:
                            {
                                m.fileTypeName = "Word";
                            } break;
                        case FileType.Excel:
                            {
                                m.fileTypeName = "Excel";
                            } break;
                        case FileType.PDF:
                            {
                                m.fileTypeName = "PDF";
                            } break;
                        case FileType.PPT:
                            {
                                m.fileTypeName = "PowerPoint";
                            } break;
                    }
                    mylist.Add(m);
                }
                result.Value.rows = mylist;
                result.Value.total = amount;
            });
            if (!result.Success)
            {
                result.Value = new MaterialListResult();
                result.Message = "获取失败";
            }
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
        [PermissionControlAttribute(Functions.Root_DataManagement_DataListManagement_Download)]
        public ActionResult Download(string fileName, Guid fileID)
        {
            var result = new StandardJsonResult();
            byte[] file = null;
            result.Try(() =>
            {
                file = service.DownLoading(fileID);
            });
            if (file == null)
                throw new KnownException("获取数据失败");
            else
                return File(file, fileName, fileName);
        }
        public ActionResult GetFileType()
        {
            HasFunction(Functions.Root_DataManagement_DataListManagement_Download);
            var result = new StandardJsonResult<List<FileTypeModel>>();
            result.Try(() =>
            {
                result.Value = new List<FileTypeModel>();
                result.Value.Add(new FileTypeModel()
                {
                    text = "Excel",
                    value = FileType.Excel
                });
                result.Value.Add(new FileTypeModel()
                {
                    text = "Word",
                    value = FileType.Word
                });
                result.Value.Add(new FileTypeModel()
                {
                    text = "PPT",
                    value = FileType.PPT
                });
                result.Value.Add(new FileTypeModel()
                {
                    text = "PDF",
                    value = FileType.PDF
                });
            });
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }
        [PermissionControlAttribute(Functions.Root_DataManagement_DataTypeManagement_List)]
        public ActionResult GetMaterialType()
        {
            var result = new StandardJsonResult<List<MaterialTypeModel>>();
            result.Try(() =>
            {
                result.Value = GetMaterialTypeList();
            });
            if (!result.Success)
                result.Value = new List<MaterialTypeModel>();
            return Json(result.Value, JsonRequestBehavior.AllowGet);
        }

        [PermissionControlAttribute(Functions.Root_DataManagement_DataListManagement_Add)]
        public ActionResult UploadSmall(AddMaterialListModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new KnownException(ModelState.GetFirstError());
            }
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {

                var file = Request.Files;
                int filecount = file.Count;
                for (int i = 0; i < filecount; i++)
                {
                    FileType ftype = FileType.All;
                    string fileformat = file[i].FileName.Split('.')[file[i].FileName.Split('.').Count() - 1].ToLower();
                    if (fileformat.IndexOf("doc") >= 0)
                    {
                        ftype = FileType.Word;
                    }
                    else if (fileformat.IndexOf("xls") >= 0)
                    {
                        ftype = FileType.Excel;
                    }
                    else if (fileformat.IndexOf("ppt") >= 0)
                    {
                        ftype = FileType.PPT;
                    }
                    else if (fileformat.IndexOf("pdf") >= 0)
                    {
                        ftype = FileType.PDF;
                    }
                    else
                    {
                        throw new KnownException("上传的格式不对");
                    }
                    var f = file[i];
                    Stream sf = f.InputStream;
                    result.Value = service.UpLoading(new KnowlegeDto()
                    {
                        Deleted = false,
                        Name = model.fileName == null ? f.FileName : model.fileName + "." + fileformat,
                        KnowledgeType = model.materialType,
                        DocumentType = ftype,
                        FileStream = sf,
                        EnterpriseID = BCSession.User.EnterpriseID,
                        DocumentSize = f.ContentLength,
                        ID = DateTime.Now.ToFileTime().ToString() + "." + fileformat
                    });
                }
            });
            if (result.Value && result.Success)
            {
                result.Message = "添加成功";
            }
            return result;
        }

        public ActionResult Upload(AddMaterialListModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new KnownException(ModelState.GetFirstError());
            }
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {

                var file = Request.Files;
                int filecount = file.Count;
                for (int i = 0; i < filecount; i++)
                {
                    FileType ftype = FileType.All;
                    if (model.ext.ToLower().IndexOf("doc") >= 0)
                    {
                        ftype = FileType.Word;
                    }
                    else if (model.ext.ToLower().IndexOf("xls") >= 0)
                    {
                        ftype = FileType.Excel;
                    }
                    else if (model.ext.ToLower().IndexOf("ppt") >= 0)
                    {
                        ftype = FileType.PPT;
                    }
                    else if (model.ext.ToLower().IndexOf("pdf") >= 0)
                    {
                        ftype = FileType.PDF;
                    }
                    else
                    {
                        throw new KnownException("上传的格式不对");
                    }

                    var f = file[i];
                    Stream sf = f.InputStream;
                    result.Value = service.UpLoading(new KnowlegeDto()
                    {
                        Deleted = false,
                        Name = model.fileName == null ? f.FileName : model.fileName + "." + model.ext,
                        KnowledgeType = model.materialType,
                        DocumentType = ftype,
                        FileStream = sf,
                        EnterpriseID = BCSession.User.EnterpriseID,
                        DocumentSize = f.ContentLength,
                        ID = DateTime.Now.ToFileTime().ToString() + "." + model.ext,
                        FileGUID = model.guid,
                        FileAllSize = model.byteLength,
                        FileNumber = model.chunk,

                    });


                }
            });
            if (result.Value && result.Success)
            {
                result.Message = "添加成功";
            }
            return result;
        }
        [PermissionControlAttribute(Functions.Root_DataManagement_DataListManagement_Edit)]
        public ActionResult Update(AddMaterialListModel model, string fileID)
        {
            if (!ModelState.IsValid)
            {
                throw new KnownException(ModelState.GetFirstError());
            }
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                result.Value = service.UpdateFileType(fileID, model.materialType, model.guid);
            });
            if (result.Value && result.Success)
            {
                result.Message = "修改成功";
            }

            return result;
        }
        public List<MaterialTypeModel> GetMaterialTypeList()
        {
            var mservice = ML.BC.Infrastructure.Ioc.GetService<IMaterialTypeManagementService>();
            List<MaterialTypeModel> list = new List<MaterialTypeModel>();
            List<MaterialTypeDto> mtype = mservice.GetAllMaterialType();
            if (mtype != null)
            {
                foreach (var t in mtype)
                {
                    list.Add(new MaterialTypeModel()
                    {
                        value = t.MaterialTypeID,
                        text = t.Name
                    });
                }
            }
            return list;
        }
        [PermissionControlAttribute(Functions.Root_DataManagement_DataListManagement_Delete)]
        public ActionResult Delete(Guid fileID)
        {
            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                result.Value = service.DeleteFile(fileID);
            });
            if (result.Value && result.Success)
            {
                result.Message = "删除成功";
            }
            return result;
        }
        public ActionResult uploadBigFile()
        {
            return View();
        }
    }
}
