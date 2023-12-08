using WebApplication2.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.ViewModels
{
    public class AddProductViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int SellerId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }
        public byte[] Image { get; set; }
    }
}
