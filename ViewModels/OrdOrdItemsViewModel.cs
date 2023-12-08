using WebApplication2.Models;
namespace WebApplication2.ViewModels
{
    public class OrdOrdItemsViewModel
    {
        public IReadOnlyList<Order> Orders { get; }
        public IReadOnlyList<Order_Items> Order_Items { get; }
        public IReadOnlyList<Product> Products { get; }
        public IReadOnlyList<OldProduct> OldProducts { get; }
        public OrdOrdItemsViewModel(IReadOnlyList<Order> orders, IReadOnlyList<Order_Items> order_items, IReadOnlyList<Product> products, IReadOnlyList<OldProduct> oldproducts)
        {
            this.Orders = orders;
            this.Order_Items = order_items;
            this.Products = products;
            this.OldProducts = oldproducts;
        }

        public IEnumerable<Order_Items> GetOrderItemsByOrderId(int orderId)
        {
            return this.Order_Items.Where(o => o.OrderId == orderId);
        }
    }
}