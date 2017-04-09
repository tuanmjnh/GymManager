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
    public class CardPersonController : BaseController
    {
        // GET: CardPerson
        public ActionResult Index(Guid? customer, int? flag, string order, string currentFilter, string searchString, int? page, string export)
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
                ViewBag.customer = customer;

                var rs = from d in db.items where d.app_key == AppKey.cardPerson select d;
                rs = rs.Where(d => d.id_key.Contains(customer + ","));

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
                    case "price_asc":
                        rs = rs.OrderBy(d => d.price);
                        break;
                    case "time_desc":
                        rs = rs.OrderBy(d => d.created_at);
                        break;
                    default:
                        rs = rs.OrderByDescending(d => d.created_at);
                        break;
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

        // GET: CardPerson/Details/5
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

        // GET: CardPerson/Create
        public ActionResult Create(Guid? customer)
        {
            ViewBag.Card = db.items.Where(d => d.app_key == AppKey.cardManager && d.flag > 0).ToList();
            return View();
        }

        // POST: CardPerson/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_key,code_key,title,desc,price_old,price,flag")] Models.item item, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    item.id = Guid.NewGuid();
                    item.app_key = AppKey.cardPerson;
                    item.id_key = collection["id"] + "," + item.id_key;
                    item.title = collection["title[]"];
                    item.created_by = VinaphoneCommon.Auth.id().ToString();
                    item.created_at = DateTime.Now;
                    db.items.Add(item);

                    db.SaveChanges();
                    this.success(VinaphoneCommon.Language.msgUpdateSucsess);
                    return RedirectToAction("Edit/" + collection["id"]);
                }
                else
                    this.danger(VinaphoneCommon.Language.msgError);
            }
            catch (Exception ex)
            {
                this.danger(VinaphoneCommon.Language.msgUpdateError);
            }
            return RedirectToAction("Edit/" + collection["id"]);
        }

        // GET: CardPerson/Edit/5
        public ActionResult Edit(Guid? id, Guid? customer)
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

        // POST: CardPerson/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,app_key,id_key,code_key,title,desc,quantity,quantity_total,price_old,price,created_by,created_at,updated_by,updated_at,flag,extras")] Models.item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(item);
        }

        // GET: CardPerson/Delete/5
        //public ActionResult Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Models.item item = db.items.Find(id);
        //    if (item == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(item);
        //}

        //// POST: CardPerson/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    Models.item item = db.items.Find(id);
        //    db.items.Remove(item);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        private Models.item GetCardManager(Guid id)
        {
            try
            {
                return db.items.Where(d => d.id == id).FirstOrDefault();
            }
            catch (Exception) { throw; }
        }
    }
}
