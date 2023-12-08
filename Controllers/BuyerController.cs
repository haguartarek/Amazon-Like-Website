using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using WebApplication2.Models;
using WebApplication2.ViewModels;


namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Buyer")]
    public class BuyerController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;

        public BuyerController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        Entity context = new Entity();
        string BuyerId;
       
        public IActionResult Index(string buyerid)
        {
            // BuyerId = TempData["CurrentBuyerId2"].ToString();
            BuyerId = buyerid;
            int currentcartid;
            Cart cart = context.Carts.FirstOrDefault(c => c.BuyerId == buyerid);
            if (cart == null)
            {
                Cart cart2 = new Cart();
                cart2.BuyerId = buyerid;
                context.Carts.Add(cart2);
                context.SaveChanges();

            }
            var categories = context.Categories.ToList();

            List<Product> activeproducts = context.Products
     .Where(c => c.IsActive && c.Quantity > 0)
     .ToList();

            List<string> activeSellerIds = activeproducts.Select(p => p.SellerId).Distinct().ToList();

            List<ApplicationUser> activeSellers = context.Users
                .Where(s => activeSellerIds.Contains(s.Id) && s.EmailConfirmed)
                .ToList();

            activeproducts = activeproducts
                .Where(p => activeSellerIds.Contains(p.SellerId))
                .ToList();

            // Check expiration date and update product status
            DateTime currentDate = DateTime.UtcNow;

            foreach (var product in activeproducts)
            {
                if (product.ExpirationDate != null && product.ExpirationDate < currentDate)
                {
                    product.IsActive = false;
                    activeproducts.Remove(product);
                  
                }
            }
            var viewModel = new ProductCategoryViewModel(activeproducts, categories);
            ViewData["ProductCategoryViewModel3"] = viewModel;
            return View("BuyerIndex", viewModel);

        }
        public IActionResult ViewCart(string buyerid)
        {
            TempData["CurrentBuyerId2"] = buyerid;
            int currentcartid;
            Cart cart3 = context.Carts.FirstOrDefault(c => c.BuyerId == buyerid);
            currentcartid = cart3.CartId;
            var carts = context.Carts.ToList();
            var cart_productss = context.Cart_Productss.Where(c => c.CartId == currentcartid).ToList();
            foreach (var cartProduct in cart_productss)
            {
                // Get the product
                Product product = context.Products.Find(cartProduct.ProductId);

                // If the product is inactive, remove it from the cart
                if (!product.IsActive)
                {
                    context.Cart_Productss.Remove(cartProduct);
                    context.SaveChanges();
                }
            }
            cart_productss = context.Cart_Productss.Where(c => c.CartId == currentcartid).ToList();
            var productss = context.Products.ToList();
            
            var viewModel = new ProdCartCartpViewModel(productss, carts, cart_productss, currentcartid, cart3.Value);
            ViewData["ProductCategoryViewModel"] = viewModel;
            return View(viewModel);
        }
        public IActionResult AddToCart(int productId, int quantity, string buyerid)
        {
            List<Category> cats = context.Categories.ToList();
            List<Product> prods = context.Products.Where(p => p.IsActive == true).ToList();

            Product product = context.Products.Find(productId);
            var model = new ProductCategoryViewModel(prods, cats);
            if (quantity > product.Quantity)
            {
                ModelState.AddModelError("", "*Quantity not in stock");
                ViewData["QuantityError"] = "*Quantity not in stock";
                ViewData["ProductId"] = productId;

                return View("BuyerIndex", model);
            }

            if (!ModelState.IsValid)
            {
                return View("BuyerIndex", model);
            }

            Cart cart = context.Carts.FirstOrDefault(c => c.BuyerId == buyerid);
            int cartid = cart.CartId;

            // Check if the product is already in the cart
            Cart_products existingCartProduct = context.Cart_Productss.FirstOrDefault(c => c.ProductId == productId && c.CartId == cartid);
            if (existingCartProduct != null)
            {
                // Increase the quantity of the existing product
                existingCartProduct.Quantity += quantity;
            }
            else
            {
                // Add the product to the cart
                Cart_products cart_Products = new Cart_products();
                cart_Products.ProductId = productId;
                cart_Products.CartId = cartid;
                cart_Products.InsertionDate = DateTime.Now;
                cart_Products.Quantity = quantity;

                // Set the category ID for the cart product
                Product cartProduct = context.Products.Find(productId);
                cart_Products.CategoryId = cartProduct.CategoryId;

                context.Cart_Productss.Add(cart_Products);
            }

            // Get the cart value
            decimal cartValue = cart.Value;
            cartValue += product.Price * quantity;

            // Calculate the total purchase amount from the same category
            List<Cart_products> cartProductsInSameCategory = context.Cart_Productss
      .Where(cp => cp.CategoryId == product.CategoryId && cp.CartId == cartid)
      .ToList();
            decimal categoryTotalPurchaseAmount = 0;
            if (cartProductsInSameCategory.Count == 0)
            {
                categoryTotalPurchaseAmount = product.Price*quantity;

            }
            
            foreach (var cartProduct in cartProductsInSameCategory)
            {
                Product productInCart = context.Products.Find(cartProduct.ProductId);
                categoryTotalPurchaseAmount += productInCart.Price * cartProduct.Quantity;
            }

            // Check if the buyer is eligible for a discount
            if (categoryTotalPurchaseAmount >= 10000)
            {
                // Get the discount percentage for the category
                CategoryDiscount categoryDiscount = context.CategoryDiscounts.FirstOrDefault(cd => cd.CategoryId == product.CategoryId);
                if (categoryDiscount != null)
                {
                    if (existingCartProduct != null)
                    {
                        decimal discountPercentage = categoryDiscount.DiscountPercentage;
                        decimal categoryDiscountAmount = (product.Price * existingCartProduct.Quantity * discountPercentage) / 100;
                        cartValue -= categoryDiscountAmount;
                    }
                    else
                    {
                        decimal discountPercentage = categoryDiscount.DiscountPercentage;
                        decimal categoryDiscountAmount = (product.Price * quantity * discountPercentage) / 100;
                        cartValue -= categoryDiscountAmount;
                    }
                  
                }
            }

            // Update the cart value
            cart.Value = cartValue;

            // Update the cart
            context.Carts.Update(cart);
            context.SaveChanges();
            TempData["CurrentBuyerId2"] = BuyerId;
            return RedirectToAction("Index", new { buyerid = buyerid });
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int cartid, int quantity, string buyerid)
        {
            Cart_products cartProduct = context.Cart_Productss.FirstOrDefault(cp => cp.ProductId == productId && cp.CartId == cartid);
            if (cartProduct != null)
            {
                Product product = context.Products.Find(cartProduct.ProductId);
                // Perform quantity validation and update the quantity
                if (quantity >= 1 && quantity <= product.Quantity)
                {
                    var oldQuantity = cartProduct.Quantity;
                    
                    cartProduct.Quantity = quantity;
                    context.Cart_Productss.Update(cartProduct);
                    context.SaveChanges();

                    Cart cart = context.Carts.Find(cartid);
                    if (oldQuantity * product.Price > 10000) {
                        CategoryDiscount categoryDiscount = context.CategoryDiscounts.FirstOrDefault(cd => cd.CategoryId == product.CategoryId);
                        if (categoryDiscount != null)
                        {
                            decimal discountPercentage = categoryDiscount.DiscountPercentage;
                            decimal categoryDiscountAmount = (product.Price * oldQuantity * discountPercentage) / 100;
                            cart.Value = cart.Value - oldQuantity * product.Price- categoryDiscountAmount;
                        }
                    }
                    else
                    {
                        cart.Value = cart.Value - oldQuantity * product.Price; // Deduct the previous quantity * price

                    }
                    cart.Value = cart.Value + product.Price * quantity; // Add the new quantity * price
                    var cartValue = cart.Value;
                    // Apply category discount
                    decimal categoryTotalPurchaseAmount = context.Cart_Productss
                        .Where(c => c.CartId == cartid && c.CategoryId == product.CategoryId)
                        .Join(context.Products,
                            cp => cp.ProductId,
                            prod => prod.ProductId,
                            (cp, prod) => cp.Quantity * prod.Price)
                        .Sum();

                    if (categoryTotalPurchaseAmount >= 10000)
                    {
                        // Get the discount percentage for the category
                        CategoryDiscount categoryDiscount = context.CategoryDiscounts.FirstOrDefault(cd => cd.CategoryId == product.CategoryId);
                        if (categoryDiscount != null)
                        {
                            decimal discountPercentage = categoryDiscount.DiscountPercentage;
                            decimal categoryDiscountAmount = (product.Price * quantity * discountPercentage) / 100;
                            cartValue -= categoryDiscountAmount;
                        }
                    }
                    cart.Value = cartValue;
                    context.Update(cart);
                    context.SaveChanges();
                }
            }

            return RedirectToAction("ViewCart", new { buyerid = buyerid });
        }
        public IActionResult DeleteProduct(int id, int cartid)
        {
            Cart cart = context.Carts.FirstOrDefault(c => c.CartId == cartid);
            Cart_products cartproducts = context.Cart_Productss.FirstOrDefault(c => c.CartId == cartid && c.ProductId == id);
            Product prod = context.Products.Find(cartproducts.ProductId);
            context.Cart_Productss.Remove(cartproducts);
            int quant = cartproducts.Quantity;
            if (prod.Price*quant > 10000)
            {
                CategoryDiscount categoryDiscount = context.CategoryDiscounts.FirstOrDefault(cd => cd.CategoryId == prod.CategoryId);
                if (categoryDiscount != null)
                {
                    decimal discountPercentage = categoryDiscount.DiscountPercentage;
                    decimal categoryDiscountAmount = (prod.Price * quant * discountPercentage) / 100;
                    cart.Value = cart.Value - prod.Price * quant+ categoryDiscountAmount;
                }
            }
            else
            {
                cart.Value = cart.Value - prod.Price * quant;

            }
            
            context.Update(cart);
            context.SaveChanges();
            var carts = context.Carts.ToList();
            var cart_productss = context.Cart_Productss.Where(c => c.CartId == cartid).ToList();
            var productss = context.Products.ToList();
            int cartidd = 0;
            var viewModel = new ProdCartCartpViewModel(productss, carts, cart_productss, cartidd, cart.Value);
            ViewData["ProductCategoryViewModel"] = viewModel;
            return View("ViewCart", viewModel);
            //return View("ViewCart");

        }
        [HttpPost]
        [HttpGet]
        public IActionResult SearchProducts(string? searchQuery)
        {
            List<Product> activeproducts = context.Products
     .Where(c => c.IsActive && c.Quantity > 0)
     .ToList();
            List<string> activeSellerIds = activeproducts.Select(p => p.SellerId).Distinct().ToList();

            List<ApplicationUser> activeSellers = context.Users
                .Where(s => activeSellerIds.Contains(s.Id) && s.EmailConfirmed)
                .ToList();

            activeproducts = activeproducts
                .Where(p => activeSellerIds.Contains(p.SellerId))
                .ToList();

            // Check expiration date and update product status
            DateTime currentDate = DateTime.UtcNow;

            foreach (var product in activeproducts)
            {
                if (product.ExpirationDate != null && product.ExpirationDate < currentDate)
                {
                    product.IsActive = false;
                    activeproducts.Remove(product);
                   
                }
            }

            // Fetch filtered products based on the search query using Entity Framework Core
            var filteredProducts = activeproducts
                .Where(p => searchQuery == null ||
                       p.Name.ToLower().Contains(searchQuery.ToLower()) ||
                       p.Description.ToLower().Contains(searchQuery.ToLower()))
                .ToList();

            var categories = context.Categories.ToList();
            var viewModel = new ProductCategoryViewModel(filteredProducts, categories);
            ViewData["ProductCategoryViewModel2"] = viewModel;

            // Return the partial view containing the filtered products
            return PartialView("_ProductListModalPartial", viewModel);
        }

        public IActionResult GetAllProducts()
        {
            List<Product> activeproducts = context.Products.Where(c => c.IsActive == true && c.Quantity > 0).ToList();
            List<string> activeSellerIds = activeproducts.Select(p => p.SellerId).Distinct().ToList();
            List<ApplicationUser> activeSellers = context.Users
                .Where(s => activeSellerIds.Contains(s.Id) && s.EmailConfirmed)
                .ToList();
            activeproducts = activeproducts
      .Where(p => activeSellerIds.Contains(p.SellerId))
      .ToList();

            List<Category> categories = context.Categories.ToList();

            // Create the view model with filtered products and all categories
            var viewModel = new ProductCategoryViewModel(activeproducts,categories);
            ViewData["ProductCategoryViewModel"] = viewModel;

            // Return the partial view with the updated view model


            return PartialView("_ProductListModalPartial", viewModel);

        }
        [HttpGet]
        public IActionResult FilterProductsByCategories(string categoryIds)
        {

            List<Product> activeproducts = context.Products
     .Where(c => c.IsActive && c.Quantity > 0)
     .ToList();
            List<string> activeSellerIds = activeproducts.Select(p => p.SellerId).Distinct().ToList();

            List<ApplicationUser> activeSellers = context.Users
                .Where(s => activeSellerIds.Contains(s.Id) && s.EmailConfirmed)
                .ToList();

            activeproducts = activeproducts
                .Where(p => activeSellerIds.Contains(p.SellerId))
                .ToList();

            // Check expiration date and update product status
            DateTime currentDate = DateTime.UtcNow;

            foreach (var product in activeproducts)
            {
                if (product.ExpirationDate != null && product.ExpirationDate < currentDate)
                {
                    product.IsActive = false;
                    activeproducts.Remove(product);
                    // You can perform additional actions here if needed
                }
            }
            if (!string.IsNullOrEmpty(categoryIds))
            {
               
                // Split the categoryIds string into an array of strings
                string[] categoryIdArray = categoryIds.Split(',');

                // Convert the array of strings to an array of integers
                int[] categoryIdIntArray = Array.ConvertAll(categoryIdArray, int.Parse);

                // Filter products based on categoryIds
                activeproducts = activeproducts
                   .Where(p => categoryIdIntArray.Contains(p.CategoryId))
                   .ToList();
            }
            // Retrieve all categories if context is not null
            var categories = context.Categories.ToList();

            // Create the view model with filtered products and all categories
            var viewModel = new ProductCategoryViewModel(activeproducts, categories);
            ViewData["ProductCategoryViewModel3"] = viewModel;

            // Return the partial view with the updated view model
           

            return PartialView("_ProductListModalPartial", viewModel);
        }
        public IActionResult Checkout(int cartid)
        {
            Cart cart = context.Carts.Find(cartid);
            string buyerid = cart.BuyerId;
            Order order = new Order();
            
            order.BuyerId = buyerid;
            order.Value = cart.Value;
            cart.Value = 0;
            context.Carts.Update(cart);
            context.SaveChanges();
            DateTime datetime = DateTime.Now;
            order.Date = datetime;
            context.Orders.Add(order);
            context.SaveChanges();
            Order order2 = context.Orders.FirstOrDefault(c => c.Date == datetime);
            int orderid = order2.OrderId;
            

            // Retrieve the cart products associated with the given cartid
            List<Cart_products> cartProducts = context.Cart_Productss
                .Where(cp => cp.CartId == cartid)
                .ToList();

            foreach (var cartProduct in cartProducts)
            {
                Product prod = context.Products.Find(cartProduct.ProductId);
                OldProduct oldprod = new OldProduct();
                oldprod.RequiresExpirationDate = prod.RequiresExpirationDate;
                oldprod.Name = prod.Name;
                oldprod.Image = prod.Image;
                oldprod.IsActive = prod.IsActive;
                oldprod.Quantity = prod.Quantity;
                oldprod.Price = prod.Price;
                oldprod.Description = prod.Description;
                oldprod.CategoryId = prod.CategoryId;
                oldprod.ExpirationDate = prod.ExpirationDate;
                context.OldProducts.Add(oldprod);
                context.SaveChanges();
                Order_Items orderitems = new Order_Items();
                orderitems.OrderId = orderid;
                orderitems.OldProductId = oldprod.OldProductId;
                // Decrease the quantity of the product
                Product product = context.Products.FirstOrDefault(p => p.ProductId == cartProduct.ProductId);
                product.Quantity -= cartProduct.Quantity;
                context.Products.Update(product);
                context.SaveChanges();

                orderitems.ProductId = cartProduct.ProductId;
                orderitems.Quantity = cartProduct.Quantity;
                context.Order_Itemss.Add(orderitems);
            }

          
            context.SaveChanges();

            // Remove all cart products associated with the given cartid
            context.Cart_Productss.RemoveRange(cartProducts);
            context.SaveChanges();

            return RedirectToAction("DisplayAllBuyerOrders", new { buyerId = buyerid });
        }

        public IActionResult DisplayAllBuyerOrders(string buyerId)
        {
            var viewModel = new List<OrdOrdItemsViewModel>();

            // Retrieve all orders for the buyer
            var orders = context.Orders.Where(o => o.BuyerId == buyerId).ToList();

            foreach (var order in orders)
            {
                // Retrieve order items for the current order
                var orderItems = context.Order_Itemss
                    .Where(oi => oi.OrderId == order.OrderId)
                    .ToList();
                List<Product> products = context.Products.Where(p => p.IsActive == true).ToList();
                List<OldProduct> oldproducts = context.OldProducts.Where(p => p.IsActive == true).ToList();
                // Create an instance of OrdOrdItemsViewModel for the current order and its order items
                var orderViewModel = new OrdOrdItemsViewModel(new List<Order> { order }, orderItems, products, oldproducts);

                // Add the view model to the list
                viewModel.Add(orderViewModel);
            }

            return View(viewModel);
        } 
    }
    }
