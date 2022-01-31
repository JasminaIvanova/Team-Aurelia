namespace Aurelia.App.Models
{
    public class Order : Base
    {
        public string UserId { get; set; }
        public AureliaUser  AureliaUser { get; set; }
        public DateTime date_placed { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string Address { get; set; }
        public string DeliveryMethod { get; set; }


    }
}
