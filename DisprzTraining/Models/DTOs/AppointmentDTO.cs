using System;

namespace DisprzTraining.Models.DTOs
{
    /// <summary>
    /// Data transfer object for appointment information
    /// </summary>
    public class AppointmentDTO
    {
        /// <summary>
        /// Unique identifier for the appointment
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }
        
        /// <summary>
        /// Title of the appointment
        /// </summary>
        /// <example>Team Meeting</example>
        public string Title { get; set; }
        
        /// <summary>
        /// Start time of the appointment in ISO 8601 format
        /// </summary>
        /// <example>2023-06-01T09:00:00Z</example>
        public DateTimeOffset StartTime { get; set; }
        
        /// <summary>
        /// End time of the appointment in ISO 8601 format
        /// </summary>
        /// <example>2023-06-01T10:00:00Z</example>
        public DateTimeOffset EndTime { get; set; }
        
        /// <summary>
        /// Description of the appointment
        /// </summary>
        /// <example>Weekly team sync-up meeting to discuss project progress</example>
        public string Description { get; set; }
        
        /// <summary>
        /// Indicates if the appointment is an all-day event
        /// </summary>
        /// <example>false</example>
        public bool IsAllDay { get; set; }
        
        /// <summary>
        /// Location of the appointment
        /// </summary>
        /// <example>Conference Room A</example>
        public string Location { get; set; }
        
        /// <summary>
        /// When the appointment was created
        /// </summary>
        /// <example>2023-05-25T14:30:00Z</example>
        public DateTimeOffset CreatedAt { get; set; }
        
        /// <summary>
        /// When the appointment was last updated
        /// </summary>
        /// <example>2023-05-26T09:15:00Z</example>
        public DateTimeOffset? UpdatedAt { get; set; }
        
        /// <summary>
        /// User identifier for the appointment
        /// </summary>
        /// <example>1</example>
        public int UserId { get; set; }
    }
}
