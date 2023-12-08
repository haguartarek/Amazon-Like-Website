using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models
{
    public class Entity : IdentityDbContext<ApplicationUser>
    {
        public Entity()
        {

        }
        public Entity(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Order_Items> Order_Itemss { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Cart_products> Cart_Productss { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<OldProduct> OldProducts { get; set; }
        public DbSet<ActivationRequest> ActivationRequests { get; set; }
        public DbSet<CategoryDiscount> CategoryDiscounts { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data source=DESKTOP-QPN9GPL\\SQLEXPRESS; Initial Catalog =ProjectFinalDatabase; Integrated Security=True");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
