using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.ViewModels;

namespace ProjecWebApplication2tDraft.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public  IActionResult New()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> New(RoleViewModel rolevm)
        {
            if (ModelState.IsValid == true)
            {
                IdentityRole roleModel = new IdentityRole();
                roleModel.Name = rolevm.RoleName;
                IdentityResult result = await roleManager.CreateAsync(roleModel);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var items in result.Errors)
                    {
                        ModelState.AddModelError("", items.Description);
                    }
                }
            }
            return View(rolevm);
        }
    }
}
