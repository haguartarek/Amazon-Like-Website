using WebApplication2.Models;

namespace WebApplication2.ViewModels
{
    public class ProdCartCartpViewModel
    {
        public IReadOnlyList<Product> Products { get; }
        public IReadOnlyList<Cart> Carts { get; }
        public int Cartid { get; }
        public IReadOnlyList<Cart_products> Cart_Productss { get; }
        public decimal CartValue { get; }
        
        public ProdCartCartpViewModel(IReadOnlyList<Product> products, IReadOnlyList<Cart> carts, IReadOnlyList<Cart_products> cart_productss, int cartid,  decimal cartvalue)
        {
            this.Products = products;
            this.Carts = carts;
            this.Cart_Productss = cart_productss;
            this.Cartid = cartid;
            this.CartValue = cartvalue;
        }
    }
}
