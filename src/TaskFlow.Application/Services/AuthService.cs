using AutoMapper;
using FluentValidation;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Application.Abstractions.Services;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<RegisterUserRequest> _registerUserValidator;

    public AuthService(
        IUserRepository userRepository,
        IMapper mapper,
        IValidator<RegisterUserRequest> registerUserValidator)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _registerUserValidator = registerUserValidator;
    }

    public async Task<UserDto> RegisterAsync(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await _registerUserValidator.ValidateAndThrowAsync(request, cancellationToken);

        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new InvalidOperationException($"A user with email '{request.Email}' already exists.");
        }

        var user = new User(Guid.NewGuid(), request.FullName, request.Email, request.Role);
        await _userRepository.AddAsync(user, cancellationToken);

        return _mapper.Map<UserDto>(user);
    }
}
