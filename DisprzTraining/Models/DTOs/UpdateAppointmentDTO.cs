using System;
using System.ComponentModel.DataAnnotations;

namespace DisprzTraining.Models.DTOs
{
    /// <summary>
    /// Data transfer object for updating an existing appointment
    /// </summary>
    public class UpdateAppointmentDTO
    {
        /// <summary>
        /// Title of the appointment
        /// </summary>
        /// <example>Updated Team Meeting</example>
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }
        
        /// <summary>
        /// Start time of the appointment in ISO 8601 format
        /// </summary>
        /// <example>2023-06-01T09:30:00Z</example>
        [Required(ErrorMessage = "Start time is required")]
        public DateTimeOffset StartTime { get; set; }
        
        /// <summary>
        /// End time of the appointment in ISO 8601 format
        /// </summary>
        /// <example>2023-06-01T10:30:00Z</example>
        [Required(ErrorMessage = "End time is required")]
        public DateTimeOffset EndTime { get; set; }
        
        /// <summary>
        /// Description of the appointment
        /// </summary>
        /// <example>Weekly team sync-up with project updates</example>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
        
        /// <summary>
        /// Indicates if the appointment is an all-day event
        /// </summary>
        /// <example>false</example>
        public bool IsAllDay { get; set; }
        
        /// <summary>
        /// Location of the appointment
        /// </summary>
        /// <example>Conference Room B</example>
        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
        public string Location { get; set; }
    }
}
