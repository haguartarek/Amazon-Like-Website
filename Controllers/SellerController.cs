using System.Data.OleDb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NuGet.Protocol.Plugins;
using WebApplication2.Models;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
   [Authorize(Roles = "Seller")]
    public class SellerController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;

        public SellerController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        Entity context = new Entity();
        string currentseller;
        public IActionResult Index(string sellerId)
        {
            var productss = context.Products.ToList();
            var categories = context.Categories.ToList();

            // Create a view model.
            var viewModel = new ProductCategoryViewModel(productss, categories);
            ViewData["ProductCategoryViewModel"] = viewModel;
            // string intsellerid = TempData["CurrentSellerIdIndex"].ToString();
            // currentseller = sellerId;
            List<Product> products=context.Products.Where(e=>e.SellerId==sellerId).ToList();
            return View(products);
        }
        [HttpGet]
        public ActionResult AddProduct()
        {
            // Get the list of products and categories.
            var products = context.Products.ToList();
            var categories = context.Categories.ToList();

            // Create a view model.
            var viewModel = new ProductCategoryViewModel(products, categories);

            // Pass the view model to the view.
            ViewData["ProductCategoryViewModel"] = viewModel;

            return PartialView("_AddProductModalPartial");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAddNewProductAsync(Product product, IFormFile ImageFile, int hiddenCategoryId)
        {
            string intsellerid;
            
         //   intsellerid = TempData["CurrentSellerId"].ToString();
            product.SellerId = TempData["CurrentSellerId"].ToString();
            currentseller = product.SellerId;
            product.CategoryId = hiddenCategoryId;
          //  product.CategoryId = CategoryId;
                if (product != null)
                {
                using (var memoryStream = new MemoryStream())
                {
                    ImageFile.CopyTo(memoryStream);
                    product.Image = memoryStream.ToArray();
                }
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\images", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                }
                if (product.RequiresExpirationDate == false)
                {
                    product.ExpirationDate = null;
                }
                else
                {
                    var expirationDate = product.ExpirationDate;
                    if (expirationDate != null && expirationDate < DateTime.Now)
                    {
                        product.IsActive = false;
                    }
                }
                context.Products.Add(product);
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products ON");

                    context.SaveChanges();
                    context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Products OFF");
                    //refresh
                    return RedirectToAction("Index");
                    //  return PartialView("_AddStTeacherModalPartial");
                
            }
           

            return PartialView("_AddProductModalPartial", product);
        }
        public IActionResult ActdisProduct(int productId)
        {
            // Get the seller from the database

            Product product = context.Products.Find( productId);
            // Check if the seller is active
            if (product.IsActive == true)
            {
              
                product.IsActive = false;
               
            }
            else
            {
                product.IsActive = true;
            }
            string sellerid = product.SellerId;
            context.SaveChanges();
            return RedirectToAction("Index", new { sellerId = sellerid });
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {

            var products = context.Products.ToList();
            var categories = context.Categories.ToList();

            // Create a view model.
            var viewModel = new ProductCategoryViewModel(products, categories);

            // Pass the view model to the view.
            ViewData["ProductCategoryViewModel"] = viewModel;
            Product productmodel = context.Products.FirstOrDefault(e => e.ProductId == id);
            return PartialView("_EditProductModalPartial", productmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveEdit(Product newProduct, int id, int hiddenCategoryId)
        {

            Product oldproduct = context.Products.Find(id);
            var cartproducts = context.Cart_Productss.Where(p => p.ProductId == id);
            var old = oldproduct;
            

            if (oldproduct != null)
                    {
                        oldproduct.Name = newProduct.Name;
                        oldproduct.Price = newProduct.Price;
                        oldproduct.Quantity = newProduct.Quantity;
                        oldproduct.IsActive = newProduct.IsActive;
                        oldproduct.Description = newProduct.Description;
                        oldproduct.CategoryId = newProduct.CategoryId;
                        oldproduct.ExpirationDate = newProduct.ExpirationDate;

                if (oldproduct.RequiresExpirationDate == false)
                {
                    oldproduct.ExpirationDate = null;
                }
                else
                {
                    var expirationDate = oldproduct.ExpirationDate;
                    if (expirationDate != null && expirationDate < DateTime.Now)
                    {
                        oldproduct.IsActive = false;
                    }
                }
                context.SaveChanges();
                        return RedirectToAction("Index");

                    }

                
              
            

            // return RedirectToAction("Index");
            return PartialView("_EditProductModalPartial", newProduct);
        }
        public IActionResult DeleteProduct(int id)
        {
            
            Product prodmodel = context.Products.FirstOrDefault(e => e.ProductId == id);
            var price = prodmodel.Price;
            string sellerid = prodmodel.SellerId;
            var cartproducts = context.Cart_Productss.Where(p => p.ProductId == id);
            foreach (var cartproduct in cartproducts)
            {
                var quantity = cartproduct.Quantity;
                Cart cart = context.Carts.Find(cartproduct.CartId);
                cart.Value = cart.Value - price * quantity;
                context.Update(cart);
                context.Cart_Productss.Remove(cartproduct);
            }
            if (prodmodel == null)
            {
                return NotFound();
            }

            context.Products.Remove(prodmodel);
            context.SaveChanges();

            return RedirectToAction("Index", new { sellerId = sellerid });
        }

    }
}
