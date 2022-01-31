namespace Aurelia.App.Models
{
    public class ShoppingCart : Base
    {
        public string UserId { get; set; }
        public AureliaUser AureliaUser { get; set; }
        public int Count { get; set; }
        public int TotalAmount { get; set; }
    }
}
