using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aurelia.App.Models
{
    public class OrderDetails : Base
    {
        [Display(Name = "Order")]
        public string? OrderId { get; set; }
        [Display(Name = "Product")]
        public string? ProductId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; } 

        public int Quantity { get; set; }
    }
}
