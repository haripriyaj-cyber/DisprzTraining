using System;
using AutoMapper;
using DisprzTraining.Models;
using DisprzTraining.Models.DTOs;
using DisprzTraining.Utils;
using Xunit;

namespace DisprzTraining.Tests.Mapping
{
    public class MappingProfileTests
    {
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            
            // Verify the configuration is valid
            mapperConfig.AssertConfigurationIsValid();
            
            _mapper = mapperConfig.CreateMapper();
        }

        [Fact]
        public void Mapper_Configuration_IsValid()
        {
            // This test will fail if the mapping configuration is invalid
            // The assertion is done in the constructor with AssertConfigurationIsValid()
            // This test just ensures the constructor completes without errors
            Assert.NotNull(_mapper);
        }

        [Fact]
        public void Map_Appointment_To_AppointmentDTO_MapsCorrectly()
        {
            // Arrange
            var appointment = new Appointment
            {
                Id = 42,
                Title = "Test Meeting",
                StartTime = new DateTimeOffset(2023, 6, 1, 9, 0, 0, TimeSpan.Zero),
                EndTime = new DateTimeOffset(2023, 6, 1, 10, 0, 0, TimeSpan.Zero),
                Description = "Test Description",
                Location = "Test Location",
                IsAllDay = false,
                CreatedAt = new DateTimeOffset(2023, 5, 25, 14, 30, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2023, 5, 26, 9, 15, 0, TimeSpan.Zero)
            };

            // Act
            var appointmentDto = _mapper.Map<AppointmentDTO>(appointment);

            // Assert
            Assert.Equal(appointment.Id, appointmentDto.Id);
            Assert.Equal(appointment.Title, appointmentDto.Title);
            Assert.Equal(appointment.StartTime, appointmentDto.StartTime);
            Assert.Equal(appointment.EndTime, appointmentDto.EndTime);
            Assert.Equal(appointment.Description, appointmentDto.Description);
            Assert.Equal(appointment.Location, appointmentDto.Location);
            Assert.Equal(appointment.IsAllDay, appointmentDto.IsAllDay);
            Assert.Equal(appointment.CreatedAt, appointmentDto.CreatedAt);
            Assert.Equal(appointment.UpdatedAt, appointmentDto.UpdatedAt);
        }

        [Fact]
        public void Map_CreateAppointmentDTO_To_Appointment_MapsCorrectly()
        {
            // Arrange
            var createDto = new CreateAppointmentDTO
            {
                Title = "New Meeting",
                StartTime = new DateTimeOffset(2023, 6, 1, 9, 0, 0, TimeSpan.Zero),
                EndTime = new DateTimeOffset(2023, 6, 1, 10, 0, 0, TimeSpan.Zero),
                Description = "New Description",
                Location = "New Location",
                IsAllDay = false
            };

            // Act
            var appointment = _mapper.Map<Appointment>(createDto);

            // Assert
            Assert.Equal(createDto.Title, appointment.Title);
            Assert.Equal(createDto.StartTime, appointment.StartTime);
            Assert.Equal(createDto.EndTime, appointment.EndTime);
            Assert.Equal(createDto.Description, appointment.Description);
            Assert.Equal(createDto.Location, appointment.Location);
            Assert.Equal(createDto.IsAllDay, appointment.IsAllDay);
            
            // Default values should be set
            Assert.Equal(0, appointment.Id); // Default int value
            Assert.NotEqual(default, appointment.CreatedAt); // Should have a default value
        }

        [Fact]
        public void Map_UpdateAppointmentDTO_To_Appointment_MapsCorrectly()
        {
            // Arrange
            var updateDto = new UpdateAppointmentDTO
            {
                Title = "Updated Meeting",
                StartTime = new DateTimeOffset(2023, 6, 1, 9, 30, 0, TimeSpan.Zero),
                EndTime = new DateTimeOffset(2023, 6, 1, 10, 30, 0, TimeSpan.Zero),
                Description = "Updated Description",
                Location = "Updated Location",
                IsAllDay = true
            };

            // Act
            var appointment = _mapper.Map<Appointment>(updateDto);

            // Assert
            Assert.Equal(updateDto.Title, appointment.Title);
            Assert.Equal(updateDto.StartTime, appointment.StartTime);
            Assert.Equal(updateDto.EndTime, appointment.EndTime);
            Assert.Equal(updateDto.Description, appointment.Description);
            Assert.Equal(updateDto.Location, appointment.Location);
            Assert.Equal(updateDto.IsAllDay, appointment.IsAllDay);
            
            // Default values should be set
            Assert.Equal(0, appointment.Id); // Default int value
            Assert.NotEqual(default, appointment.CreatedAt); // Should have a default value
        }

        [Fact]
        public void Map_UpdateAppointmentDTO_To_ExistingAppointment_PreservesOriginalValues()
        {
            // Arrange
            var existingAppointment = new Appointment
            {
                Id = 42,
                Title = "Original Meeting",
                StartTime = new DateTimeOffset(2023, 5, 1, 9, 0, 0, TimeSpan.Zero),
                EndTime = new DateTimeOffset(2023, 5, 1, 10, 0, 0, TimeSpan.Zero),
                Description = "Original Description",
                Location = "Original Location",
                IsAllDay = false,
                CreatedAt = new DateTimeOffset(2023, 4, 25, 14, 30, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2023, 4, 26, 9, 15, 0, TimeSpan.Zero)
            };

            var updateDto = new UpdateAppointmentDTO
            {
                Title = "Updated Meeting",
                StartTime = new DateTimeOffset(2023, 6, 1, 9, 30, 0, TimeSpan.Zero),
                EndTime = new DateTimeOffset(2023, 6, 1, 10, 30, 0, TimeSpan.Zero),
                Description = "Updated Description",
                Location = "Updated Location",
                IsAllDay = true
            };

            // Act
            _mapper.Map(updateDto, existingAppointment);

            // Assert
            // Updated properties
            Assert.Equal(updateDto.Title, existingAppointment.Title);
            Assert.Equal(updateDto.StartTime, existingAppointment.StartTime);
            Assert.Equal(updateDto.EndTime, existingAppointment.EndTime);
            Assert.Equal(updateDto.Description, existingAppointment.Description);
            Assert.Equal(updateDto.Location, existingAppointment.Location);
            Assert.Equal(updateDto.IsAllDay, existingAppointment.IsAllDay);
            
            // Preserved properties
            Assert.Equal(42, existingAppointment.Id);
            Assert.Equal(new DateTimeOffset(2023, 4, 25, 14, 30, 0, TimeSpan.Zero), existingAppointment.CreatedAt);
        }
    }
}
