using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TestApi.Models;
using System.Collections;

namespace TestApi.Controllers
{
    public class OrderDetailsController : Controller
    {
        private MyContext db = new MyContext();


        public ActionResult Index(DateTime? start)
        {
            
            if (start != null)
            {
                //дата конца периода
                DateTime stop = start.Value.AddMonths(1);
                //дата начала предыдущего периода
                DateTime PrePstart = start.Value.AddMonths(-8);

                var result = db.OrderDetails.Where(a => a.DateCreate >= PrePstart && a.DateCreate <= stop).AsEnumerable().GroupBy(o => new
                {
                    o.ItemCode
                })
            .Select(grp => new OrderDetail
            {
                ItemCode = grp.Key.ItemCode,
                ItemTitle = grp.Where(a => a.ItemCode == grp.Key.ItemCode).Select(a => a.ItemTitle).First(),
                CountIn = grp.Where(a => a.Status == true && a.DateCreate >= start && a.DateCreate <= stop).Sum(c => c.Count),
                CountOut = grp.Where(a => a.Status == false && a.DateCreate >= start && a.DateCreate <= stop).Sum(c => c.Count),
                Count = grp.Where(a => a.DateCreate >= start && a.DateCreate <= stop).Sum(o => o.Count),

                Pstart = grp.Where(a => a.DateCreate >= PrePstart && a.DateCreate <= start && a.Status == true).Sum(a => a.Count) - 
                grp.Where(a => a.DateCreate >= PrePstart && a.DateCreate <= start && a.Status == false).Sum(a => a.Count) +
                grp.Where(a => a.DateCreate >= PrePstart && a.DateCreate <= PrePstart && a.Status == true).Sum(a => a.Count) -
                grp.Where(a => a.DateCreate >= PrePstart && a.DateCreate <= PrePstart && a.Status == false).Sum(a => a.Count),

                Pfinish = grp.Where(a => a.DateCreate >= start && a.DateCreate <= stop && a.Status == true).Sum(a => a.Count) - 
                grp.Where(a => a.DateCreate >= start && a.DateCreate <= stop && a.Status == false).Sum(a => a.Count) + 
                grp.Where(a => a.DateCreate >= PrePstart && a.DateCreate <= start && a.Status == true).Sum(a => a.Count) - 
                grp.Where(a => a.DateCreate >= PrePstart && a.DateCreate <= start && a.Status == false).Sum(a => a.Count)

            }).ToList();

                return View(result);
            }
            //если не чего не ввели просто выводим таблицу
            return View(db.OrderDetails.ToList());
        }

        // GET: OrderDetails/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderDetail orderDetail = await db.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return HttpNotFound();
            }
            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            OrderDetail orderDetail = await db.OrderDetails.FindAsync(id);
            db.OrderDetails.Remove(orderDetail);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
