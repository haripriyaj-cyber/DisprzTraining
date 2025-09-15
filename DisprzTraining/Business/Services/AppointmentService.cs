using System;
using System.Collections.Generic;
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
    }
}
