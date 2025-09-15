using System.Collections.Generic;
using System.Threading.Tasks;
using DisprzTraining.Models;
using DisprzTraining.Models.DTOs;

namespace DisprzTraining.Business.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<Appointment> GetAppointmentByIdAsync(int id);
        Task<Appointment> CreateAppointmentAsync(CreateAppointmentDTO appointmentDto);
        Task<Appointment> UpdateAppointmentAsync(int id, UpdateAppointmentDTO appointmentDto);
        Task<bool> DeleteAppointmentAsync(int id);
    }
}
