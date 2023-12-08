using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        [ForeignKey("AspNetUsers")]
        public string BuyerId { get; set; }
        public decimal Value { get; set; }
    }
}
