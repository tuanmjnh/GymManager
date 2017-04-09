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
    public class PersonInfoController : BaseController
    {
        // GET: PersonInfo
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
                ViewBag.order = order;
                ViewBag.currentFilter = searchString;
                ViewBag.flag = flag;

                var rs = from d in db.sub_item where d.app_key == AppKey.personInfo select d;

                if (!String.IsNullOrEmpty(searchString))
                    rs = rs.Where(d =>
                    d.value.Contains(searchString) ||
                    d.sub_value.Contains(searchString));

                if (flag == 0) rs = rs.Where(d => d.flag == 0);
                else rs = rs.Where(d => d.flag == 1);

                switch (order)
                {
                    case "value_desc":
                        rs = rs.OrderByDescending(d => d.value);
                        break;
                    default:
                        rs = rs.OrderBy(d => d.value);
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
                    TM.Exports.ExportExcel(TM.Exports.LINQToDataTable(rs.ToList()), "Danh sách thông tin cá nhân");
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

        // GET: PersonInfo/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.sub_item sub_item = db.sub_item.Find(id);
            if (sub_item == null)
            {
                return HttpNotFound();
            }
            return View(sub_item);
        }

        // GET: PersonInfo/Create
        public ActionResult Create()
        {
            //ViewBag.id_key = new SelectList(db.customers, "id", "full_name");
            return View();
        }

        // POST: PersonInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "value,desc,flag")] Models.sub_item sub_item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    sub_item.id = Guid.NewGuid();
                    sub_item.app_key = AppKey.personInfo;
                    sub_item.created_by = VinaphoneCommon.Auth.id().ToString();
                    sub_item.created_at = DateTime.Now;
                    db.sub_item.Add(sub_item);
                    db.SaveChanges();
                    this.success(VinaphoneCommon.Language.msgCreateSucsess);
                }
                //ViewBag.id_key = new SelectList(db.customers, "id", "full_name", sub_item.id_key);
            }
            catch (Exception)
            {
                this.danger(VinaphoneCommon.Language.msgCreateError);
            }
            return RedirectToAction("Create");
        }

        // GET: PersonInfo/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.sub_item sub_item = db.sub_item.Find(id);
            if (sub_item == null)
            {
                return HttpNotFound();
            }
            //ViewBag.id_key = new SelectList(db.customers, "id", "full_name", sub_item.id_key);
            return View(sub_item);
        }

        // POST: PersonInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,value,desc,flag")] Models.sub_item sub_item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(sub_item).State = EntityState.Modified;
                    sub_item.updated_by = VinaphoneCommon.Auth.id().ToString();
                    sub_item.updated_at = DateTime.Now;
                    db.SaveChanges();
                    this.success(VinaphoneCommon.Language.msgUpdateSucsess);
                    return RedirectToAction("Index");
                }
                else
                    this.danger(VinaphoneCommon.Language.msgError);
                //ViewBag.id_key = new SelectList(db.customers, "id", "full_name", sub_item.id_key);
            }
            catch (Exception)
            {
                this.danger(VinaphoneCommon.Language.msgUpdateError);
            }
            return RedirectToAction("Edit");
        }

        // GET: PersonInfo/Delete/5
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

        //// POST: PersonInfo/Delete/5
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
        public JsonResult update_flag(string uid)
        {
            try
            {
                string[] id = uid.Split(',');
                var flag = 0;
                foreach (var item in id)
                {
                    var tmp = Guid.Parse(item);
                    var rs = db.sub_item.Find(tmp);
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
                    var rs = db.sub_item.Find(tmp);
                    db.sub_item.Remove(rs);
                }
                db.SaveChanges();
                return Json(new { success = VinaphoneCommon.Language.msgDeleteSucsess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json(new { danger = VinaphoneCommon.Language.msgError }, JsonRequestBehavior.AllowGet); }
        }
    }
}
