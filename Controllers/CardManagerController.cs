using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TM;
using PagedList;

namespace GymManager.Controllers
{
    [Filters.AuthVinaphone()]
    public class CardManagerController : BaseController
    {
        // GET: CardManager
        public ActionResult Index(int? flag, string order, string currentFilter, string searchString, int? page, string export)
        {
            try
            {
                //if (!customer.HasValue)
                //{
                //    this.danger("Vui lòng chọn tài khoản trước!");
                //    return RedirectToAction("Index", "Customer", null);
                //}
                if (searchString != null)
                {
                    page = 1;
                    searchString = searchString.Trim();
                }
                else searchString = currentFilter;
                ViewBag.order = order;
                ViewBag.currentFilter = searchString;
                ViewBag.flag = flag;

                var rs = from d in db.items where d.app_key == AppKey.cardManager select d;

                if (!String.IsNullOrEmpty(searchString))
                    rs = rs.Where(d =>
                    d.title.Contains(searchString) ||
                    d.code_key.Contains(searchString));

                if (flag == 0) rs = rs.Where(d => d.flag == 0);
                else rs = rs.Where(d => d.flag == 1);

                switch (order)
                {
                    case "title_desc":
                        rs = rs.OrderByDescending(d => d.title);
                        break;
                    case "title_asc":
                        rs = rs.OrderBy(d => d.title);
                        break;
                    case "price_desc":
                        rs = rs.OrderByDescending(d => d.price);
                        break;
                    default:
                        rs = rs.OrderBy(d => d.price);
                        break;
                        //case "id_asc":
                        //    rs = rs.OrderByDescending(d => d.id);
                        //    break;
                        //default:
                        //    rs = rs.OrderBy(d => d.id);
                        //    break;
                }

                //Export to any
                if (!String.IsNullOrEmpty(export))
                {
                    TM.Exports.ExportExcel(TM.Exports.LINQToDataTable(rs.ToList()), "Danh sách thẻ");
                    return RedirectToAction("Index");
                }

                ViewBag.TotalRecords = rs.Count();
                int pageSize = 15;
                int pageNumber = (page ?? 1);

                return View(rs.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                this.danger(VinaphoneCommon.Language.msgError);
            }
            return View();
        }

        // GET: CardManager/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.item item = db.items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: CardManager/Create
        public ActionResult Create()
        {
            //ViewBag.id_key = new SelectList(db.customers, "id", "full_name");
            //ViewBag.item_id = new SelectList(db.items, "id", "app_key");
            return View();
        }

        // POST: CardManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "code_key,title,quantity,price,desc,flag")] Models.item item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    item.id = Guid.NewGuid();
                    item.app_key = AppKey.cardManager;
                    item.created_by = VinaphoneCommon.Auth.id().ToString();
                    item.created_at = DateTime.Now;
                    db.items.Add(item);
                    db.SaveChanges();
                    this.success(VinaphoneCommon.Language.msgCreateSucsess);
                    return RedirectToAction("Create");
                }
                else
                    this.danger(VinaphoneCommon.Language.msgError);
            }
            catch (Exception)
            {
                this.danger(VinaphoneCommon.Language.msgCreateError);
            }
            return View(item);
        }

        // GET: CardManager/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.item item = db.items.Find(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            //ViewBag.id_key = new SelectList(db.customers, "id", "full_name", sub_item.id_key);
            //ViewBag.item_id = new SelectList(db.items, "id", "app_key", sub_item.item_id);
            return View(item);
        }

        // POST: CardManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,code_key,title,quantity,price,desc,flag")] Models.item item_tmp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = db.items.Find(item_tmp.id);
                    item.code_key = item_tmp.code_key;
                    item.title = item_tmp.title;
                    item.quantity = item_tmp.quantity;
                    item.price = item_tmp.price;
                    item.desc = item_tmp.desc;
                    item.flag = item_tmp.flag;
                    item.updated_by = VinaphoneCommon.Auth.id().ToString();
                    item.updated_at = DateTime.Now;

                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                    this.success(VinaphoneCommon.Language.msgUpdateSucsess);
                    return RedirectToAction("Index");
                    //ViewBag.id_key = new SelectList(db.customers, "id", "full_name", sub_item_tmp.id_key);
                    //ViewBag.item_id = new SelectList(db.items, "id", "app_key", sub_item_tmp.item_id);
                }
                else
                    this.danger(VinaphoneCommon.Language.msgError);
            }
            catch (Exception)
            {
                this.danger(VinaphoneCommon.Language.msgUpdateError);
            }
            return View(item_tmp);
        }
        public ActionResult CreateCard()
        {
            //ViewBag.id_key = new SelectList(db.customers, "id", "full_name");
            //ViewBag.item_id = new SelectList(db.items, "id", "app_key");
            return View();
        }

        // POST: CardManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCard([Bind(Include = "title,price,desc,flag")] Models.item item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    item.id = Guid.NewGuid();
                    item.app_key = AppKey.cardManager;
                    item.created_by = VinaphoneCommon.Auth.id().ToString();
                    item.created_at = DateTime.Now;
                    db.items.Add(item);
                    db.SaveChanges();
                    this.success(VinaphoneCommon.Language.msgCreateSucsess);
                    return RedirectToAction("Create");
                }
                else
                    this.danger(VinaphoneCommon.Language.msgError);
            }
            catch (Exception)
            {
                this.danger(VinaphoneCommon.Language.msgCreateError);
            }
            return View(item);
        }
        public ActionResult TimeKeeping(int? flag, string order, string currentFilter, string searchString, int? page, string export)
        {
            try
            {
                //if (!customer.HasValue)
                //{
                //    this.danger("Vui lòng chọn tài khoản trước!");
                //    return RedirectToAction("Index", "Customer", null);
                //}
                if (searchString != null)
                {
                    page = 1;
                    searchString = searchString.Trim();
                }
                else searchString = currentFilter;
                ViewBag.order = order;
                ViewBag.currentFilter = searchString;
                ViewBag.flag = flag;
                ViewBag.cardPersonDay = db.items.Where(d => d.app_key == AppKey.cardManager && d.code_key == AppKey.TypePayDay && d.flag > 0).OrderByDescending(d => d.price).ToList();

                var datetimenow = DateTime.Now;
                var rs = (from d in db.items
                          join c in db.customers on d.id_key equals c.id.ToString()
                          where d.app_key == AppKey.cardPerson && d.flag > 0
                          select new
                          {
                              id = d.id,
                              appkey = d.app_key,
                              idkey = d.id_key,
                              codekey = d.code_key,
                              title = d.title,
                              desc = d.desc,
                              quantity = d.quantity,
                              quantitytotal = d.quantity_total,
                              priceold = d.price_old,
                              price = d.price,
                              startedat = d.started_at,
                              endedat = d.ended_at,
                              flag = d.flag,
                              extras = d.extras,
                              customerid = c.id,
                              customerflag = c.flag,
                              fullname = c.full_name,
                              cardid = c.card_id,
                              checkcard = db.sub_item.Where(s => s.item_id == d.id && s.app_key == AppKey.cardPerson &&
                                        s.created_at.Value.Year == datetimenow.Year &&
                                        s.created_at.Value.Month == datetimenow.Month &&
                                        s.created_at.Value.Day == datetimenow.Day).FirstOrDefault()
                          });
                rs = rs.Where(d => d.customerflag > 0);
                if (!String.IsNullOrEmpty(searchString))
                    rs = rs.Where(d =>
                    d.fullname.Contains(searchString) ||
                    d.cardid.Contains(searchString));

                if (flag == 0) rs = rs.Where(d => d.flag == 0);
                else rs = rs.Where(d => d.flag == 1);

                switch (order)
                {
                    case "title_desc":
                        rs = rs.OrderByDescending(d => d.title);
                        break;
                    case "title_asc":
                        rs = rs.OrderBy(d => d.title);
                        break;
                    case "price_desc":
                        rs = rs.OrderByDescending(d => d.price);
                        break;
                    default:
                        rs = rs.OrderBy(d => d.price);
                        break;
                        //case "id_asc":
                        //    rs = rs.OrderByDescending(d => d.id);
                        //    break;
                        //default:
                        //    rs = rs.OrderBy(d => d.id);
                        //    break;
                }

                //Export to any
                if (!String.IsNullOrEmpty(export))
                {
                    TM.Exports.ExportExcel(TM.Exports.LINQToDataTable(rs.ToList()), "Danh sách chấm tập");
                    return RedirectToAction("Index");
                }

                ViewBag.TotalRecords = rs.Count();
                int pageSize = 15;
                int pageNumber = (page ?? 1);

                return View(rs.AsEnumerable().Select(d => d.ToExpando()).ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                this.danger(VinaphoneCommon.Language.msgError);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddBillClient(string customer, string price, string desc)
        {
            try
            {
                var item = new Models.item();
                item.id = Guid.NewGuid();
                item.app_key = AppKey.cardPerson;
                item.id_key = customer;
                item.title = price;
                item.desc = desc;
                item.quantity = 1;
                item.price = decimal.Parse(price);
                item.flag = 0;
                item.extras = AppKey.TypePayDay;
                item.created_by = VinaphoneCommon.Auth.id().ToString();
                item.created_at = DateTime.Now;
                db.items.Add(item);
                db.SaveChanges();
                return Json(new { success = VinaphoneCommon.Language.msgSucsess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json(new { danger = VinaphoneCommon.Language.msgError }, JsonRequestBehavior.AllowGet); }
        }
        // GET: CardManager/Delete/5
        //public ActionResult Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Models.sub_item sub_item = db.sub_item.Find(id);
        //    if (sub_item == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(sub_item);
        //}

        //// POST: CardManager/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    Models.sub_item sub_item = db.sub_item.Find(id);
        //    db.sub_item.Remove(sub_item);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        [HttpGet]
        public JsonResult LoadTimeKeeping()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var TimeKeeping = db.sub_item.Where(d => d.app_key == AppKey.cardPerson &&
                d.created_at.Value.Year == DateTime.Now.Year &&
                d.created_at.Value.Month == DateTime.Now.Month &&
                d.created_at.Value.Day == DateTime.Now.Day).ToList();
                var Client = db.items.Where(d => d.app_key == AppKey.cardPerson && d.code_key == null &&
                d.created_at.Value.Year == DateTime.Now.Year &&
                d.created_at.Value.Month == DateTime.Now.Month &&
                d.created_at.Value.Day == DateTime.Now.Day).ToList();
                return Json(new { data = TimeKeeping, Client = Client, success = VinaphoneCommon.Language.msgSucsess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json(new { danger = VinaphoneCommon.Language.msgError }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DoTimeKeeping(Guid? id, int? quantity)
        {
            try
            {
                var TimeKeeping = db.sub_item.Where(d => d.item_id == id && d.app_key == AppKey.cardPerson &&
                d.created_at.Value.Year == DateTime.Now.Year &&
                d.created_at.Value.Month == DateTime.Now.Month &&
                d.created_at.Value.Day == DateTime.Now.Day).FirstOrDefault();
                if (TimeKeeping == null)
                {
                    var cardPerson = db.items.Find(id);
                    if (cardPerson != null)
                    {
                        if (cardPerson.extras == AppKey.TypePayDay)
                        {
                            cardPerson.quantity_total = cardPerson.quantity_total + quantity;
                            db.Entry(cardPerson).State = EntityState.Modified;
                        }
                        TimeKeeping = new Models.sub_item();
                        TimeKeeping.id = Guid.NewGuid();
                        TimeKeeping.app_key = AppKey.cardPerson;
                        TimeKeeping.item_id = id;
                        TimeKeeping.value = quantity.ToString();
                        TimeKeeping.created_by = VinaphoneCommon.Auth.id().ToString();
                        TimeKeeping.created_at = DateTime.Now;
                        TimeKeeping.flag = 1;
                        TimeKeeping.extras = cardPerson.extras;
                        db.sub_item.Add(TimeKeeping);
                    }
                    db.SaveChanges();
                    return Json(new { success = VinaphoneCommon.Language.msgSucsess }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { danger = "Tài khoản đã được chấm trong ngày!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json(new { danger = VinaphoneCommon.Language.msgError }, JsonRequestBehavior.AllowGet); }
        }
        [AllowAnonymous]
        public JsonResult check_exist_main_key(string title)
        {
            return Json(!db.items.Any(d => d.title.ToUpper() == title.ToUpper() && d.app_key == AppKey.cardManager), JsonRequestBehavior.AllowGet);
        }
        private int GetCardID(int code_key)
        {
            try
            {
                var check = db.items.Any(d => d.code_key == code_key.ToString() && d.app_key == AppKey.cardManager);
                if (check)
                {
                    Random rand = new Random();
                    var tmp = rand.Next(100000, 999999);
                    return GetCardID(tmp);
                }
                else return code_key;
            }
            catch (Exception) { throw; }
        }
        [AllowAnonymous]
        public JsonResult getCardID()
        {
            try
            {
                Random rand = new Random();
                var tmp = rand.Next(100000, 999999);
                return Json(new { success = VinaphoneCommon.Language.msgSucsess, data = GetCardID(tmp) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { danger = ex.Message }, JsonRequestBehavior.AllowGet); }//"Xử lý lỗi vui lòng thử lại!" 
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
                    var tmp = Guid.Parse(item);
                    var rs = db.items.Find(tmp);
                    rs.flag = flag = rs.flag == 1 ? 0 : 1;
                }
                db.SaveChanges();
                return Json(new { success = (flag == 0 ? VinaphoneCommon.Language.msgLockSucsess : VinaphoneCommon.Language.msgRecoverSucsess) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json(new { danger = VinaphoneCommon.Language.msgError }, JsonRequestBehavior.AllowGet); }
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(string uid)
        {
            try
            {
                string[] id = uid.Split(',');
                foreach (var item in id)
                {
                    var tmp = Guid.Parse(item);
                    var rs = db.items.Find(tmp);
                    db.items.Remove(rs);
                    var sub_item = db.sub_item.Where(d => d.id_key == rs.id);
                    foreach (var i in sub_item)
                        db.sub_item.Remove(i);
                }
                db.SaveChanges();
                return Json(new { success = VinaphoneCommon.Language.msgDeleteSucsess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json(new { danger = VinaphoneCommon.Language.msgError }, JsonRequestBehavior.AllowGet); }
        }
    }
}
