using System.ComponentModel.DataAnnotations;

namespace WebApplication2.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string  Phone { get; set; }
        [Required]
        public int  Age { get; set; }
    }

}
