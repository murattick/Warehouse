using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TestApi.Models
{
    public class Dbinitializer: DropCreateDatabaseAlways<MyContext>
    {
        protected override void Seed(MyContext context)
        {
            context.OrderDetails.Add(new OrderDetail { ItemCode = "000123", Count = 5, Status = true, DateCreate = DateTime.Parse("01-07-2017") });
            context.OrderDetails.Add(new OrderDetail { ItemCode = "000321", Count = 5, Status = true, DateCreate = DateTime.Parse("01-07-2017") });
            context.OrderDetails.Add(new OrderDetail { ItemCode = "000124", Count = 7, Status = false, DateCreate = DateTime.Parse("01-09-2017") });
            context.OrderDetails.Add(new OrderDetail { ItemCode = "000123", Count = 5, Status = true, DateCreate = DateTime.Parse("01-09-2017") });
            context.OrderDetails.Add(new OrderDetail { ItemCode = "000123", Count = 10, Status = false, DateCreate = DateTime.Parse("01-10-2017") });
            context.OrderDetails.Add(new OrderDetail { ItemCode = "000321", Count = 5, Status = true, DateCreate = DateTime.Parse("01-10-2017") });
            context.OrderDetails.Add(new OrderDetail { ItemCode = "000321", Count = 5, Status = false, DateCreate = DateTime.Parse("01-10-2017") });


            base.Seed(context);
        }
    }
}