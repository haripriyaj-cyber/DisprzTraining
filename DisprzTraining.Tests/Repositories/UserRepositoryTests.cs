using System;
using System.Threading.Tasks;
using DisprzTraining.DataAccess;
using DisprzTraining.DataAccess.Repositories;
using DisprzTraining.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DisprzTraining.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private AppDbContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
                
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var userId = 0;
            
            using (var context = CreateInMemoryContext(dbName))
            {
                var user = new User { 
                    Username = "testuser",
                    Password = "password123",
                    Email = "test@example.com",
                    FullName = "Test User"
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();
                userId = user.Id;
            }

            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new UserRepository(context);
                var user = await repository.GetByIdAsync(userId);

                // Assert
                Assert.NotNull(user);
                Assert.Equal("testuser", user.Username);
                Assert.Equal("test@example.com", user.Email);
            }
        }

        [Fact]
        public async Task GetByUsernameAsync_WithValidUsername_ShouldReturnUser()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var username = "testuser";
            
            using (var context = CreateInMemoryContext(dbName))
            {
                var user = new User { 
                    Username = username,
                    Password = "password123",
                    Email = "test@example.com",
                    FullName = "Test User"
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new UserRepository(context);
                var user = await repository.GetByUsernameAsync(username);

                // Assert
                Assert.NotNull(user);
                Assert.Equal(username, user.Username);
                Assert.Equal("test@example.com", user.Email);
            }
        }

        [Fact]
        public async Task CreateAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var user = new User 
            { 
                Username = "newuser",
                Password = "newpassword",
                Email = "new@example.com",
                FullName = "New User"
            };

            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new UserRepository(context);
                var result = await repository.CreateAsync(user);

                // Assert
                Assert.NotEqual(0, result.Id); // ID should be set
                Assert.Equal(user.Username, result.Username);
            }

            // Verify it was saved to the database
            using (var context = CreateInMemoryContext(dbName))
            {
                Assert.Equal(1, await context.Users.CountAsync());
                var savedUser = await context.Users.FirstAsync();
                Assert.Equal("newuser", savedUser.Username);
                Assert.Equal("new@example.com", savedUser.Email);
            }
        }

        [Fact]
        public async Task UsernameExistsAsync_WithExistingUsername_ShouldReturnTrue()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var username = "existinguser";
            
            using (var context = CreateInMemoryContext(dbName))
            {
                var user = new User { 
                    Username = username,
                    Password = "password123",
                    Email = "existing@example.com",
                    FullName = "Existing User"
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new UserRepository(context);
                var exists = await repository.UsernameExistsAsync(username);

                // Assert
                Assert.True(exists);
            }
        }

        [Fact]
        public async Task UsernameExistsAsync_WithNonExistingUsername_ShouldReturnFalse()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var username = "nonexistinguser";
            
            // Act
            using (var context = CreateInMemoryContext(dbName))
            {
                var repository = new UserRepository(context);
                var exists = await repository.UsernameExistsAsync(username);

                // Assert
                Assert.False(exists);
            }
        }
    }
}