using System;
using System.ComponentModel.DataAnnotations;

namespace DisprzTraining.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public DateTime? StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        public DateTime? EndTime { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}