using System.Threading.Tasks;
using DisprzTraining.Models;

namespace DisprzTraining.DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User> CreateAsync(User user);
        Task<bool> UsernameExistsAsync(string username);
    }
}