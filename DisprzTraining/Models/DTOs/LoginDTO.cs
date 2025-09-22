using System.ComponentModel.DataAnnotations;

namespace DisprzTraining.Models.DTOs
{
    /// <summary>
    /// Data transfer object for user login
    /// </summary>
    public class LoginDTO
    {
        /// <summary>
        /// Username for login
        /// </summary>
        /// <example>johndoe</example>
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        
        /// <summary>
        /// Password for login
        /// </summary>
        /// <example>password123</example>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}