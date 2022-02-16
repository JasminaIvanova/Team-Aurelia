using System.ComponentModel.DataAnnotations;

namespace Aurelia.App.Models
{
    public class Product : Base
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public string? ProductCategoryId { get; set; }

        public virtual ProductCategory? ProductCategory { get; set; }

        public string Brand { get; set; }
        public string? Image { get; set; }   

    }
}
