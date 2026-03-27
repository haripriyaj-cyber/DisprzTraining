using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DisprzTraining.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }  // In a real app, this should be hashed
        
        public string Email { get; set; }
        
        public string FullName { get; set; }
        
        public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property for user's appointments
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}