﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Magaz.Models
{
    public class Product
    {
        public Product()
        {

            SqrtM = 1;
        }
        [Key] public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        public string? Image { get; set; }
        [Display(Name="Category Type")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [Display(Name = "Application Type")]
        public int ApplicationId { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual ApplicationType? ApplicationType { get; set; }

        [NotMapped] //не добавит в БД
        [Range(1,1000)]
        public int SqrtM { get; set; }
    }
}
