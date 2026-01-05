using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Tests.Unit.Services;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenEmailDoesNotExist()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var mapperMock = new Mock<IMapper>();
        var validatorMock = new Mock<IValidator<RegisterUserRequest>>();

        var request = new RegisterUserRequest
        {
            FullName = "Jane Doe",
            Email = "jane.doe@example.com",
            Role = UserRole.Member
        };

        validatorMock
            .Setup(validator => validator.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        mapperMock
            .Setup(mapper => mapper.Map<UserDto>(It.IsAny<User>()))
            .Returns((User source) => new UserDto
            {
                Id = source.Id,
                FullName = source.FullName,
                Email = source.Email,
                Role = source.Role,
                CreatedAtUtc = source.CreatedAtUtc,
                UpdatedAtUtc = source.UpdatedAtUtc
            });

        var sut = new AuthService(userRepositoryMock.Object, mapperMock.Object, validatorMock.Object);

        var result = await sut.RegisterAsync(request);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(request.FullName, result.FullName);
        Assert.Equal(request.Email.ToLowerInvariant(), result.Email);
        Assert.Equal(request.Role, result.Role);

        userRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<User>(user => user.Email == request.Email.ToLowerInvariant() && user.FullName == request.FullName),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowInvalidOperationException_WhenEmailAlreadyExists()
    {
        var userRepositoryMock = new Mock<IUserRepository>();
        var mapperMock = new Mock<IMapper>();
        var validatorMock = new Mock<IValidator<RegisterUserRequest>>();

        var request = new RegisterUserRequest
        {
            FullName = "Jane Doe",
            Email = "jane.doe@example.com",
            Role = UserRole.Member
        };

        validatorMock
            .Setup(validator => validator.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User(Guid.NewGuid(), "Existing User", request.Email, UserRole.Member));

        var sut = new AuthService(userRepositoryMock.Object, mapperMock.Object, validatorMock.Object);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.RegisterAsync(request));

        Assert.Contains("already exists", exception.Message, StringComparison.OrdinalIgnoreCase);

        userRepositoryMock.Verify(
            repository => repository.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
