using System;
using System.Threading.Tasks;
using AutoMapper;
using DisprzTraining.Business.Services;
using DisprzTraining.DataAccess.Repositories;
using DisprzTraining.Models;
using DisprzTraining.Models.DTOs;
using DisprzTraining.Utils;
using Moq;
using Xunit;

namespace DisprzTraining.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepository;
        private readonly IMapper _mapper;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockRepository = new Mock<IUserRepository>();
            
            // Create actual mapper with real mapping profile
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = mapperConfig.CreateMapper();
            
            _service = new UserService(_mockRepository.Object, _mapper);
        }

        [Fact]
        public async Task RegisterAsync_WithNewUsername_ShouldCreateUser()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Username = "newuser",
                Password = "password123",
                Email = "new@example.com",
                FullName = "New User"
            };

            _mockRepository.Setup(repo => repo.UsernameExistsAsync(registerDto.Username))
                .ReturnsAsync(false);

            _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => 
                {
                    u.Id = 1;
                    return u;
                });

            // Act
            var result = await _service.RegisterAsync(registerDto);

            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal(registerDto.Username, result.Username);
            Assert.Equal(registerDto.Email, result.Email);
            Assert.Equal(registerDto.FullName, result.FullName);
            _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WithExistingUsername_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Username = "existinguser",
                Password = "password123",
                Email = "existing@example.com",
                FullName = "Existing User"
            };

            _mockRepository.Setup(repo => repo.UsernameExistsAsync(registerDto.Username))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _service.RegisterAsync(registerDto));
            
            _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnUser()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Username = "validuser",
                Password = "validpassword"
            };

            var user = new User
            {
                Id = 1,
                Username = loginDto.Username,
                Password = loginDto.Password,
                Email = "valid@example.com",
                FullName = "Valid User"
            };

            _mockRepository.Setup(repo => repo.GetByUsernameAsync(loginDto.Username))
                .ReturnsAsync(user);

            // Act
            var result = await _service.LoginAsync(loginDto);

            // Assert
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidUsername_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Username = "invaliduser",
                Password = "password123"
            };

            _mockRepository.Setup(repo => repo.GetByUsernameAsync(loginDto.Username))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _service.LoginAsync(loginDto));
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Username = "validuser",
                Password = "wrongpassword"
            };

            var user = new User
            {
                Id = 1,
                Username = loginDto.Username,
                Password = "correctpassword", // Different from the login password
                Email = "valid@example.com",
                FullName = "Valid User"
            };

            _mockRepository.Setup(repo => repo.GetByUsernameAsync(loginDto.Username))
                .ReturnsAsync(user);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _service.LoginAsync(loginDto));
        }

        [Fact]
        public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Password = "password123",
                Email = "test@example.com",
                FullName = "Test User"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync(user);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var userId = 999;
            _mockRepository.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.GetUserByIdAsync(userId));
        }
    }
}