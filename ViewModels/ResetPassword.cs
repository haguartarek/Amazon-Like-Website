using System.ComponentModel.DataAnnotations;

namespace WebApplication2.ViewModels
{
    public class ResetPassword
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
}
