using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestApi.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required]
        [Display(Name ="Количество")]
        public int Count { get; set; }

        public Item Item { get; set; }
    }
}