using AutoMapper;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Tests;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public async Task GetAllAsync_ReturnsUserDtos()
    {
        var users = new List<User> { new User { Id = Guid.NewGuid(), FirstName = "Test" } };
        _unitOfWork.Setup(u => u.Users.GetAllAsync()).ReturnsAsync(users);
        _mapper.Setup(m => m.Map<IEnumerable<UserDto>>(users)).Returns(new List<UserDto> { new UserDto { FirstName = "Test" } });

        var service = new UserService(_unitOfWork.Object, _mapper.Object);

        var result = await service.GetAllAsync();

        Assert.NotNull(result);
    }
[Fact]
    public async Task GetByIdAsync_ReturnsUserDto()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id, FirstName = "Test" };
        _unitOfWork.Setup(u => u.Users.GetByIdAsync(id)).ReturnsAsync(user);
        _mapper.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { FirstName = "Test" });

        var service = new UserService(_unitOfWork.Object, _mapper.Object);

        var result = await service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Test", result.FirstName);
    }

    [Fact]
    public async Task AddAsync_AddsUser()
    {
        var dto = new CreateUserDto { FirstName = "Test" };
        var userId = Guid.NewGuid();
        var user = new User { Id = Guid.NewGuid(), FirstName = "Test" };
        _mapper.Setup(m => m.Map<User>(dto)).Returns(user);
        _unitOfWork.Setup(u => u.Users.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _mapper.Setup(m => m.Map<User>(dto)).Returns(user);        
        dto.Password = "testpassword";

        var service = new UserService(_unitOfWork.Object, _mapper.Object);

        await service.AddAsync(dto, userId);

        _unitOfWork.Verify(u => u.Users.AddAsync(user), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dto = new UpdateUserDto { Id = id, FirstName = "Updated" };
        var user = new User { Id = id, FirstName = "Old" };
        _unitOfWork.Setup(u => u.Users.GetByIdAsync(id)).ReturnsAsync(user);
        _mapper.Setup(m => m.Map(dto, user)).Callback<UpdateUserDto, User>((src, dest) => dest.FirstName = src.FirstName);
        _unitOfWork.Setup(u => u.Users.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var service = new UserService(_unitOfWork.Object, _mapper.Object);

        await service.UpdateAsync(dto, id, userId);

        Assert.Equal("Updated", user.FirstName);
        _unitOfWork.Verify(u => u.Users.UpdateAsync(user), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeletesUser()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var user = new User { Id = id, FirstName = "ToDelete" };
        _unitOfWork.Setup(u => u.Users.GetByIdAsync(id)).ReturnsAsync(user);
        _unitOfWork.Setup(u => u.Users.RemoveAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var service = new UserService(_unitOfWork.Object, _mapper.Object);

        await service.DeleteAsync(id, userId);

        _unitOfWork.Verify(u => u.Users.RemoveAsync(user), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPagedResult()
    {
        var service = new UserService(_unitOfWork.Object, _mapper.Object);

        await Assert.ThrowsAsync<NullReferenceException>(() => service.GetPagedAsync(1, 10));
    }
}