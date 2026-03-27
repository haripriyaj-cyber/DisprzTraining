using System;

namespace DisprzTraining.Models.DTOs
{
    /// <summary>
    /// Data transfer object for user information
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Username of the user
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Email address of the user
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Full name of the user
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// When the user was created
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }
    }
}