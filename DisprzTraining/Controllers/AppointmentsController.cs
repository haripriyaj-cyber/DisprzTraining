using Microsoft.AspNetCore.Mvc;
using DisprzTraining.DataAccess; // To reference AppDbContext
using DisprzTraining.Models;     // To reference Appointment model
using System.Linq;               // For Any() and ToList()

namespace DisprzTraining.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/appointments
        [HttpGet]
        public IActionResult GetAppointments()
        {
            var appointments = _context.Appointments.ToList();
            return Ok(appointments);
        }

        // GET: api/appointments/{id}
        [HttpGet("{id}")]
        public IActionResult GetAppointmentById(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }
            return Ok(appointment);
        }

        // POST: api/appointments
        [HttpPost]
        public IActionResult CreateAppointment(Appointment appointment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool hasConflict = _context.Appointments.Any(a =>
                a.Date == appointment.Date &&
                (
                    (appointment.StartTime >= a.StartTime && appointment.StartTime < a.EndTime) ||
                    (appointment.EndTime > a.StartTime && appointment.EndTime <= a.EndTime) ||
                    (appointment.StartTime <= a.StartTime && appointment.EndTime >= a.EndTime)
                )
            );

            if (hasConflict)
            {
                return Conflict(new { message = "Appointment time conflicts with an existing appointment." });
            }

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAppointments), new { id = appointment.Id }, appointment);
        }

        // PUT: api/appointments/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateAppointment(int id, Appointment updatedAppointment)
        {
            if (id != updatedAppointment.Id)
            {
                return BadRequest(new { message = "Appointment ID mismatch." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAppointment = _context.Appointments.Find(id);
            if (existingAppointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }

            bool hasConflict = _context.Appointments.Any(a =>
                a.Id != id &&
                a.Date == updatedAppointment.Date &&
                (
                    (updatedAppointment.StartTime >= a.StartTime && updatedAppointment.StartTime < a.EndTime) ||
                    (updatedAppointment.EndTime > a.StartTime && updatedAppointment.EndTime <= a.EndTime) ||
                    (updatedAppointment.StartTime <= a.StartTime && updatedAppointment.EndTime >= a.EndTime)
                )
            );

            if (hasConflict)
            {
                return Conflict(new { message = "Appointment time conflicts with an existing appointment." });
            }

            // Update fields
            existingAppointment.Title = updatedAppointment.Title;
            existingAppointment.Description = updatedAppointment.Description;
            existingAppointment.Date = updatedAppointment.Date;
            existingAppointment.StartTime = updatedAppointment.StartTime;
            existingAppointment.EndTime = updatedAppointment.EndTime;

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/appointments/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteAppointment(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment == null)
            {
                return NotFound(new { message = "Appointment not found." });
            }

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
