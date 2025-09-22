using System.ComponentModel.DataAnnotations;

namespace DisprzTraining.Models.DTOs
{
    /// <summary>
    /// Data transfer object for user registration
    /// </summary>
    public class RegisterDTO
    {
        /// <summary>
        /// Username for registration
        /// </summary>
        /// <example>johndoe</example>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string Username { get; set; }
        
        /// <summary>
        /// Password for registration
        /// </summary>
        /// <example>password123</example>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; }
        
        /// <summary>
        /// Email address
        /// </summary>
        /// <example>john.doe@example.com</example>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        
        /// <summary>
        /// Full name of the user
        /// </summary>
        /// <example>John Doe</example>
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; }
    }
}