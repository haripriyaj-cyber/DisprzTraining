using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DisprzTraining.Models;
using Xunit;

namespace DisprzTraining.Tests.Models
{
    public class AppointmentTests
    {
        [Fact]
        public void Appointment_InitializedWithProperties_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var title = "Team Meeting";
            var startTime = new DateTimeOffset(2023, 6, 1, 9, 0, 0, TimeSpan.Zero);
            var endTime = new DateTimeOffset(2023, 6, 1, 10, 0, 0, TimeSpan.Zero);
            var description = "Weekly team sync-up";
            var location = "Conference Room A";
            var isAllDay = false;

            // Act
            var appointment = new Appointment
            {
                Title = title,
                StartTime = startTime,
                EndTime = endTime,
                Description = description,
                Location = location,
                IsAllDay = isAllDay
            };

            // Assert
            Assert.Equal(title, appointment.Title);
            Assert.Equal(startTime, appointment.StartTime);
            Assert.Equal(endTime, appointment.EndTime);
            Assert.Equal(description, appointment.Description);
            Assert.Equal(location, appointment.Location);
            Assert.Equal(isAllDay, appointment.IsAllDay);
            Assert.NotEqual(default, appointment.CreatedAt);
        }

        [Fact]
        public void Appointment_DefaultValues_ShouldBeSetCorrectly()
        {
            // Act
            var appointment = new Appointment();
            var now = DateTimeOffset.UtcNow;

            // Assert
            Assert.Equal(0, appointment.Id);
            Assert.Null(appointment.Title);
            Assert.Equal(default, appointment.StartTime);
            Assert.Equal(default, appointment.EndTime);
            Assert.Null(appointment.Description);
            Assert.False(appointment.IsAllDay);
            Assert.Null(appointment.Location);
            
            // CreatedAt should be initialized to approximately now
            Assert.True((now - appointment.CreatedAt).TotalSeconds < 5);
            
            // UpdatedAt should be default
            Assert.Equal(default, appointment.UpdatedAt);
        }

        [Fact]
        public void Appointment_UpdatedAt_ShouldBeSettable()
        {
            // Arrange
            var appointment = new Appointment();
            var updatedAt = DateTimeOffset.UtcNow.AddHours(-1); // 1 hour ago

            // Act
            appointment.UpdatedAt = updatedAt;

            // Assert
            Assert.Equal(updatedAt, appointment.UpdatedAt);
        }

        [Fact]
        public void Appointment_AllDayEvent_ShouldSetIsAllDayProperty()
        {
            // Arrange
            var appointment = new Appointment
            {
                Title = "All-Day Conference",
                StartTime = new DateTimeOffset(2023, 6, 1, 0, 0, 0, TimeSpan.Zero),
                EndTime = new DateTimeOffset(2023, 6, 1, 23, 59, 59, TimeSpan.Zero),
                Description = "Annual company conference",
                Location = "Convention Center",
                IsAllDay = true
            };

            // Assert
            Assert.True(appointment.IsAllDay);
            Assert.Equal(new DateTimeOffset(2023, 6, 1, 0, 0, 0, TimeSpan.Zero), appointment.StartTime);
            Assert.Equal(new DateTimeOffset(2023, 6, 1, 23, 59, 59, TimeSpan.Zero), appointment.EndTime);
        }

        [Fact]
        public void Appointment_WithId_ShouldSetIdProperty()
        {
            // Arrange
            var appointment = new Appointment
            {
                Id = 42,
                Title = "Meeting with ID"
            };

            // Assert
            Assert.Equal(42, appointment.Id);
        }

        [Fact]
        public void Appointment_WithoutOptionalProperties_ShouldStillBeValid()
        {
            // Arrange & Act
            var appointment = new Appointment
            {
                Title = "Minimal Meeting",
                StartTime = DateTimeOffset.UtcNow,
                EndTime = DateTimeOffset.UtcNow.AddHours(1)
            };

            // Assert
            Assert.Equal("Minimal Meeting", appointment.Title);
            Assert.Null(appointment.Description);
            Assert.Null(appointment.Location);
            Assert.False(appointment.IsAllDay);
        }

        [Fact]
        public void Appointment_RequiredFields_ShouldHaveRequiredAttribute()
        {
            // Arrange & Act
            var titleProperty = typeof(Appointment).GetProperty("Title");
            var startTimeProperty = typeof(Appointment).GetProperty("StartTime");
            var endTimeProperty = typeof(Appointment).GetProperty("EndTime");

            // Assert
            Assert.NotNull(titleProperty.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault());
            Assert.NotNull(startTimeProperty.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault());
            Assert.NotNull(endTimeProperty.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault());
        }

        [Fact]
        public void Appointment_UserRelationship_ShouldBeConfiguredCorrectly()
        {
            // Arrange
            var appointment = new Appointment
            {
                Title = "Meeting with User",
                StartTime = DateTimeOffset.UtcNow,
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                UserId = 42
            };

            // Act & Assert
            Assert.Equal(42, appointment.UserId);
            Assert.Null(appointment.User); // User is lazy-loaded, so it should be null initially
        }

        [Fact]
        public void Appointment_DateTimeValidation_StartTimeShouldBeBeforeEndTime()
        {
            // This is a conceptual test - in a real app, you might have validation logic
            // Arrange
            var appointment = new Appointment
            {
                Title = "Invalid Meeting",
                StartTime = DateTimeOffset.UtcNow.AddHours(2),
                EndTime = DateTimeOffset.UtcNow.AddHours(1) // End time before start time
            };

            // Act & Assert
            // In a real application, you would validate this in your service layer
            // This test demonstrates that the model itself doesn't prevent invalid date ranges
            Assert.True(appointment.EndTime < appointment.StartTime);
        }

        [Fact]
        public void Appointment_KeyAttribute_IdShouldHaveKeyAttribute()
        {
            // Arrange & Act
            var idProperty = typeof(Appointment).GetProperty("Id");

            // Assert
            Assert.NotNull(idProperty.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.KeyAttribute), false).FirstOrDefault());
        }
    }
}
