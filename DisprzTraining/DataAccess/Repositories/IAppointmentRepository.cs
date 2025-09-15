using System.Collections.Generic;
using System.Threading.Tasks;
using DisprzTraining.Models;

namespace DisprzTraining.DataAccess.Repositories
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment> GetByIdAsync(int id);
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<Appointment> UpdateAsync(Appointment appointment);
        Task<bool> DeleteAsync(int id);
    }
}
