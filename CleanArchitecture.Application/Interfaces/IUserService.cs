using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;

namespace CleanArchitecture.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(Guid id);
    Task AddAsync(CreateUserDto product);
    Task UpdateAsync(UpdateUserDto product);
    Task DeleteAsync(Guid id);
    Task<PagedResult<UserDto>> GetPagedAsync(int page, int pageSize);
}
