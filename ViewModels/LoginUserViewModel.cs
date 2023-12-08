using System.ComponentModel.DataAnnotations;

namespace WebApplication2.ViewModels
{
    public class LoginUserViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool rememberme { get; set; }
    }
}
