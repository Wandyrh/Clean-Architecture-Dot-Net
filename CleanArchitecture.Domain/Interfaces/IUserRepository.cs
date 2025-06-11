using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User> GetUserByEmailAsync(string email);
}
