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

namespace TestApi.Controllers
{
    public class WarehousesController : Controller
    {
        private MyContext db = new MyContext();

        // GET: Warehouses
        public async Task<ActionResult> Index()
        {
            var warehouses = db.Warehouses.Include(w => w.Item);
            return View(await warehouses.ToListAsync());
        }


        public ActionResult AddToCart(int? id)
        {   
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse warehouse = db.Warehouses.Where(a => a.ItemId == id).FirstOrDefault();

            if (warehouse == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemId = new SelectList(db.Items, "Id", "Title", warehouse.ItemId);
            return View(warehouse);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(int Id, int count, Warehouse warehouse)
        {
            ViewBag.ItemId = new SelectList(db.Items, "Id", "Title", warehouse.ItemId);
            if (ModelState.IsValid)
            {
                Item item = db.Items.Where(a => a.Id == Id).FirstOrDefault();
                var addedItem = db.Warehouses.Single(a => a.ItemId == Id);

                addedItem.Item.Title = item.Title;
                addedItem.Item.Code = item.Code;

                var cart = ItemCart.GetCart(this.HttpContext);

                cart.AddToCartFromWarehouse(addedItem, count);
                return RedirectToAction("IndexOUT", "Cart");
            }
            return HttpNotFound();
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
