using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TestApi.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Имя")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name ="Фамилия")]
        public string LastName { get; set; }

        [Required]
        [Display(Name ="Компания")]
        public string Company { get; set; }
        
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}