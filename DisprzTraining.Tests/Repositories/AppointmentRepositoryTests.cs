using System;
using System.Linq;
using System.Threading.Tasks;
using DisprzTraining.DataAccess;
using DisprzTraining.DataAccess.Repositories;
using DisprzTraining.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DisprzTraining.Tests.Repositories
{
    public class AppointmentRepositoryTests
    {
        private AppDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
                
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllAppointments()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString(); // Unique DB name for test isolation
            using (var context = CreateInMemoryContext(dbName))
            {
                context.Appointments.Add(new Appointment { 
                    Title = "Meeting 1",
                    StartTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.AddHours(1),
                    Description = "Test Description 1", // Add required property
                    Location = "Test Location 1"        // Add required property
                });
                context.Appointments.Add(new Appointment { 
                    Title = "Meeting 2",
                    StartTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.AddHours(1),
                    Description = "Test Description 2", // Add required property
                    Location = "Test Location 2"        // Add required property
                });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new AppointmentRepository(context);
                var appointments = await repository.GetAllAsync();

                // Assert
                Assert.Equal(2, appointments.Count());
                Assert.Contains(appointments, a => a.Title == "Meeting 1");
                Assert.Contains(appointments, a => a.Title == "Meeting 2");
            }
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnAppointment()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var appointmentId = 0;
            
            using (var context = CreateInMemoryContext(dbName))
            {
                var appointment = new Appointment { 
                    Title = "Test Meeting",
                    StartTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.AddHours(1),
                    Description = "Test Description", // Add required property
                    Location = "Test Location"        // Add required property
                };
                context.Appointments.Add(appointment);
                await context.SaveChangesAsync();
                appointmentId = appointment.Id;
            }

            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new AppointmentRepository(context);
                var appointment = await repository.GetByIdAsync(appointmentId);

                // Assert
                Assert.NotNull(appointment);
                Assert.Equal("Test Meeting", appointment.Title);
            }
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAppointmentToDatabase()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var appointment = new Appointment 
            { 
                Title = "New Meeting",
                StartTime = DateTimeOffset.UtcNow,
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                Description = "Test Description",
                Location = "Test Location"
            };

            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new AppointmentRepository(context);
                var result = await repository.CreateAsync(appointment);

                // Assert
                Assert.NotEqual(0, result.Id); // ID should be set
                Assert.Equal(appointment.Title, result.Title);
            }

            // Verify it was saved to the database
            using (var context = CreateInMemoryContext(dbName))
            {
                Assert.Equal(1, await context.Appointments.CountAsync());
                var savedAppointment = await context.Appointments.FirstAsync();
                Assert.Equal("New Meeting", savedAppointment.Title);
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAppointmentInDatabase()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var appointmentId = 0;
            
            using (var context = CreateInMemoryContext(dbName))
            {
                var appointment = new Appointment 
                { 
                    Title = "Original Meeting",
                    StartTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.AddHours(1),
                    Description = "Original Description",
                    Location = "Original Location"
                };
                context.Appointments.Add(appointment);
                await context.SaveChangesAsync();
                appointmentId = appointment.Id;
            }

            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new AppointmentRepository(context);
                var appointment = await repository.GetByIdAsync(appointmentId);
                appointment.Title = "Updated Meeting";
                appointment.Description = "Updated Description";
                var result = await repository.UpdateAsync(appointment);

                // Assert
                Assert.Equal(appointmentId, result.Id);
                Assert.Equal("Updated Meeting", result.Title);
                Assert.Equal("Updated Description", result.Description);
            }

            // Verify it was updated in the database
            using (var context = CreateInMemoryContext(dbName))
            {
                var updatedAppointment = await context.Appointments.FindAsync(appointmentId);
                Assert.Equal("Updated Meeting", updatedAppointment.Title);
                Assert.Equal("Updated Description", updatedAppointment.Description);
            }
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveAppointmentFromDatabase()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var appointmentId = 0;
            
            using (var context = CreateInMemoryContext(dbName))
            {
                var appointment = new Appointment 
                { 
                    Title = "Meeting to Delete",
                    StartTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.AddHours(1),
                    Description = "Test Description",
                    Location = "Test Location"
                };
                context.Appointments.Add(appointment);
                await context.SaveChangesAsync();
                appointmentId = appointment.Id;
            }

            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new AppointmentRepository(context);
                var result = await repository.DeleteAsync(appointmentId);

                // Assert
                Assert.True(result);
            }

            // Verify it was removed from the database
            using (var context = CreateInMemoryContext(dbName))
            {
                var deletedAppointment = await context.Appointments.FindAsync(appointmentId);
                Assert.Null(deletedAppointment);
            }
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var nonExistentId = 999;

            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new AppointmentRepository(context);
                var result = await repository.DeleteAsync(nonExistentId);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnUserAppointments()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var userId = 1;
            
            using (var context = CreateInMemoryContext(dbName))
            {
                context.Appointments.Add(new Appointment { 
                    Title = "User Meeting 1",
                    StartTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.AddHours(1),
                    Description = "User Description 1",
                    Location = "User Location 1",
                    UserId = userId
                });
                context.Appointments.Add(new Appointment { 
                    Title = "User Meeting 2",
                    StartTime = DateTimeOffset.UtcNow.AddDays(1),
                    EndTime = DateTimeOffset.UtcNow.AddDays(1).AddHours(1),
                    Description = "User Description 2",
                    Location = "User Location 2",
                    UserId = userId
                });
                context.Appointments.Add(new Appointment { 
                    Title = "Other User Meeting",
                    StartTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.AddHours(1),
                    Description = "Other Description",
                    Location = "Other Location",
                    UserId = 2 // Different user
                });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new AppointmentRepository(context);
                var appointments = await repository.GetByUserIdAsync(userId);

                // Assert
                Assert.Equal(2, appointments.Count());
                Assert.All(appointments, a => Assert.Equal(userId, a.UserId));
            }
        }

        [Fact]
        public async Task ConcurrentOperations_ShouldHandleCorrectly()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var appointment1 = new Appointment 
            { 
                Title = "Meeting 1",
                StartTime = DateTimeOffset.UtcNow,
                EndTime = DateTimeOffset.UtcNow.AddHours(1),
                Description = "Description 1",
                Location = "Location 1"
            };
            var appointment2 = new Appointment 
            { 
                Title = "Meeting 2",
                StartTime = DateTimeOffset.UtcNow.AddHours(2),
                EndTime = DateTimeOffset.UtcNow.AddHours(3),
                Description = "Description 2",
                Location = "Location 2"
            };

            // Act & Assert
            using (var context1 = CreateInMemoryContext(dbName))
            using (var context2 = CreateInMemoryContext(dbName))
            {
                var repository1 = new AppointmentRepository(context1);
                var repository2 = new AppointmentRepository(context2);

                // Add first appointment with repository1
                await repository1.CreateAsync(appointment1);
                
                // Add second appointment with repository2
                await repository2.CreateAsync(appointment2);
                
                // Verify both appointments are in the database
                var allAppointments = await repository1.GetAllAsync();
                Assert.Equal(2, allAppointments.Count());
                Assert.Contains(allAppointments, a => a.Title == "Meeting 1");
                Assert.Contains(allAppointments, a => a.Title == "Meeting 2");
            }
        }

        [Fact]
        public async Task GetAllAsync_WithMultipleUsers_ShouldReturnAllAppointments()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = CreateInMemoryContext(dbName))
            {
                context.Appointments.Add(new Appointment { 
                    Title = "User 1 Meeting",
                    StartTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.AddHours(1),
                    Description = "User 1 Description",
                    Location = "User 1 Location",
                    UserId = 1
                });
                context.Appointments.Add(new Appointment { 
                    Title = "User 2 Meeting",
                    StartTime = DateTimeOffset.UtcNow,
                    EndTime = DateTimeOffset.UtcNow.AddHours(1),
                    Description = "User 2 Description",
                    Location = "User 2 Location",
                    UserId = 2
                });
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new AppointmentRepository(context);
                var appointments = await repository.GetAllAsync();

                // Assert
                Assert.Equal(2, appointments.Count());
                Assert.Contains(appointments, a => a.UserId == 1);
                Assert.Contains(appointments, a => a.UserId == 2);
            }
        }

        [Fact]
        public async Task UpdateAsync_WithChangedFields_ShouldUpdateOnlySpecifiedFields()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var appointmentId = 0;
            var originalStartTime = DateTimeOffset.UtcNow;
            var originalEndTime = DateTimeOffset.UtcNow.AddHours(1);
            
            using (var context = CreateInMemoryContext(dbName))
            {
                var appointment = new Appointment 
                { 
                    Title = "Original Meeting",
                    StartTime = originalStartTime,
                    EndTime = originalEndTime,
                    Description = "Original Description",
                    Location = "Original Location",
                    IsAllDay = false
                };
                context.Appointments.Add(appointment);
                await context.SaveChangesAsync();
                appointmentId = appointment.Id;
            }

            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new AppointmentRepository(context);
                var appointment = await repository.GetByIdAsync(appointmentId);
                
                // Only update title and location
                appointment.Title = "Updated Meeting";
                appointment.Location = "Updated Location";
                
                var result = await repository.UpdateAsync(appointment);

                // Assert
                Assert.Equal(appointmentId, result.Id);
                Assert.Equal("Updated Meeting", result.Title);
                Assert.Equal("Updated Location", result.Location);
                Assert.Equal(originalStartTime, result.StartTime); // Should remain unchanged
                Assert.Equal(originalEndTime, result.EndTime); // Should remain unchanged
                Assert.Equal("Original Description", result.Description); // Should remain unchanged
                Assert.False(result.IsAllDay); // Should remain unchanged
            }

            // Verify it was updated in the database
            using (var context = CreateInMemoryContext(dbName))
            {
                var updatedAppointment = await context.Appointments.FindAsync(appointmentId);
                Assert.Equal("Updated Meeting", updatedAppointment.Title);
                Assert.Equal("Updated Location", updatedAppointment.Location);
                Assert.Equal(originalStartTime, updatedAppointment.StartTime);
                Assert.Equal(originalEndTime, updatedAppointment.EndTime);
                Assert.Equal("Original Description", updatedAppointment.Description);
                Assert.False(updatedAppointment.IsAllDay);
            }
        }
    }
}
