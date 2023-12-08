using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Cart_products
    {
        [Key]
        public int CartProductId { get; set; }
        [ForeignKey(nameof(Cart))]
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime InsertionDate { get; set; }
        public int CategoryId { get; set; }

        public decimal CatProductPrice { get; set; }



    }
}
