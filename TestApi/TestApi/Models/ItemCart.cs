using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TestApi.Models
{
    public partial class ItemCart
    {
        private MyContext storeDB = new MyContext();

        string ShoppingCartId { get; set; }

        public const string CartSessionKey = "CartId";

        public static ItemCart GetCart(HttpContextBase context)
        {
            var cart = new ItemCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        public static ItemCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        public void AddToCart(Item item, int count)
        {
            var cartItem = storeDB.Carts.SingleOrDefault(c => c.CartId == ShoppingCartId 
            && c.ItemId == item.Id);

            if (cartItem == null)
            {
                cartItem = new Cart
                {
                    ItemId = item.Id,
                    ItemCode = item.Code,
                    ItemTitle = item.Title,
                    CartId = ShoppingCartId,
                    Count = count
                };

                storeDB.Carts.Add(cartItem);
            }
            else
            {
                cartItem.Count += count;
            }

            storeDB.SaveChanges();
        }

        public void AddToCartFromWarehouse(Warehouse warehouse, int count)
        {
            var cartItem = storeDB.Carts.SingleOrDefault(c => c.CartId == ShoppingCartId
            && c.ItemId == warehouse.ItemId);
            int itemCount = storeDB.Warehouses.Where(a => a.ItemId == warehouse.ItemId).Select(a => a.Count).First();

            if (cartItem == null && count <= itemCount)
            {
              
                cartItem = new Cart
                {
                    
                    ItemId = warehouse.ItemId,
                    ItemCode = warehouse.Item.Code,
                    ItemTitle = warehouse.Item.Title,
                    CartId = ShoppingCartId,
                    Count = count
                };

                storeDB.Carts.Add(cartItem);
            }
            else 
            {
                if (count <= itemCount)
                {
                    cartItem.Count+= count;
                    if(cartItem.Count > itemCount)
                    {
                        cartItem.Count = itemCount;
                    }
                }
                else
                {
                    //иначе выставляем максимальное значениe
                    count = itemCount;
                }
            }
          
            storeDB.SaveChanges();
        }

        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = storeDB.Carts.Single(cart => cart.CartId == ShoppingCartId
                && cart.RecordId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    storeDB.Carts.Remove(cartItem);
                }

                storeDB.SaveChanges();
            }

            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = storeDB.Carts.Where(cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                storeDB.Carts.Remove(cartItem);
            }

            storeDB.SaveChanges();
        }

        public List<Cart> GetCartItems()
        {
            return storeDB.Carts.Where(cart => cart.CartId == ShoppingCartId).ToList();
        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in storeDB.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();

            // Return 0 if all entries are null
            return count ?? 0;
        }

        public decimal GetTotal()
        {
            // Multiply album price by count of that album to get 
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            decimal? total = (from cartItems in storeDB.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count).Sum();
            return total ?? decimal.Zero;
        }

        public int CreateOrder(Order order, int? id)
        {
            int orderTotal = 0;
            order.OrderDetails = new List<OrderDetail>();

            var cartItems = GetCartItems();

            // Iterate over the items in the cart, adding the order details for each
            foreach (var item in cartItems)
            {
                var rec = storeDB.Warehouses.Where(a => a.ItemId == item.ItemId).FirstOrDefault();
                if(rec == null)
                {
                    var warehouse = new Warehouse
                    {
                        ItemId = item.ItemId,
                        Count = item.Count
                    };

                    storeDB.Warehouses.Add(warehouse);
                }
                else
                {
                    rec.Count += item.Count;
                    storeDB.Entry(rec).State = EntityState.Modified;
                }

                var orderDetail = new OrderDetail();

                if (id == null)
                {
                    orderDetail.ItemCode = item.ItemCode;
                    orderDetail.ItemTitle = item.ItemTitle;
                    orderDetail.Count = item.Count;
                    orderDetail.DateCreate = DateTime.Now;
                    orderDetail.Status = true;

                }
                else
                {
                    orderDetail.ItemCode = item.ItemCode;
                    orderDetail.ItemTitle = item.ItemTitle;
                    orderDetail.Count = item.Count;
                    orderDetail.DateCreate = DateTime.Now;
                    orderDetail.Status = true;
                }

                orderTotal += item.Count;
                order.OrderDetails.Add(orderDetail);
                storeDB.OrderDetails.Add(orderDetail);

            }
            
            storeDB.SaveChanges();

            // удаление корзины
            EmptyCart();

            // Return the OrderId as the confirmation number
            return orderTotal;
        }

        public int CreateOrderOut(Order order, int? id)
        {
            order.OrderDetails = new List<OrderDetail>();

            var cartItems = GetCartItems();

            // Iterate over the items in the cart, adding the order details for each
            foreach (var item in cartItems)
            {
                var warehouse = storeDB.Warehouses.Where(a => a.ItemId == item.ItemId).FirstOrDefault();
                warehouse.ItemId = item.ItemId;

                int count = storeDB.Warehouses.Where(a => a.ItemId == item.ItemId).Select(a => a.Count).First();

                var orderDetail = new OrderDetail();

                if (id == null)
                {
                    //var orderDetail = new OrderDetail

                    orderDetail.ItemCode = item.ItemCode;
                    orderDetail.ItemTitle = item.ItemTitle;
                    orderDetail.Count = item.Count;
                    orderDetail.DateCreate = DateTime.Now;
                    orderDetail.Status = false;
                    
                }
                else
                {
                    orderDetail.ItemCode = item.ItemCode;
                    orderDetail.ItemTitle = item.ItemTitle;
                    orderDetail.Count = item.Count;
                    orderDetail.DateCreate = DateTime.Now;
                    orderDetail.Status = false;
                    
                }

                

                warehouse.Count -= item.Count; 
                if(warehouse.Count > 0 && warehouse.Count != 0)
                {
                    storeDB.Entry(warehouse).State = EntityState.Modified;
                    order.OrderDetails.Add(orderDetail);
                    storeDB.OrderDetails.Add(orderDetail);
                }
                else if(warehouse.Count == 0)
                {
                    //удаляем если значение 0         
                    order.OrderDetails.Add(orderDetail);
                    storeDB.OrderDetails.Add(orderDetail);
                    storeDB.Warehouses.Remove(warehouse);
                }
                else
                {
                    //если меньше 0 то выставляем максимальное и удаляем
                    warehouse.Count = count;
                    order.OrderDetails.Add(orderDetail);
                    storeDB.OrderDetails.Add(orderDetail);
                    storeDB.Warehouses.Remove(warehouse);

                }

            }

            storeDB.SaveChanges();

            // удаление корзины
            EmptyCart();

            // Return the OrderId as the confirmation number
            return order.Id;
        }
        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] = context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();

                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }

            return context.Session[CartSessionKey].ToString();
        }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            var shoppingCart = storeDB.Carts.Where(c => c.CartId == ShoppingCartId);

            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            storeDB.SaveChanges();
        }
    }
}