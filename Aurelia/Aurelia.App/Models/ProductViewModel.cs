using System.ComponentModel.DataAnnotations;

namespace Aurelia.App.Models
{
    public class ProductViewModel
    {

        [Required(ErrorMessage = "Please enter Product Name")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Please enter Product Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Please enter Product Price")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Please choose category")]
        public string? ProductCategoryId { get; set; }

        public virtual ProductCategories? ProductCategory { get; set; }

        [Required(ErrorMessage = "Please enter Product Brand")]
        public string Brand { get; set; }
        [Required(ErrorMessage = "Please choose Product Image")]
        public IFormFile ProductImage { get; set; }
    }
}
