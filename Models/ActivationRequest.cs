using System.ComponentModel.DataAnnotations.Schema;
namespace WebApplication2.Models
{
    public class ActivationRequest
    {
        public int ActivationRequestId { get; set; }
        public DateTime RequestDate { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("AspNetUsers")]
        public string SellerId { get; set; }
        public string SellerName { get; set; }
    }
}
