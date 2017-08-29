using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestApi.Models
{
    public class Item
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Наименование")]
        public string Title { get; set; }

        [Required]
        public string Code { get; set; }

        public ICollection<Warehouse> Warehouses { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}