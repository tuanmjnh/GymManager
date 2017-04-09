using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TM;
using PagedList;
using Dapper;

namespace GymManager.Controllers
{
    [Filters.AuthVinaphone()]
    public class ReportController : BaseController
    {
        // GET: Report
        public ActionResult Index()
        {

            return View();
        }
        private dynamic getBill(string datetime, int flag)
        {
            try
            {
                var date = datetime.Split('/');
                string sql = @"SELECT SUM(total_item) AS total_item,SUM(total_quantity) AS total_quantity,SUM(total_price) AS total_price FROM bill WHERE id is not null";
                if (flag >= 0)
                    sql += " AND flag=" + flag;
                if (date.Length == 1)
                    sql += " AND YEAR(created_at)=" + date[0];
                else if (date.Length == 2)
                    sql += " AND MONTH(created_at)=" + date[0] + " AND YEAR(created_at)=" + date[1];
                else if (date.Length >= 3)
                    sql += " AND DAY(created_at)=" + date[0] + " AND MONTH(created_at)=" + date[1] + " AND YEAR(created_at)=" + date[2];
                return Connection().Query(sql).FirstOrDefault();
            }
            catch (Exception) { throw; }
        }
        private dynamic getCardPerson(string datetime)
        {
            try
            {
                var date = datetime.Split('/');
                string sql = @"SELECT SUM(price) AS paid,SUM(price_old) AS NotPay,SUM(CAST(title AS DECIMAL(10,2))) AS pay
                        FROM item WHERE app_key='" + AppKey.cardPerson + "'";
                if (date.Length == 1)
                    sql += " AND YEAR(created_at)=" + date[0];
                else if (date.Length == 2)
                    sql += " AND MONTH(created_at)=" + date[0] + " AND YEAR(created_at)=" + date[1];
                else if (date.Length >= 3)
                    sql += " AND DAY(created_at)=" + date[0] + " AND MONTH(created_at)=" + date[1] + " AND YEAR(created_at)=" + date[2];
                return Connection().Query(sql).FirstOrDefault();
            }
            catch (Exception) { throw; }
        }
        private dynamic getQtyClient(string datetime)
        {
            try
            {
                var date = datetime.Split('/');
                string sql = @"SELECT COUNT(id) AS quantity FROM item WHERE app_key='" + AppKey.cardPerson + "' AND code_key is null";
                if (date.Length == 1)
                    sql += " AND YEAR(created_at)=" + date[0];
                else if (date.Length == 2)
                    sql += " AND MONTH(created_at)=" + date[0] + " AND YEAR(created_at)=" + date[1];
                else if (date.Length >= 3)
                    sql += " AND DAY(created_at)=" + date[0] + " AND MONTH(created_at)=" + date[1] + " AND YEAR(created_at)=" + date[2];
                return Connection().Query(sql).FirstOrDefault();
            }
            catch (Exception) { throw; }
        }
        private dynamic getQtyCustomer(string datetime)
        {
            try
            {
                var date = datetime.Split('/');
                string sql = @"SELECT COUNT(id) AS quantity FROM sub_item WHERE app_key='" + AppKey.cardPerson + "'";
                if (date.Length == 1)
                    sql += " AND YEAR(created_at)=" + date[0];
                else if (date.Length == 2)
                    sql += " AND MONTH(created_at)=" + date[0] + " AND YEAR(created_at)=" + date[1];
                else if (date.Length >= 3)
                    sql += " AND DAY(created_at)=" + date[0] + " AND MONTH(created_at)=" + date[1] + " AND YEAR(created_at)=" + date[2];
                return Connection().Query(sql).FirstOrDefault();
            }
            catch (Exception) { throw; }
        }
        [AllowAnonymous]
        public JsonResult Report(string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    return Json(new
                    {
                        bill = new { billPaid = getBill(data, 1), billNotPay = getBill(data, 0), billPay = getBill(data, -1) },
                        cardPerson = getCardPerson(data),
                        QtyCustomer = getQtyCustomer(data),
                        QtyClient = getQtyClient(data),
                        success = VinaphoneCommon.Language.msgSucsess
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(VinaphoneCommon.Language.msgSucsess, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { danger = ex.Message }, JsonRequestBehavior.AllowGet); }//"Xử lý lỗi vui lòng thử lại!" 
        }
    }
}