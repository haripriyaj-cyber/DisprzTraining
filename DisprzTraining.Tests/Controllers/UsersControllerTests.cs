using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DisprzTraining.Business.Services;
using DisprzTraining.Controllers;
using DisprzTraining.Models.DTOs;
using DisprzTraining.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DisprzTraining.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockService;
        private readonly IMapper _mapper;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockService = new Mock<IUserService>();
            
            // Create actual mapper with real mapping profile
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = mapperConfig.CreateMapper();
            
            _controller = new UsersController(_mockService.Object, _mapper);
        }

        [Fact]
        public async Task Register_WithValidData_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Username = "newuser",
                Password = "password123",
                Email = "new@example.com",
                FullName = "New User"
            };

            var userDto = new UserDTO
            {
                Id = 1,
                Username = registerDto.Username,
                Email = registerDto.Email,
                FullName = registerDto.FullName
            };

            _mockService.Setup(service => service.RegisterAsync(registerDto))
                .ReturnsAsync(userDto);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(UsersController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues!["id"]);
            
            var returnedUserDto = Assert.IsType<UserDTO>(createdAtActionResult.Value);
            Assert.Equal(userDto.Id, returnedUserDto.Id);
            Assert.Equal(userDto.Username, returnedUserDto.Username);
        }

        [Fact]
        public async Task Register_WithExistingUsername_ShouldReturnConflict()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Username = "existinguser",
                Password = "password123",
                Email = "existing@example.com",
                FullName = "Existing User"
            };

            _mockService.Setup(service => service.RegisterAsync(registerDto))
                .ThrowsAsync(new InvalidOperationException($"Username '{registerDto.Username}' is already taken"));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal($"Username '{registerDto.Username}' is already taken", conflictResult.Value);
        }

        [Fact]
        public async Task Register_WithInvalidModel_ShouldReturnBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                // Missing required fields
            };

            // Add model validation error
            _controller.ModelState.AddModelError("Username", "Username is required");

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOk()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Username = "validuser",
                Password = "validpassword"
            };

            var userDto = new UserDTO
            {
                Id = 1,
                Username = loginDto.Username,
                Email = "valid@example.com",
                FullName = "Valid User"
            };

            _mockService.Setup(service => service.LoginAsync(loginDto))
                .ReturnsAsync(userDto);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUserDto = Assert.IsType<UserDTO>(okResult.Value);
            Assert.Equal(userDto.Id, returnedUserDto.Id);
            Assert.Equal(userDto.Username, returnedUserDto.Username);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                Username = "invaliduser",
                Password = "invalidpassword"
            };

            _mockService.Setup(service => service.LoginAsync(loginDto))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid username or password"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_WithInvalidModel_ShouldReturnBadRequest()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                // Missing required fields
            };

            // Add model validation error
            _controller.ModelState.AddModelError("Username", "Username is required");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var userId = 1;
            var userDto = new UserDTO
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                FullName = "Test User"
            };

            _mockService.Setup(service => service.GetUserByIdAsync(userId))
                .ReturnsAsync(userDto);

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUserDto = Assert.IsType<UserDTO>(okResult.Value);
            Assert.Equal(userDto.Id, returnedUserDto.Id);
            Assert.Equal(userDto.Username, returnedUserDto.Username);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var userId = 999;
            
            // Setup the mock to throw KeyNotFoundException
            _mockService.Setup(service => service.GetUserByIdAsync(userId))
                .ThrowsAsync(new KeyNotFoundException($"User with ID {userId} not found"));

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"User with ID {userId} not found", notFoundResult.Value);
        }
    }
}