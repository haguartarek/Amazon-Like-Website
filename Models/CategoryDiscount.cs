using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class CategoryDiscount
    {
        public int CategoryDiscountId { get; set; }
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public decimal DiscountPercentage { get; set; }
    }
}
