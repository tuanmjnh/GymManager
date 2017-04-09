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
    public class CategoryController : BaseController
    {
        // GET: Category
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

                var rs = getGroup(AppKey.product, "0", Guid.Empty).AsQueryable();

                if (!String.IsNullOrEmpty(searchString))
                    rs = rs.Where(d =>
                    d.title.Contains(searchString));

                if (flag == 0) rs = rs.Where(d => d.flag == 0);
                else rs = rs.Where(d => d.flag == 1);

                //switch (order)
                //{
                //case "value_desc":
                //    rs = rs.OrderByDescending(d => d.title);
                //    break;
                //default:
                //    rs = rs.OrderBy(d => d.title);
                //    break;
                //case "id_asc":
                //    rs = rs.OrderByDescending(d => d.id);
                //    break;
                //default:
                //    rs = rs.OrderBy(d => d.id);
                //    break;
                //}

                //Export to any
                if (!String.IsNullOrEmpty(export))
                {
                    TM.Exports.ExportExcel(TM.Exports.LINQToDataTable(rs.ToList()), "Danh sách danh mục");
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

        // GET: Category/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.group group = db.groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            ViewBag.group = getGroup(AppKey.product, "0", Guid.Empty);
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "title,parent_id,desc,flag")] Models.group group)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //ViewBag.group
                    group.id = Guid.NewGuid();

                    //Parents_ID
                    group.parents_id = ",";
                    group.level = 0;
                    if (group.parent_id != "0")
                    {
                        var parent_id = Guid.Parse(group.parent_id);
                        var tmp = db.groups.Where(d => d.id == parent_id && d.flag > 0).FirstOrDefault();
                        group.parents_id = tmp.parents_id + parent_id + ",";
                        group.level = tmp.level + 1;
                    }

                    group.app_key = AppKey.product;
                    group.created_by = VinaphoneCommon.Auth.id().ToString();
                    group.created_at = DateTime.Now;

                    db.groups.Add(group);
                    db.SaveChanges();
                    this.success(VinaphoneCommon.Language.msgCreateSucsess);
                }
                else this.danger(VinaphoneCommon.Language.msgError);
            }
            catch (Exception ex)
            {
                this.danger(VinaphoneCommon.Language.msgCreateError);
            }
            return RedirectToAction("Create");
        }

        // GET: Category/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.group group = db.groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            ViewBag.group = getGroup(AppKey.product, "0", id.Value);
            return View(group);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,title,parent_id,desc,flag")] Models.group group_tmp)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var group = db.groups.Find(group_tmp.id);

                    //Parents_ID
                    if (group_tmp.parent_id != "0")
                    {
                        var parent_id = Guid.Parse(group_tmp.parent_id);
                        var tmp = db.groups.Where(d => d.id == parent_id && d.flag > 0).FirstOrDefault();
                        group.parents_id = tmp.parents_id + parent_id + ",";
                        group.level = tmp.level + 1;
                    }

                    group.parent_id = group_tmp.parent_id;
                    group.title = group_tmp.title;
                    group.desc = group_tmp.desc;
                    group.flag = group_tmp.flag;
                    group.updated_by = VinaphoneCommon.Auth.id().ToString();
                    group.updated_at = DateTime.Now;

                    db.Entry(group).State = EntityState.Modified;
                    db.SaveChanges();
                    this.success(VinaphoneCommon.Language.msgCreateSucsess);
                    return RedirectToAction("Index");
                }
                else this.danger(VinaphoneCommon.Language.msgError);
            }
            catch (Exception ex)
            {
                this.danger(VinaphoneCommon.Language.msgCreateError);
            }
            return RedirectToAction("Edit");
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
                    var rs = db.groups.Find(tmp);
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
                    var rs = db.groups.Find(tmp);
                    db.groups.Remove(rs);
                }
                db.SaveChanges();
                return Json(new { success = VinaphoneCommon.Language.msgDeleteSucsess }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception) { return Json(new { danger = VinaphoneCommon.Language.msgError }, JsonRequestBehavior.AllowGet); }
        }
        // GET: Category/Delete/5
        //public ActionResult Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Models.group group = db.groups.Find(id);
        //    if (group == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(group);
        //}

        //// POST: Category/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    Models.group group = db.groups.Find(id);
        //    db.groups.Remove(group);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

    }
}
