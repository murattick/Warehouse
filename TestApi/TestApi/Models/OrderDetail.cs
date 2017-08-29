using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TestApi.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        [Display(Name = "Код товара")]
        public string ItemCode { get; set; }

        public string ItemTitle { get; set; }

        [Display(Name = "Количество поступившего")]
        public int CountIn { get; set; }

        [Display(Name = "Количество ушедшего")]
        public int CountOut { get; set; }

        [Display(Name = "Количество на начало периода")]
        public int Pstart { get; set; }

        [Display(Name = "Количество на Конец периода")]
        public int Pfinish { get; set; }

        [Required]
        [Display(Name ="Количество")]
        public int Count { get; set; }

        [Required]
        [Display(Name = "Дата создания")]
        public DateTime DateCreate { get; set; }

        public bool Status { get; set; }

    }
}