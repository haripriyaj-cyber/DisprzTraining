using System.Threading.Tasks;
using DisprzTraining.Models;
using DisprzTraining.Models.DTOs;

namespace DisprzTraining.Business.Services
{
    public interface IUserService
    {
        Task<UserDTO> RegisterAsync(RegisterDTO registerDto);
        Task<UserDTO> LoginAsync(LoginDTO loginDto);
        Task<UserDTO> GetUserByIdAsync(int id);
    }
}