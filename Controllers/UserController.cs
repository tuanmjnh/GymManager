using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TM;
using PagedList;

namespace GymManager.Controllers
{
    [Filters.AuthVinaphone(Role = Common.roles.superadmin + "," + Common.roles.admin)]
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Index(int? flag, string order, string currentFilter, string searchString, int? page, string export)
        {
            try
            {
                if (searchString != null)
                {
                    page = 1;
                    searchString = searchString.Trim();
                }
                else searchString = currentFilter;

                ViewBag.currentFilter = searchString;
                ViewBag.flag = flag;

                Guid id = VinaphoneCommon.Auth.id();
                var rs = from u in db.users where u.id != id select u;//db.users.Where(u => u.id != id && u.flag == 1 && u.roles == roles).ToList();

                if (flag == 0) rs = rs.Where(s => s.flag == 0);
                else rs = rs.Where(s => s.flag == 1);

                if (!String.IsNullOrWhiteSpace(searchString))
                    rs = rs.Where(d => d.username.Contains(searchString) || d.full_name.Contains(searchString));

                if (VinaphoneCommon.Auth.roles() == VinaphoneCommon.roles.admin)
                    rs = rs.Where(u => u.roles == VinaphoneCommon.roles.mod);
                else if (VinaphoneCommon.Auth.roles() == VinaphoneCommon.roles.mod)
                    rs = null;

                rs = rs.OrderByDescending(d => d.roles).ThenBy(d => d.staff_id);

                //Export to any
                if (!String.IsNullOrEmpty(export))
                {
                    TM.Exports.ExportExcel(TM.Exports.LINQToDataTable(rs.ToList()), "Danh sách tài khoản");
                    return RedirectToAction("Index");
                }

                ViewBag.TotalRecords = rs.Count();
                int pageSize = 15;
                int pageNumber = (page ?? 1);
                return View(rs.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception)
            {
                this.danger("Lỗi không tìm thấy dữ liệu. Vui lòng thử lại!");
            }
            return View();
        }
        public ActionResult Details(Guid id)
        {
            return View(db.users.SingleOrDefault(u => u.id == id));
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Models.user user) //[Bind(Include = "username,full_name,mobile,email,address,roles,flag")]
        {
            try
            {
                //if (ModelState.IsValid)
                //{
                if (!db.users.Any(u => u.username.ToLower() == user.username.ToLower()))
                {
                    user.id = Guid.NewGuid();
                    user.salt = Guid.NewGuid().ToString();
                    user.password = TM.Encrypt.CryptoMD5TM(user.password + user.salt);
                    //user.roles = VinaphoneCommon.roles.mod;
                    //user.repeatPassword = user.password;
                    user.created_by = VinaphoneCommon.Auth.id().ToString();
                    user.created_at = DateTime.Now;
                    db.users.Add(user);
                    db.SaveChanges();
                    ModelState.Clear();
                    this.success("Tạo mới tài khoản thành công");
                    return RedirectToAction("Index");
                }
                else
                    this.danger("Tài khoản đã tồn tại");
                //}
                //else
                //    this.danger("Tạo mới tài khoản không thành công");
            }
            catch (Exception)
            {
                this.danger("Tạo mới tài khoản không thành công");
            }
            return RedirectToAction("Create");
        }

        [HttpGet]
        public ActionResult ResetPassword(Guid id)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var rs = db.users.Find(id);
                    var newpass = Guid.NewGuid().ToString().Split('-');
                    rs.password = TM.Encrypt.CryptoMD5TM(newpass[0] + rs.salt);
                    db.SaveChanges();
                    this.success("Mật khẩu mới của tài khoản <strong>" + rs.username + "</strong> là: <strong>" + newpass[0] + "</strong>");
                    ModelState.Clear();
                }
                else
                    TempData["Message"] = "Lỗi trong quá trình xử lý";
            }
            catch
            {
                TempData["Message"] = "Lỗi trong quá trình xử lý";
            }
            return RedirectToAction("Index");
        }
        public ActionResult Edit(Guid id)
        {
            return View(db.users.SingleOrDefault(u => u.id == id));
        }
        [HttpPost]
        public ActionResult Edit(Guid id, [Bind(Include = "full_name,mobile,email,address,roles,flag")]Models.user user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var rs = db.users.Find(id);
                    rs.full_name = user.full_name;
                    rs.mobile = user.mobile;
                    rs.email = user.email;
                    rs.address = user.address;
                    rs.roles = user.roles;
                    rs.flag = user.flag;
                    user.updated_by = VinaphoneCommon.Auth.id().ToString();
                    user.updated_at = DateTime.Now;
                    db.SaveChanges();
                    ModelState.Clear();
                    this.success("Cập nhật thông tin thành công");
                    return RedirectToAction("Index");
                }
                else
                    this.danger("Cập nhật không thành công");
            }
            catch { }
            this.danger("Cập nhật thông tin thất bại");
            return Redirect(id.ToString());
        }
        public ActionResult Delete(Guid id)
        {
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult Delete(Guid id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [AllowAnonymous]
        public JsonResult check_exist_cname(string username)
        {
            return Json(!db.users.Any(u => u.username.ToLower() == username.ToLower()), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult update_flag(string uid)
        {
            try
            {
                string[] id = uid.Split(',');
                var flag = 0;
                foreach (var item in id)
                {
                    Guid tmp = Guid.Parse(item);
                    var rs = db.users.Find(tmp);
                    rs.flag = flag = rs.flag == 1 ? 0 : 1;
                }
                db.SaveChanges();
                return Json(new { success = (flag == 0 ? "Khóa tài khoản thành công" : "Khôi phục tài khoản thành công") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json(new { danger = "Xử lý lỗi vui lòng thử lại!" }, JsonRequestBehavior.AllowGet); }
        }
    }
}