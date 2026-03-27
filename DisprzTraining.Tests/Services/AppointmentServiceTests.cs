using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DisprzTraining.Business.Services;
using DisprzTraining.DataAccess.Repositories;
using DisprzTraining.Models;
using DisprzTraining.Models.DTOs;
using Moq;
using Xunit;

namespace DisprzTraining.Tests.Services
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentRepository> _mockRepository;
        private readonly AppointmentService _service;

        public AppointmentServiceTests()
        {
            _mockRepository = new Mock<IAppointmentRepository>();
            _service = new AppointmentService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAppointmentsAsync_ShouldReturnAllAppointments()
        {
            // Arrange
            var expectedAppointments = new List<Appointment>
            {
                new Appointment { 
                    Id = 1, 
                    Title = "Meeting 1",
                    StartTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.AddHours(1),
                    Description = "Test Description 1",
                    Location = "Test Location 1"
                },
                new Appointment { 
                    Id = 2, 
                    Title = "Meeting 2",
                    StartTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.AddHours(1),
                    Description = "Test Description 2",
                    Location = "Test Location 2"
                }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedAppointments);

            // Act
            var result = await _service.GetAllAppointmentsAsync();

            // Assert
            Assert.Equal(expectedAppointments, result);
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_WithValidId_ShouldReturnAppointment()
        {
            // Arrange
            var appointmentId = 1;
            var expectedAppointment = new Appointment { 
                Id = appointmentId, 
                Title = "Test Meeting",
                StartTime = DateTimeOffset.UtcNow,
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                Description = "Test Description",
                Location = "Test Location"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(appointmentId))
                .ReturnsAsync(expectedAppointment);

            // Act
            var result = await _service.GetAppointmentByIdAsync(appointmentId);

            // Assert
            Assert.Equal(expectedAppointment, result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(appointmentId), Times.Once);
        }

        [Fact]
        public async Task GetAppointmentByIdAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var appointmentId = 999;
            _mockRepository.Setup(repo => repo.GetByIdAsync(appointmentId))
                .ReturnsAsync((Appointment?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.GetAppointmentByIdAsync(appointmentId));
        }

        [Fact]
        public async Task CreateAppointmentAsync_WithValidData_ShouldCreateAppointment()
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

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Appointment>());

            _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Appointment>()))
                .ReturnsAsync((Appointment a) => 
                {
                    a.Id = 1;
                    return a;
                });

            // Act
            var result = await _service.CreateAppointmentAsync(createDto);

            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal(createDto.Title, result.Title);
            Assert.Equal(createDto.StartTime, result.StartTime);
            Assert.Equal(createDto.EndTime, result.EndTime);
            Assert.Equal(createDto.Description, result.Description);
            Assert.Equal(createDto.Location, result.Location);
            _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<Appointment>()), Times.Once);
        }

        [Fact]
        public async Task CreateAppointmentAsync_WithEndTimeBeforeStartTime_ShouldThrowArgumentException()
        {
            // Arrange
            var createDto = new CreateAppointmentDTO
            {
                Title = "Invalid Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(2), // Start time after end time
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                Description = "Test Description",
                Location = "Test Location"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.CreateAppointmentAsync(createDto));
        }

        [Fact]
        public async Task CreateAppointmentAsync_WithOverlappingAppointment_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var createDto = new CreateAppointmentDTO
            {
                Title = "New Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(1),
                EndTime = DateTimeOffset.UtcNow.AddHours(2),
                Description = "Test Description",
                Location = "Test Location",
                UserId = 1 // Add UserId
            };

            var existingAppointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = 1,
                    Title = "Existing Meeting",
                    StartTime = DateTimeOffset.UtcNow.AddMinutes(30), // Overlaps with new meeting
                    EndTime = DateTimeOffset.UtcNow.AddHours(1).AddMinutes(30),
                    Description = "Existing Description",
                    Location = "Existing Location",
                    UserId = 1 // Add UserId
                }
            };

            // Setup GetByUserIdAsync instead of GetAllAsync
            _mockRepository.Setup(repo => repo.GetByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(existingAppointments);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _service.CreateAppointmentAsync(createDto));
            
            // Verify the exception message contains the expected text
            Assert.Contains("conflicts with existing appointment", exception.Message);
        }

        [Fact]
        public async Task UpdateAppointmentAsync_WithValidData_ShouldUpdateAppointment()
        {
            // Arrange
            var appointmentId = 1;
            var existingAppointment = new Appointment
            {
                Id = appointmentId,
                Title = "Original Meeting",
                StartTime = DateTimeOffset.UtcNow,
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                Description = "Original Description",
                Location = "Original Location"
            };

            var updateDto = new UpdateAppointmentDTO
            {
                Title = "Updated Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(2),
                EndTime = DateTimeOffset.UtcNow.AddHours(3),
                Description = "Updated Description",
                Location = "Updated Location"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(appointmentId))
                .ReturnsAsync(existingAppointment);

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Appointment> { existingAppointment });

            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Appointment>()))
                .ReturnsAsync((Appointment a) => a);

            // Act
            var result = await _service.UpdateAppointmentAsync(appointmentId, updateDto);

            // Assert
            Assert.Equal(appointmentId, result.Id);
            Assert.Equal(updateDto.Title, result.Title);
            Assert.Equal(updateDto.StartTime, result.StartTime);
            Assert.Equal(updateDto.EndTime, result.EndTime);
            Assert.Equal(updateDto.Description, result.Description);
            Assert.Equal(updateDto.Location, result.Location);
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Appointment>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAppointmentAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var appointmentId = 999;
            var updateDto = new UpdateAppointmentDTO
            {
                Title = "Updated Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(2),
                EndTime = DateTimeOffset.UtcNow.AddHours(3),
                Description = "Updated Description",
                Location = "Updated Location"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(appointmentId))
                .ReturnsAsync((Appointment)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.UpdateAppointmentAsync(appointmentId, updateDto));
        }

        [Fact]
        public async Task UpdateAppointmentAsync_WithEndTimeBeforeStartTime_ShouldThrowArgumentException()
        {
            // Arrange
            var appointmentId = 1;
            var existingAppointment = new Appointment
            {
                Id = appointmentId,
                Title = "Original Meeting",
                StartTime = DateTimeOffset.UtcNow,
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                Description = "Original Description",
                Location = "Original Location"
            };

            var updateDto = new UpdateAppointmentDTO
            {
                Title = "Invalid Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(2), // Start time after end time
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                Description = "Updated Description",
                Location = "Updated Location"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(appointmentId))
                .ReturnsAsync(existingAppointment);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.UpdateAppointmentAsync(appointmentId, updateDto));
        }

        [Fact]
        public async Task UpdateAppointmentAsync_WithOverlappingAppointment_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var appointmentId = 1;
            var existingAppointment = new Appointment
            {
                Id = appointmentId,
                Title = "Original Meeting",
                StartTime = DateTimeOffset.UtcNow,
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                Description = "Original Description",
                Location = "Original Location",
                UserId = 1 // Add UserId
            };

            var otherAppointment = new Appointment
            {
                Id = 2,
                Title = "Other Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(2).AddMinutes(30),
                EndTime = DateTimeOffset.UtcNow.AddHours(3).AddMinutes(30),
                Description = "Other Description",
                Location = "Other Location",
                UserId = 1 // Add UserId
            };

            var updateDto = new UpdateAppointmentDTO
            {
                Title = "Updated Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(2), // Overlaps with other meeting
                EndTime = DateTimeOffset.UtcNow.AddHours(3),
                Description = "Updated Description",
                Location = "Updated Location"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(appointmentId))
                .ReturnsAsync(existingAppointment);

            // Setup GetByUserIdAsync instead of GetAllAsync
            _mockRepository.Setup(repo => repo.GetByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Appointment> { existingAppointment, otherAppointment });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _service.UpdateAppointmentAsync(appointmentId, updateDto));
            
            // Verify the exception message contains the expected text
            Assert.Contains("conflicts with existing appointment", exception.Message);
        }

        [Fact]
        public async Task DeleteAppointmentAsync_WithValidId_ShouldDeleteAppointment()
        {
            // Arrange
            var appointmentId = 1;
            var existingAppointment = new Appointment
            {
                Id = appointmentId,
                Title = "Meeting to Delete",
                StartTime = DateTimeOffset.UtcNow,
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                Description = "Test Description",
                Location = "Test Location"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(appointmentId))
                .ReturnsAsync(existingAppointment);

            _mockRepository.Setup(repo => repo.DeleteAsync(appointmentId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAppointmentAsync(appointmentId);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.DeleteAsync(appointmentId), Times.Once);
        }

        [Fact]
        public async Task DeleteAppointmentAsync_WithInvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var appointmentId = 999;
            _mockRepository.Setup(repo => repo.GetByIdAsync(appointmentId))
                .ReturnsAsync((Appointment)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => 
                _service.DeleteAppointmentAsync(appointmentId));
        }

        [Fact]
        public async Task GetAppointmentsByUserIdAsync_ShouldReturnUserAppointments()
        {
            // Arrange
            var userId = 1;
            var expectedAppointments = new List<Appointment>
            {
                new Appointment { 
                    Id = 1, 
                    Title = "User Meeting 1",
                    StartTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.AddHours(1),
                    Description = "User Description 1",
                    Location = "User Location 1",
                    UserId = userId
                },
                new Appointment { 
                    Id = 2, 
                    Title = "User Meeting 2",
                    StartTime = DateTimeOffset.UtcNow.AddDays(1),
                    EndTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
                    Description = "User Description 2",
                    Location = "User Location 2",
                    UserId = userId
                }
            };

            _mockRepository.Setup(repo => repo.GetByUserIdAsync(userId))
                .ReturnsAsync(expectedAppointments);

            // Act
            var result = await _service.GetAppointmentsByUserIdAsync(userId);

            // Assert
            Assert.Equal(expectedAppointments, result);
            _mockRepository.Verify(repo => repo.GetByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task CreateAppointmentAsync_WithSameStartAndEndTime_ShouldThrowArgumentException()
        {
            // Arrange
            var sameTime = DateTimeOffset.UtcNow.AddHours(1);
            var createDto = new CreateAppointmentDTO
            {
                Title = "Invalid Meeting",
                StartTime = sameTime,
                EndTime = sameTime, // Same as start time
                Description = "Test Description",
                Location = "Test Location"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _service.CreateAppointmentAsync(createDto));
        }

        [Fact]
        public async Task CreateAppointmentAsync_WithAllDayEvent_ShouldSetIsAllDayProperty()
        {
            // Arrange
            var createDto = new CreateAppointmentDTO
            {
                Title = "All-Day Meeting",
                StartTime = new DateTimeOffset(DateTime.Today),
                EndTime = new DateTimeOffset(DateTime.Today.AddDays(1).AddTicks(-1)),
                Description = "All-day event description",
                Location = "Test Location",
                IsAllDay = true
            };

            _mockRepository.Setup(repo => repo.GetByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Appointment>());

            _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Appointment>()))
                .ReturnsAsync((Appointment a) => 
                {
                    a.Id = 1;
                    return a;
                });

            // Act
            var result = await _service.CreateAppointmentAsync(createDto);

            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal(createDto.Title, result.Title);
            Assert.True(result.IsAllDay);
            _mockRepository.Verify(repo => repo.CreateAsync(It.Is<Appointment>(a => a.IsAllDay == true)), Times.Once);
        }

        [Fact]
        public async Task UpdateAppointmentAsync_WithChangedUserId_ShouldNotChangeUserId()
        {
            // Arrange
            var appointmentId = 1;
            var originalUserId = 42;
            var existingAppointment = new Appointment
            {
                Id = appointmentId,
                Title = "Original Meeting",
                StartTime = DateTimeOffset.UtcNow,
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                Description = "Original Description",
                Location = "Original Location",
                UserId = originalUserId
            };

            var updateDto = new UpdateAppointmentDTO
            {
                Title = "Updated Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(2),
                EndTime = DateTimeOffset.UtcNow.AddHours(3),
                Description = "Updated Description",
                Location = "Updated Location"
                // Note: UpdateAppointmentDTO typically doesn't include UserId
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(appointmentId))
                .ReturnsAsync(existingAppointment);

            _mockRepository.Setup(repo => repo.GetByUserIdAsync(originalUserId))
                .ReturnsAsync(new List<Appointment> { existingAppointment });

            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Appointment>()))
                .ReturnsAsync((Appointment a) => a);

            // Act
            var result = await _service.UpdateAppointmentAsync(appointmentId, updateDto);

            // Assert
            Assert.Equal(appointmentId, result.Id);
            Assert.Equal(originalUserId, result.UserId); // UserId should not change
            Assert.Equal(updateDto.Title, result.Title);
            _mockRepository.Verify(repo => repo.UpdateAsync(It.Is<Appointment>(a => a.UserId == originalUserId)), Times.Once);
        }
    }
}
