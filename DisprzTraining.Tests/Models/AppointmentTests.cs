using System;
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
    }
}
