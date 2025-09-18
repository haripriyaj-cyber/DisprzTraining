using System;
using System.ComponentModel.DataAnnotations;

namespace DisprzTraining.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public DateTimeOffset StartTime { get; set; }
        
        [Required]
        public DateTimeOffset EndTime { get; set; }
        
        public string Description { get; set; }
        
        public bool IsAllDay { get; set; }
        
        public string Location { get; set; }
        
        public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
