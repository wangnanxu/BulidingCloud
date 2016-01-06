using ML.BC.EnterpriseWeb.Areas.Account.Models;
using ML.BC.Infrastructure;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.Mvc;
using ML.BC.Services;
using ML.BC.Web.Framework.Controllers;
using System;
using System.Web.Mvc;
using ML.BC.Web.Framework;
using System.Web;
using System.IO;
using System.Drawing;
using ML.BC.Web.Framework.Security;


namespace ML.BC.EnterpriseWeb.Areas.Account.Controllers
{
    
    public class PersonalInfoManagmentController : BCControllerBase
    {
        private FrontUserDto _user = new FrontUserDto
                {
                    UserID = "",
                    Name = "",
                    Mobile = "",
                    RegistDate = DateTime.Now,
                    LastDate = DateTime.Now
                };
        private IFrontUserManagementService _userService = Ioc.GetService<IFrontUserManagementService>();
        private IEnterpriseUserManagementService _userSecurService = Ioc.GetService<IEnterpriseUserManagementService>();

        public PersonalInfoManagmentController()
        {
            try
            {
                var userId = GetSession().User.UserID;
                _user = _userService.GetFrontUserByUserID(userId);

            }
            catch (Exception e)
            {
            }
            finally
            {

            }
        }

        [AuthorizeCheck]
        public ActionResult Index()
        {
            _user = _userService.GetFrontUserByUserID(_user.UserID);
            ViewBag.ID = _user.UserID;
            ViewBag.Avatar = string.IsNullOrEmpty(_user.Picture) ? "/Content/Images/32/20131030111304984.png" : _user.Picture + "?" + new Random(Convert.ToInt32(DateTime.Now.Millisecond)).NextDouble();
            ViewBag.UserName = _user.Name ?? "";
            ViewBag.RegistDate = _user.RegistDate.ToString("yyyy-MM-dd HH:mm");
            ViewBag.LastDate = _user.LastDate == null ? "" : _user.LastDate.Value.ToString("yyyy-MM-dd HH:mm");
            ViewBag.Mobile = _user.Mobile ?? "";
            return View(ViewBag);
        }

        [AuthorizeCheck]
        public ActionResult UpdateInfo(PersonalUpdateModel model)
        {
            if (_user == null || _user.UserID == null) return RedirectToRoute("Account_default", new { controller = "Account", action = "Login", id = UrlParameter.Optional });

            var result = new StandardJsonResult<bool>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                if (model.Password == null) model.Password = "";
                if (model.newPassword == null) model.newPassword = "";
                if (model.newPassword2 == null) model.newPassword2 = "";
                //密码处理
                //旧密码不为空进入
                if (!model.Password.Equals(""))
                {
                    //新密码不匹配或为空进入
                    if (!model.newPassword.Equals(model.newPassword2) || model.newPassword.Eq(""))
                    {
                        result.Value = false;
                        result.Message = "请正确输入新密码.";
                    }
                    //修改密码 和资料
                    else
                    {
                        try
                        {
                            //资料处理
                            _user.Name = model.UserName;
                            _user.Mobile = model.Mobile;
                            var r1 = _userService.UpdateFrontUserInfo(_user);
                            //密码处理
                            var r2 = _userSecurService.UpdateUserPassword(_user.UserID, model.Password, model.newPassword);
                            string s1, s2;
                            if (r1) s1 = "资料更新成功 "; else s1 = "资料更新失败 ";
                            if (r2) s2 = "密码更新成功 "; else s2 = "密码更新失败 ";
                            result.Value = true;
                            result.Message = s1 + "," + s2;
                        }
                        catch (Exception e)
                        {
                            result.Value = false;
                            result.Message += ";" + e.Message;
                        }
                    }

                }
                //旧密码为空进入 不检查新密码
                else
                {
                    //资料处理

                    _user.Name = model.UserName;
                    _user.Mobile = model.Mobile;

                    result.Value = _userService.UpdateFrontUserInfo(_user);
                    result.Message = "资料已更新,密码未更新";
                }


            });
            return result;
        }

        private static string AvatarDir = "/UserAvatar/";
        private static string TempDir = "/UserAvatar/temp/";
        public ActionResult ProcessImage(AvatarUpdateModel model)
        {

            var result = new StandardJsonResult<string>();
            result.Try(() =>
            {
                if (!ModelState.IsValid)
                {
                    throw new KnownException(ModelState.GetFirstError());
                }
                //context.Response.ContentType = "text/plain";
                string action = model.actionx;
                if (action == "up")//接收上传的图像
                {
                    HttpPostedFileBase file = Request.Files[0];//接收文件.
                    //火狐浏览器无法获取到会话 上传图片时可不验证 则不使用id命名临时图片 待其他操作如保存头像时验证
                    string _fileNameSuffix = BCSession.User.UserID ?? DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    string fileName = _fileNameSuffix + "TEMP.jpg";//获取文件名。
                    //string fileExt = Path.GetExtension(fileName);//获取文件类型.
                    //if (fileExt.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase))
                    //{
                    using (Image img = Image.FromStream(file.InputStream))//根据上传的文件创建一个Image.
                    {
                        if (Directory.Exists(Server.MapPath(TempDir)) == false)//如果不存在就创建file文件夹
                        {
                            Directory.CreateDirectory(Server.MapPath(TempDir));
                        }
                        file.SaveAs(Server.MapPath(TempDir + fileName));
                        result.Value = "ok:" + TempDir + fileName + ":" + img.Width + ":" + img.Height;
                    }
                    //}

                }
                else if (action == "cut")//头像截取并保存
                {
                    if (BCSession.User == null || BCSession.User.UserID == null)
                    {
                        result.Value = "error:need login";
                    }
                    else
                    {
                        int x = Convert.ToInt32(model.x);
                        int y = Convert.ToInt32(model.y);
                        int width = Convert.ToInt32(model.width);
                        int height = Convert.ToInt32(model.height);
                        string imgSrc = model.imgSrc;//获取上传成功的图片路径
                        //根据传递过来的范围，将该范围的图片画到画布上，将画布保存。
                        using (Bitmap map = new Bitmap(width, height))
                        {
                            using (Graphics g = Graphics.FromImage(map))//为画布创建画笔.
                            {
                                using (Image img = Image.FromFile(Server.MapPath(imgSrc)))//创建img
                                {
                                    var zoom = img.Width / 350.0;

                                    //将图片画到画布上
                                    //第一：对哪张图片进行操作
                                    //二：画多么大
                                    //三：画哪部分
                                    g.DrawImage(img, new Rectangle(0, 0, 200, 200), new Rectangle(Convert.ToInt32(x * zoom + 0.9), Convert.ToInt32(y * zoom + 0.9), Convert.ToInt32(width * zoom + 0.9), Convert.ToInt32(height * zoom + 0.9)), GraphicsUnit.Pixel);
                                    //string newfile = Guid.NewGuid().ToString();
                                    if (Directory.Exists(Server.MapPath(AvatarDir)) == false)//如果不存在就创建file文件夹
                                    {
                                        Directory.CreateDirectory(Server.MapPath(AvatarDir));
                                    }
                                    if (Directory.Exists(Server.MapPath(TempDir)) == false)//如果不存在就创建file文件夹
                                    {
                                        Directory.CreateDirectory(Server.MapPath(TempDir));
                                    }
                                    int idx = _user.Picture.LastIndexOf(AvatarDir);
                                    if (idx > 0) _user.Picture = _user.Picture.Substring(idx);
                                    try
                                    {
                                        if (System.IO.File.Exists(Server.MapPath(_user.Picture)))
                                        {
//                                            System.IO.File.Delete(Server.MapPath(_user.Picture));
                                        }
                                    }
                                    catch (Exception)
                                    {

                                    }
                                    finally
                                    {
                                        string fileName = _user.UserID + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
                                        map.Save(Server.MapPath(AvatarDir + fileName));//将画布上的图片按照GUID命名保存
                                        result.Value = AvatarDir + fileName;
                                        _userSecurService.SetUserAvatar(_user.UserID, result.Value);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    result.Value = "error";
                }

            });
            return new OringinalJsonResult<string> { Value = result.Value };
        }
    }
}
