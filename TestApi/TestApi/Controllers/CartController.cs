using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TestApi.Models;
using TestApi.ViewModels;

namespace TestApi.Controllers
{
    public class CartController : Controller
    {
        MyContext storeDB = new MyContext();

        //
        // GET: /ShoppingCart/

        public ActionResult Index()
        {
            var cart = ItemCart.GetCart(this.HttpContext);

            var viewModel = new CartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            
            return View(viewModel);
        }

        public ActionResult IndexOUT()
        {
            var cart = ItemCart.GetCart(this.HttpContext);

            var viewModel = new CartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };

            return View(viewModel);
        }

        //
        // GET: /Store/AddToCart/5

        public ActionResult AddToCart(int id, int count)
        {
            var addedItem = storeDB.Items
                .Single(a => a.Id == id);

            var cart = ItemCart.GetCart(this.HttpContext);

            cart.AddToCart(addedItem, count);

            return RedirectToAction("Index");
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5

        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {

            var cart = ItemCart.GetCart(this.HttpContext);
            int itemCount = cart.RemoveFromCart(id);

            var results = new CartRemoveViewModel
            {
                Message = Server.HtmlEncode( "Товар удален из корзины." ),
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };

            return Json(results);
        }

        //
        // GET: /ShoppingCart/CartSummary

        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ItemCart.GetCart(this.HttpContext);

            ViewData["CartCount"] = cart.GetCount();

            return PartialView("CartSummary");
        }

        //
        // GET: /Checkout/AddressAndPayment
        public ActionResult CreateOrder()
        {
                return View();
        }

        //
        // POST: /Checkout/AddressAndPayment
        [HttpPost]
        public ActionResult CreateOrder(Order order)
        {
            var rec = storeDB.Order.Where(a => a.LastName == order.LastName &&
            a.FirstName == order.FirstName && a.Company == order.Company).FirstOrDefault();

            //var order = new Order();
            //TryUpdateModel(order);


            try
            {
                var cart = ItemCart.GetCart(this.HttpContext);
                if (rec == null)
                {
                    storeDB.Order.Add(order);
                    storeDB.SaveChanges();

                    cart.CreateOrder(order, null);
                    return RedirectToAction("Complete",
                       new { id = order.Id });
                }

                //var cart = ItemCart.GetCart(this.HttpContext);
                cart.CreateOrder(order, rec.Id);

                return RedirectToAction("Complete",
                        new { id = rec.Id });
            }
            catch
            {
                return View(order);
            }
        }
        //
        // GET: /Checkout/AddressAndPayment
        public ActionResult CreateOrderOut()
        {
            return View();
        }
        //
        // POST: /Checkout/AddressAndPayment
        [HttpPost]
        public ActionResult CreateOrderOut(Order order)
        {
            var rec = storeDB.Order.Where(a => a.LastName == order.LastName &&
            a.FirstName == order.FirstName && a.Company == order.Company).FirstOrDefault();
            
            //var order = new Order();
            //TryUpdateModel(order);


            try
            {
                var cart = ItemCart.GetCart(this.HttpContext);
                if (rec == null)
                {
                    storeDB.Order.Add(order);
                    storeDB.SaveChanges();
                    
                    cart.CreateOrderOut(order, null);
                    return RedirectToAction("Complete",
                       new { id = order.Id });
                }

                //var cart = ItemCart.GetCart(this.HttpContext);
                cart.CreateOrderOut(order, rec.Id);

                return RedirectToAction("Complete",
                        new { id = rec.Id });
            }
            catch
            {
                return View(order);
            }
        }

        //
        // GET: /Checkout/Complete
        public ActionResult Complete(int id)
        {
            // Validate customer owns this order
            bool isValid = storeDB.Order.Any(
                o => o.Id == id);

            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }
    }
}