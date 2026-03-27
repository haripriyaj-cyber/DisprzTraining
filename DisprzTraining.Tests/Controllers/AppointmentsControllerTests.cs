using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DisprzTraining.Business.Services;
using DisprzTraining.Controllers;
using DisprzTraining.Models;
using DisprzTraining.Models.DTOs;
using DisprzTraining.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DisprzTraining.Tests.Controllers
{
    public class AppointmentsControllerTests
    {
        private readonly Mock<IAppointmentService> _mockService;
        private readonly IMapper _mapper;
        private readonly AppointmentsController _controller;

        public AppointmentsControllerTests()
        {
            _mockService = new Mock<IAppointmentService>();
            
            // Create actual mapper with real mapping profile
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = mapperConfig.CreateMapper();
            
            _controller = new AppointmentsController(_mockService.Object, _mapper);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithAppointments()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment 
                { 
                    Id = 1, 
                    Title = "Meeting 1",
                    StartTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.AddHours(1),
                    Description = "Test Description 1",
                    Location = "Test Location 1"
                },
                new Appointment 
                { 
                    Id = 2, 
                    Title = "Meeting 2",
                    StartTime = DateTimeOffset.UtcNow.AddDays(1),
                    EndTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
                    Description = "Test Description 2",
                    Location = "Test Location 2"
                }
            };

            _mockService.Setup(service => service.GetAllAppointmentsAsync())
                .ReturnsAsync(appointments);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAppointments = Assert.IsAssignableFrom<IEnumerable<AppointmentDTO>>(okResult.Value);
            Assert.Equal(2, ((List<AppointmentDTO>)returnedAppointments).Count);
        }

        [Fact]
        public async Task GetAll_WithNoAppointments_ShouldReturnEmptyList()
        {
            // Arrange
            _mockService.Setup(service => service.GetAllAppointmentsAsync())
                .ReturnsAsync(new List<Appointment>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAppointments = Assert.IsAssignableFrom<IEnumerable<AppointmentDTO>>(okResult.Value);
            Assert.Empty(returnedAppointments);
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnOkWithAppointment()
        {
            // Arrange
            var appointmentId = 1;
            var appointment = new Appointment 
            { 
                Id = appointmentId, 
                Title = "Test Meeting",
                StartTime = DateTimeOffset.UtcNow,
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                Description = "Test Description",
                Location = "Test Location"
            };

            _mockService.Setup(service => service.GetAppointmentByIdAsync(appointmentId))
                .ReturnsAsync(appointment);

            // Act
            var result = await _controller.GetById(appointmentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAppointment = Assert.IsType<AppointmentDTO>(okResult.Value);
            Assert.Equal(appointmentId, returnedAppointment.Id);
            Assert.Equal(appointment.Title, returnedAppointment.Title);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var appointmentId = 999;
            _mockService.Setup(service => service.GetAppointmentByIdAsync(appointmentId))
                .ThrowsAsync(new KeyNotFoundException($"Appointment with ID {appointmentId} not found"));

            // Act
            var result = await _controller.GetById(appointmentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Appointment with ID {appointmentId} not found", notFoundResult.Value);
        }

        [Fact]
        public async Task Create_WithValidData_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createDto = new CreateAppointmentDTO
            {
                Title = "New Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2),
                Description = "Test Description",
                Location = "Test Location"
            };

            var createdAppointment = new Appointment
            {
                Id = 1,
                Title = createDto.Title,
                StartTime = createDto.StartTime,
                EndTime = createDto.EndTime,
                Description = createDto.Description,
                Location = createDto.Location,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _mockService.Setup(service => service.CreateAppointmentAsync(createDto))
                .ReturnsAsync(createdAppointment);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(AppointmentsController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);
            
            var returnedAppointment = Assert.IsType<AppointmentDTO>(createdAtActionResult.Value);
            Assert.Equal(createdAppointment.Id, returnedAppointment.Id);
            Assert.Equal(createdAppointment.Title, returnedAppointment.Title);
        }

        [Fact]
        public async Task Create_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var createDto = new CreateAppointmentDTO
            {
                Title = "Invalid Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(2), // End time before start time
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                Description = "Test Description",
                Location = "Test Location"
            };

            _mockService.Setup(service => service.CreateAppointmentAsync(createDto))
                .ThrowsAsync(new ArgumentException("End time must be after start time"));

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("End time must be after start time", badRequestResult.Value);
        }

        [Fact]
        public async Task Create_WithConflictingAppointment_ShouldReturnConflict()
        {
            // Arrange
            var createDto = new CreateAppointmentDTO
            {
                Title = "Conflicting Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2),
                Description = "Test Description",
                Location = "Test Location"
            };

            var conflictMessage = "Appointment conflicts with existing appointment 'Existing Meeting' scheduled from 1:00 PM to 2:00 PM";
            _mockService.Setup(service => service.CreateAppointmentAsync(createDto))
                .ThrowsAsync(new InvalidOperationException(conflictMessage));

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(conflictMessage, conflictResult.Value);
        }

        [Fact]
        public async Task Update_WithValidData_ShouldReturnOkWithUpdatedAppointment()
        {
            // Arrange
            var appointmentId = 1;
            var updateDto = new UpdateAppointmentDTO
            {
                Title = "Updated Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2),
                Description = "Updated Description",
                Location = "Updated Location"
            };

            var updatedAppointment = new Appointment
            {
                Id = appointmentId,
                Title = updateDto.Title,
                StartTime = updateDto.StartTime,
                EndTime = updateDto.EndTime,
                Description = updateDto.Description,
                Location = updateDto.Location,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            _mockService.Setup(service => service.UpdateAppointmentAsync(appointmentId, updateDto))
                .ReturnsAsync(updatedAppointment);

            // Act
            var result = await _controller.Update(appointmentId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAppointment = Assert.IsType<AppointmentDTO>(okResult.Value);
            Assert.Equal(appointmentId, returnedAppointment.Id);
            Assert.Equal(updateDto.Title, returnedAppointment.Title);
            Assert.Equal(updateDto.Description, returnedAppointment.Description);
        }

        [Fact]
        public async Task Update_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var appointmentId = 999;
            var updateDto = new UpdateAppointmentDTO
            {
                Title = "Updated Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2),
                Description = "Updated Description",
                Location = "Updated Location"
            };

            _mockService.Setup(service => service.UpdateAppointmentAsync(appointmentId, updateDto))
                .ThrowsAsync(new KeyNotFoundException($"Appointment with ID {appointmentId} not found"));

            // Act
            var result = await _controller.Update(appointmentId, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Appointment with ID {appointmentId} not found", notFoundResult.Value);
        }

        [Fact]
        public async Task Update_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var appointmentId = 1;
            var updateDto = new UpdateAppointmentDTO
            {
                Title = "Invalid Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(2), // End time before start time
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                Description = "Updated Description",
                Location = "Updated Location"
            };

            _mockService.Setup(service => service.UpdateAppointmentAsync(appointmentId, updateDto))
                .ThrowsAsync(new ArgumentException("End time must be after start time"));

            // Act
            var result = await _controller.Update(appointmentId, updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("End time must be after start time", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_WithConflictingAppointment_ShouldReturnConflict()
        {
            // Arrange
            var appointmentId = 1;
            var updateDto = new UpdateAppointmentDTO
            {
                Title = "Conflicting Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2),
                Description = "Updated Description",
                Location = "Updated Location"
            };

            var conflictMessage = "Appointment conflicts with existing appointment 'Existing Meeting' scheduled from 1:00 PM to 2:00 PM";
            _mockService.Setup(service => service.UpdateAppointmentAsync(appointmentId, updateDto))
                .ThrowsAsync(new InvalidOperationException(conflictMessage));

            // Act
            var result = await _controller.Update(appointmentId, updateDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(conflictMessage, conflictResult.Value);
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldReturnNoContent()
        {
            // Arrange
            var appointmentId = 1;
            _mockService.Setup(service => service.DeleteAppointmentAsync(appointmentId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(appointmentId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var appointmentId = 999;
            _mockService.Setup(service => service.DeleteAppointmentAsync(appointmentId))
                .ThrowsAsync(new KeyNotFoundException($"Appointment with ID {appointmentId} not found"));

            // Act
            var result = await _controller.Delete(appointmentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Appointment with ID {appointmentId} not found", notFoundResult.Value);
        }

        [Fact]
        public async Task Create_WithModelValidationErrors_ShouldReturnBadRequest()
        {
            // Arrange
            // Set up controller with invalid ModelState
            _controller.ModelState.AddModelError("Title", "Title is required");

            var createDto = new CreateAppointmentDTO
            {
                // Missing required Title
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2)
            };

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Update_WithModelValidationErrors_ShouldReturnBadRequest()
        {
            // Arrange
            // Set up controller with invalid ModelState
            _controller.ModelState.AddModelError("Title", "Title is required");

            var appointmentId = 1;
            var updateDto = new UpdateAppointmentDTO
            {
                // Missing required Title
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2)
            };

            // Act
            var result = await _controller.Update(appointmentId, updateDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
