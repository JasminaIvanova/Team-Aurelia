﻿using System.ComponentModel.DataAnnotations;

namespace Aurelia.App.Models
{
    public class ProductCategories : Base
    {
        [Display(Name = "Product Category")]
        public string Name { get; set; }
    }
}
