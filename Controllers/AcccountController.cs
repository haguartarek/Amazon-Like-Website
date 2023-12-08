using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    public class AcccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public AcccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        Entity context = new Entity();
        public enum Roles
        {
            Admin,
            Seller,
            Buyer
        }

        public IActionResult Index()
        {
            return View("Index", "Home");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Register(RegisterUserViewModel newUserVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser userModel = new ApplicationUser();
                userModel.UserName = newUserVM.Username;
                userModel.PasswordHash = newUserVM.Password;
                userModel.Age = newUserVM.Age;
                userModel.Address = newUserVM.Address;
                userModel.PhoneNumber = newUserVM.Phone;
                userModel.Email = newUserVM.Email;
                //save db
                IdentityResult result = await userManager.CreateAsync(userModel, newUserVM.Password);
                if (result.Succeeded)
                {
                    if (userModel.UserName == "Admin")
                    {
                        await userManager.AddToRoleAsync(userModel, "Admin");
                        await signInManager.SignInAsync(userModel, false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        await userManager.AddToRoleAsync(userModel, "Buyer");
                        await signInManager.SignInAsync(userModel, false);
                        return RedirectToAction("Index", "Home");
                    }
                   
                }
                else{
                foreach(var errorItem in result.Errors)
                    {
                        ModelState.AddModelError("Password", errorItem.Description);
                    }
                }
              
            }
            return View(newUserVM);
        }
        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserViewModel userVM)
        {
            if (ModelState.IsValid)
            {
                //check db
            ApplicationUser userModel= await userManager.FindByNameAsync(userVM.Username);
               
                if (userModel != null)
                {
                    bool found =await userManager.CheckPasswordAsync(userModel, userVM.Password);
                    if (found == true)
                    {

                            await signInManager.SignInAsync(userModel, userVM.rememberme);
                        if (await userManager.IsInRoleAsync(userModel, Roles.Seller.ToString()))
                        {
                            var seller = await userManager.FindByNameAsync(userVM.Username);
                            if (!seller.EmailConfirmed)
                            {
                                await CreateActivationRequest(seller.Id);
                                ModelState.AddModelError("", "This seller account is inactive. Please contact the admin. An Activation request is already sent.");
                                signInManager.SignOutAsync();
                                return View(userVM);
                            }
                            var sellerId = userModel.Id;


                            ViewBag.SellerId = sellerId;
                            TempData["CurrentSellerId"] = sellerId;
                            TempData["CurrentSellerIdIndex"] = sellerId;

                            return RedirectToAction("Index", "Seller", new { sellerId = sellerId }); // Redirect to admin page
                        }
                        else if (await userManager.IsInRoleAsync(userModel, Roles.Admin.ToString()))
                        {
                            return RedirectToAction("Index", "Admin"); // Redirect to admin page
                        }
                       
                        else
                        {
                            
                            var buyerId = userModel.Id;
                            ViewData["BuyerId"] = buyerId;
                            TempData["CurrentBuyerId"] = buyerId;
                            TempData["CurrentBuyerId2"] = buyerId;
                            return RedirectToAction("Index", "Buyer", new { buyerid = buyerId }); // Redirect to default page for non-admin users
                        }
                    }
                }
                ModelState.AddModelError("", "Invalid username or password!");
            }
            return View(userVM);
        }
        private async Task CreateActivationRequest(string sellerId)
        {
            var seller = await userManager.FindByIdAsync(sellerId);
            var activationRequest = new ActivationRequest
            {
                SellerId = sellerId,
                RequestDate = DateTime.UtcNow,
                IsActive = false,
                SellerName = seller.UserName
            };

            // Save the activation request to the database
            // You can use your database context to save the request
            context.ActivationRequests.Add(activationRequest);
            await context.SaveChangesAsync();
        }
    }
}
