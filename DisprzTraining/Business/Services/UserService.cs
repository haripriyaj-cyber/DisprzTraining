using System;
using System.Threading.Tasks;
using AutoMapper;
using DisprzTraining.DataAccess.Repositories;
using DisprzTraining.Models;
using DisprzTraining.Models.DTOs;

namespace DisprzTraining.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDTO> RegisterAsync(RegisterDTO registerDto)
        {
            // Check if username already exists
            if (await _userRepository.UsernameExistsAsync(registerDto.Username))
            {
                throw new InvalidOperationException($"Username '{registerDto.Username}' is already taken");
            }

            // Create new user
            var user = new User
            {
                Username = registerDto.Username,
                Password = registerDto.Password, // In a real app, hash this password
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);
            return _mapper.Map<UserDTO>(createdUser);
        }

        public async Task<UserDTO> LoginAsync(LoginDTO loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            
            // Check if user exists and password matches
            if (user == null || user.Password != loginDto.Password)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found");
            }
            
            return _mapper.Map<UserDTO>(user);
        }
    }
}