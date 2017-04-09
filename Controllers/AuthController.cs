using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TM;

namespace GymManager.Controllers
{
    public class AuthController : BaseController
    {
        // GET: Auth
        public ActionResult Index()
        {
            if (Connection() == null)
                this.danger("Không thể kết nối đến CSDL!");
            if (VinaphoneCommon.Auth.isAuth())
                return Redirect(TM.Url.BaseUrl);
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            try
            {
                string username = collection["username"].ToString();
                string password = collection["password"].ToString();
                System.Collections.ArrayList AuthItems = new System.Collections.ArrayList();

                //AuthStatic
                if (VinaphoneCommon.AuthStatic.isAuthStatic(username, password))
                {
                    AuthItems.Add(VinaphoneCommon.AuthStatic.id.ToString());
                    AuthItems.Add(VinaphoneCommon.AuthStatic.username);
                    AuthItems.Add(VinaphoneCommon.AuthStatic.salt);
                    AuthItems.Add(VinaphoneCommon.AuthStatic.full_name);
                    AuthItems.Add(VinaphoneCommon.AuthStatic.mobile);
                    AuthItems.Add(VinaphoneCommon.AuthStatic.email);
                    AuthItems.Add(VinaphoneCommon.AuthStatic.address);
                    AuthItems.Add(VinaphoneCommon.AuthStatic.roles);
                    AuthItems.Add(VinaphoneCommon.AuthStatic.created_by);
                    AuthItems.Add(DateTime.Now.ToString());
                    AuthItems.Add(VinaphoneCommon.AuthStatic.updated_by);
                    AuthItems.Add(DateTime.Now.ToString());
                    AuthItems.Add(DateTime.Now.ToString());
                    AuthItems.Add(VinaphoneCommon.AuthStatic.flag.ToString());
                    VinaphoneCommon.Auth.setAuth(AuthItems);
                    return Redirect(TM.Url.RedirectContinue());
                }

                //AuthDB
                var tmp = db.users.Where(u => u.username == username);
                if (tmp.FirstOrDefault() == null)
                {
                    this.danger("Sai tên tài khoản hoặc mật khẩu!");
                    return View();
                }

                password = TM.Encrypt.CryptoMD5TM(password + tmp.FirstOrDefault().salt);
                tmp = db.users.Where(u => u.username == username && u.password == password);
                if (tmp.SingleOrDefault() == null)
                {
                    this.danger("Sai tên tài khoản hoặc mật khẩu!");
                    return View();
                }

                var rs = tmp.Where(u => u.flag > 0).FirstOrDefault();
                if (rs == null)
                {
                    this.danger("Tài khoản đã bị khóa. Vui lòng liên hệ admin!");
                    return View();
                }

                AuthItems.Add(rs.id);
                AuthItems.Add(rs.username);
                AuthItems.Add(rs.salt);
                AuthItems.Add(rs.full_name);
                AuthItems.Add(rs.mobile);
                AuthItems.Add(rs.email);
                AuthItems.Add(rs.address);
                AuthItems.Add(rs.roles);
                AuthItems.Add(rs.created_by);
                AuthItems.Add(rs.created_at);
                AuthItems.Add(rs.updated_by);
                AuthItems.Add(rs.updated_at);
                AuthItems.Add(rs.last_login);
                AuthItems.Add(rs.flag);
                VinaphoneCommon.Auth.setAuth(AuthItems);
                return Redirect(TM.Url.RedirectContinue());
                //return Redirect(TM.Url.BaseUrl);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting  
                        // the current instance as InnerException  
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
            return View();
        }
        [Filters.AuthVinaphone]
        [HttpGet]
        public ActionResult logout()
        {
            VinaphoneCommon.Auth.logout();
            return Redirect(System.Web.HttpContext.Current.Request.UrlReferrer.ToString());//System.Web.HttpContext.Current.Request.UrlReferrer
        }
        [Filters.AuthVinaphone]
        public ActionResult ChangePassword(Guid id)
        {

            return View(db.users.SingleOrDefault(u => u.id == id));
        }
        [Filters.AuthVinaphone]
        [HttpPost]
        public ActionResult ChangePassword(Guid id, string password)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var rs = db.users.Find(id);
                    rs.password = TM.Encrypt.CryptoMD5TM(password + rs.salt);
                    db.SaveChanges();
                    ModelState.Clear();
                    this.success("Cập nhật mật khẩu thành công");
                    return RedirectToAction("Index");
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting  
                        // the current instance as InnerException  
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
            return View();
        }
    }
}