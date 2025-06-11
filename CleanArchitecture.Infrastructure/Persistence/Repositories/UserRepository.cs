using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{    

    public UserRepository(AppDbContext appDbContext) : base(appDbContext)
    {        
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
       return await _dbSet.Where(_ => _.Email.ToLower() == email.ToLower()).FirstAsync();
    }
}
