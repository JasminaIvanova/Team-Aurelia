namespace Aurelia.App.Models
{
    public class ShoppingCartItem : Base
    {
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public int quantity { get; set; }
        public string ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}
