namespace Aurelia.App.Models
{
    public class Product : Base
    {
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public string ProductTypeId { get; set; }
        public ProductCategories ProductCategory { get; set; }

        public string Brand { get; set; }
        public string Image { get; set; }   

    }
}
