using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Order_Items
    {
        [Key]
        public int OrderItemsId { get; set; }
        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        [ForeignKey(nameof(OldProduct))]
        public int OldProductId { get; set; }

    }
}
