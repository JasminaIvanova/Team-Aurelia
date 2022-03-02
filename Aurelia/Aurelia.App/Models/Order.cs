using System.ComponentModel.DataAnnotations;

namespace Aurelia.App.Models
{
    public class Order : Base
    {
        public Order()
        {
            OrderDetails = new List<OrderDetails>();
        }
        public string UserId { get; set; }
        public AureliaUser  AureliaUser { get; set; }
        public DateTime date_placed { get; set; }
        public string Status { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string PaymentMethod { get; set; }
        [Required]
        public string Address { get; set; }
        public string DeliveryMethod { get; set; }

        public virtual List<OrderDetails> OrderDetails { get; set; }


    }
}
