using System;
using DisprzTraining.Models;
using Xunit;

namespace DisprzTraining.Tests.Models
{
    public class UserTests
    {
        [Fact]

        public void User_DefaultValues_ShouldBeSetCorrectly()
        {
            // Act
            var user = new User();
            var now = DateTimeOffset.UtcNow;

            // Assert
            Assert.Equal(0, user.Id);
            Assert.Null(user.Username);
            Assert.Null(user.Password);
            Assert.Null(user.Email);
            Assert.Null(user.FullName);
            
            // CreatedAt should be initialized to approximately now
            Assert.True((now - user.CreatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void User_WithId_ShouldSetIdProperty()
        {
            // Arrange




            var user = new User
            {
                Id = 42,
                Username = "userWithId"
            };


            // Assert
            Assert.Equal(42, user.Id);
        }

        [Fact]
        public void User_WithAppointments_ShouldAllowAccessToAppointments()
        {
            // Arrange
            var user = new User
            {




                Username = "userWithAppointments"
            };

            // Act - initialize the Appointments collection
            user.Appointments = new System.Collections.Generic.List<Appointment>
            {
                new Appointment { Title = "Meeting 1" },
                new Appointment { Title = "Meeting 2" }
            };

            // Assert





            Assert.Equal(2, user.Appointments.Count);
            Assert.Contains(user.Appointments, a => a.Title == "Meeting 1");
            Assert.Contains(user.Appointments, a => a.Title == "Meeting 2");
        }
    }
}