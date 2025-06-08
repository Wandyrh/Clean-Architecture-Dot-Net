using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Application.Common.Extensions;
using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        return user is null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task AddAsync(CreateUserDto dto)
    {
        var user = _mapper.Map<User>(dto);
        user.Password = HashHelper.GenerateSha256Hash(dto.Password);
        user.CreatedAt = DateTime.UtcNow;
        user.CreatedBy = Guid.NewGuid();
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateUserDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(dto.Id);
        if (user is null) return;

        _mapper.Map(dto, user);

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user is null) return;

        await _unitOfWork.Users.RemoveAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PagedResult<UserDto>> GetPagedAsync(int page, int pageSize)
    {
        var query = _unitOfWork.Users
            .GetAll()
            .AsNoTracking()
            .OrderBy(_ => _.FirstName)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider);

        return await query.ToPagedResultAsync(page, pageSize);
    }
}
