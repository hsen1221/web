using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [Display(Name = "Full Name")] // <--- Shows nicely on the label
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}