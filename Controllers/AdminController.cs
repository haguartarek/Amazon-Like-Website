using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication2.ViewModels;
using WebApplication2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System;
using static WebApplication2.Controllers.AcccountController;

namespace WebApplication2.Controllers
{

   // [AllowAnonymous]
        [Authorize(Roles = "admin")]
    public class AdminController : Controller
          
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
 
      

        public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        Entity context = new Entity();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetActivationRequests()
        {
            var requests = context.ActivationRequests.ToList();
            return View(requests);
        }
        public IActionResult HasActivationRequests()
        {
            var hasActivationRequests = context.ActivationRequests.Any();
            return Json(hasActivationRequests);
        }
      
        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }
        [HttpGet]
        public IActionResult RegisterSeller()
        {

            return View("RegisterSeller");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> RegisterSeller(RegisterUserViewModel newseller)
        {

            if (ModelState.IsValid)
            {
                ApplicationUser userModel = new ApplicationUser();
                userModel.UserName = newseller.Username;
                userModel.PasswordHash = newseller.Password;
                userModel.Age = newseller.Age;
                userModel.Address = newseller.Address;
                userModel.PhoneNumber = newseller.Phone;
                userModel.Email = newseller.Email;
                userModel.EmailConfirmed = true;
                //save db
                IdentityResult result = await userManager.CreateAsync(userModel, newseller.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(userModel, "Seller");
                    
                    return RedirectToAction("ViewAllSellers");
                }
                else
                {
                    foreach (var errorItem in result.Errors)
                    {
                        ModelState.AddModelError("Password", errorItem.Description);
                    }
                }
            }
                    return View("RegisterSeller");
        }
        public async Task <IActionResult> ViewAllSellers()
        {
            var sellers = await userManager.GetUsersInRoleAsync("Seller");
            return View(sellers);
        }
        public async Task<IActionResult> ViewAllBuyers()
        {
            var sellers = await userManager.GetUsersInRoleAsync("Buyer");
            return View(sellers);
        }
        public async Task<IActionResult> ActdisSeller(string sellerId)
        {
            // Get the seller from the database
            var products = context.Products.Where(p => p.SellerId == sellerId).ToList();
            var seller = await userManager.FindByIdAsync(sellerId);
            // Check if the seller is active
            if (seller.EmailConfirmed)
            {
                // Deactivate the seller
                seller.EmailConfirmed = false;
                await userManager.UpdateAsync(seller);
                foreach (var product in products)
                {
                    product.IsActive = false;
                    context.SaveChanges();
                }
            }
            else
            {
                // Activate the seller
                seller.EmailConfirmed = true;
                await userManager.UpdateAsync(seller);
                foreach (var product in products)
                {
                    product.IsActive = true;
                    context.SaveChanges();
                }
            }
            var requests = context.ActivationRequests.Where(c => c.SellerId == sellerId).ToList();
            if (requests != null)
            {
                foreach(var request in requests)
                {
                    context.ActivationRequests.Remove(request);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("ViewAllSellers");
        }

        [HttpGet]
        public async Task <IActionResult> ResetSellerPassword(string sellerid)
        {
            ResetPassword sellervm=new ResetPassword();

           // IdentityResult result = await userManager.CreateAsync(userModel, newUserVM.Password);
            return PartialView("_ResetPasswordModalPartial", sellervm);

        }
        [HttpGet]
        public async Task<IActionResult> ResetBuyerPassword(string buyerid)
        {
            ResetPassword buyervm = new ResetPassword();
            var seller = await userManager.FindByIdAsync(buyerid);
            buyervm.Password = seller.PasswordHash;
            return PartialView("_ResetPasswordModalPartial", buyervm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveResetSellerPassword(ResetPassword sellervm , string sellerid)
        {
      
           
                string hashed = HashPassword(sellervm.Password);
                var seller = await userManager.FindByIdAsync(sellerid);
                seller.PasswordHash = hashed;
                await userManager.UpdateAsync(seller);
                return View("ViewAllSellers");

            
           

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveResetBuyerPassword(ResetPassword buyervm, string buyerid)
        {
            
                string hashed = HashPassword(buyervm.Password);
                var buyer = await userManager.FindByIdAsync(buyerid);
                buyer.PasswordHash = hashed;
                await userManager.UpdateAsync(buyer);
                return View("ViewAllSellers");
            

           // return PartialView("_ResetPasswordModalPartial");

        }
        [HttpGet]
        public IActionResult AddCategory()
        {
            return PartialView("_AddCategoryModalPartial");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAddCategory(Category category)
        {
           //add the id of category id to be random

            //   DateTime insertionDate = DateTime.Now;
            // DateTime modifiedDate = DateTime.Now;
           // context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories ON");

            if (category != null)
                {
                //    category.InsertionDate = insertionDate;
                //  category.ModifiedDate = modifiedDate;
                
                context.Categories.Add(category);
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories ON");

                context.SaveChanges();
                context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Categories OFF");
                //refresh
                return View("ViewAllCategories");
                //  return PartialView("_AddStTeacherModalPartial");
            }


            return PartialView("_AddCategoryModalPartial");
        }
        public IActionResult ViewAllCategories()
        {
            List<Category> cats = context.Categories.ToList();

            return View(cats);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {

            Category catmodel = context.Categories.FirstOrDefault(e => e.CategoryId == id);
            return PartialView("_EditCatModalPartial", catmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveEdit(Category newCategory, int id)
        {

            Category oldcat = context.Categories.Find(id);


            if (ModelState.IsValid)
            {
                try
                {
                    if (oldcat != null)
                    {
                        oldcat.Name = newCategory.Name;
                        oldcat.InsertionDate = newCategory.InsertionDate;
                        oldcat.ModifiedDate = newCategory.ModifiedDate;
                    
                        context.SaveChanges();
                        return RedirectToAction("ViewAllCategories");

                    }

                }
                catch (Exception ex)
                {
                    string innerErrorMessage = ex.InnerException?.Message;
                    ModelState.AddModelError(string.Empty, ex.Message.ToString());
                }
            }

            // return RedirectToAction("Index");
            return PartialView("_EditCAtModalPartial", newCategory);
        }
        public IActionResult DeleteCategory(int id)
        {

            Category catmodel = context.Categories.Find(id);
            var products = context.Products.Where(p => p.CategoryId == id);

            foreach (var product in products)
            {
                context.Products.Remove(product);
            }
            context.Categories.Remove(catmodel);
            context.SaveChanges();

            return RedirectToAction("ViewAllCategories");
        }
    }
}
