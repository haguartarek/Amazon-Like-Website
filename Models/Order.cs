using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        [ForeignKey("AspNetUsers")]
        public string BuyerId { get; set; }

        public DateTime Date { get; set; }
        public decimal Value { get; set; }
    }
}
