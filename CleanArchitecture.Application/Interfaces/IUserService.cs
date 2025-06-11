using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;

namespace CleanArchitecture.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(Guid id);
    Task AddAsync(CreateUserDto user, Guid userId);
    Task UpdateAsync(UpdateUserDto user, Guid id, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<PagedResult<UserDto>> GetPagedAsync(int page, int pageSize);
}
