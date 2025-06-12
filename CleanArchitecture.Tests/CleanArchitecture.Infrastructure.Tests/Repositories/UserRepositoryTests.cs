using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Persistence.Repositories;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Tests.CleanArchitecture.Infrastructure.Tests.Repositories;

public class UserRepositoryTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsUser_WhenEmailExists()
    {          
        var context = GetInMemoryDbContext();
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            Phone = "1234567890",
            Password = "password"
        };
        context.Set<User>().Add(user);
        context.SaveChanges();

        var repository = new UserRepository(context);
     
        var result = await repository.GetUserByEmailAsync("test@example.com");
     
        Assert.NotNull(result);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetUserByEmailAsync_Throws_WhenEmailDoesNotExist()
    {       
        var context = GetInMemoryDbContext();
        var repository = new UserRepository(context);
     
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await repository.GetUserByEmailAsync("notfound@example.com");
        });
    }
}