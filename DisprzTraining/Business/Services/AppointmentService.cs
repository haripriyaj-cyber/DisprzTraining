using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisprzTraining.DataAccess.Repositories;
using DisprzTraining.Models;
using DisprzTraining.Models.DTOs;

namespace DisprzTraining.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _appointmentRepository.GetAllAsync();
        }

        public async Task<Appointment> GetAppointmentByIdAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {id} not found");
            
            return appointment;
        }

        public async Task<Appointment> CreateAppointmentAsync(CreateAppointmentDTO appointmentDto)
        {
            // Validate business rules
            if (appointmentDto.EndTime <= appointmentDto.StartTime)
                throw new ArgumentException("End time must be after start time");

            // Check for overlapping appointments
            await CheckForOverlappingAppointments(appointmentDto.StartTime, appointmentDto.EndTime);

            var appointment = new Appointment
            {
                Title = appointmentDto.Title,
                StartTime = appointmentDto.StartTime,
                EndTime = appointmentDto.EndTime,
                Description = appointmentDto.Description,
                IsAllDay = appointmentDto.IsAllDay,
                Location = appointmentDto.Location,
                CreatedAt = DateTime.UtcNow
            };

            return await _appointmentRepository.CreateAsync(appointment);
        }

        public async Task<Appointment> UpdateAppointmentAsync(int id, UpdateAppointmentDTO appointmentDto)
        {
            // Validate business rules
            if (appointmentDto.EndTime <= appointmentDto.StartTime)
                throw new ArgumentException("End time must be after start time");

            var existingAppointment = await _appointmentRepository.GetByIdAsync(id);
            if (existingAppointment == null)
                throw new KeyNotFoundException($"Appointment with ID {id} not found");

            // Check for overlapping appointments (excluding the current appointment being updated)
            await CheckForOverlappingAppointments(appointmentDto.StartTime, appointmentDto.EndTime, id);

            // Update properties
            existingAppointment.Title = appointmentDto.Title;
            existingAppointment.StartTime = appointmentDto.StartTime;
            existingAppointment.EndTime = appointmentDto.EndTime;
            existingAppointment.Description = appointmentDto.Description;
            existingAppointment.IsAllDay = appointmentDto.IsAllDay;
            existingAppointment.Location = appointmentDto.Location;
            existingAppointment.UpdatedAt = DateTime.UtcNow;

            return await _appointmentRepository.UpdateAsync(existingAppointment);
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {id} not found");

            return await _appointmentRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Checks if there are any overlapping appointments for the given time range
        /// </summary>
        /// <param name="startTime">Start time of the appointment</param>
        /// <param name="endTime">End time of the appointment</param>
        /// <param name="excludeAppointmentId">Optional ID of appointment to exclude from the check (for updates)</param>
        /// <returns>Task</returns>
        /// <exception cref="InvalidOperationException">Thrown when there's a conflict</exception>
        private async Task CheckForOverlappingAppointments(DateTimeOffset startTime, DateTimeOffset endTime, int? excludeAppointmentId = null)
        {
            var allAppointments = await _appointmentRepository.GetAllAsync();
            
            // Filter out the appointment being updated if an ID is provided
            var potentialConflicts = allAppointments
                .Where(a => excludeAppointmentId == null || a.Id != excludeAppointmentId)
                .ToList();

            // Check for overlaps - fixed to handle adjacent appointments correctly
            var overlappingAppointment = potentialConflicts.FirstOrDefault(a => 
                // New appointment starts during an existing appointment (strictly before end)
                (startTime >= a.StartTime && startTime < a.EndTime) ||
                // New appointment ends during an existing appointment (strictly after start)
                (endTime > a.StartTime && endTime <= a.EndTime) ||
                // New appointment completely contains an existing appointment
                (startTime < a.StartTime && endTime > a.EndTime));

            if (overlappingAppointment != null)
            {
                throw new InvalidOperationException(
                    $"Appointment conflicts with existing appointment '{overlappingAppointment.Title}' " +
                    $"scheduled from {overlappingAppointment.StartTime:g} to {overlappingAppointment.EndTime:g}");
            }
        }
    }
}
