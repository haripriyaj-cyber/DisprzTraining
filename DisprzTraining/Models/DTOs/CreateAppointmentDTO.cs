using System;
using System.ComponentModel.DataAnnotations;

namespace DisprzTraining.Models.DTOs
{
    public class CreateAppointmentDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }
        
        [Required(ErrorMessage = "End time is required")]
        public DateTime EndTime { get; set; }
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
        
        public bool IsAllDay { get; set; }
        
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
        public string Location { get; set; }
    }
}
