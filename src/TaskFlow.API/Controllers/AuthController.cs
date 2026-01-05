using AutoMapper;
using BCrypt.Net;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.API.Contracts.Auth;
using TaskFlow.API.Services;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;
    private readonly IValidator<RegisterUserRequest> _registerValidator;

    public AuthController(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IMapper mapper,
        IValidator<RegisterUserRequest> registerValidator)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
        _registerValidator = registerValidator;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var registerRequest = new RegisterUserRequest
        {
            FullName = request.FullName,
            Email = request.Email,
            Role = request.Role
        };

        await _registerValidator.ValidateAndThrowAsync(registerRequest, cancellationToken);

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            return BadRequest("Password must be at least 8 characters long.");
        }

        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            return Conflict($"A user with email '{request.Email}' already exists.");
        }

        var user = new User(Guid.NewGuid(), request.FullName, request.Email, request.Role);
        user.SetPasswordHash(BCrypt.Net.BCrypt.HashPassword(request.Password));

        await _userRepository.AddAsync(user, cancellationToken);

        var response = new AuthResponse
        {
            Token = _jwtTokenService.GenerateToken(user),
            User = _mapper.Map<UserDto>(user)
        };

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email and password are required.");
        }

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return Unauthorized("Invalid credentials.");
        }

        var validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!validPassword)
        {
            return Unauthorized("Invalid credentials.");
        }

        var response = new AuthResponse
        {
            Token = _jwtTokenService.GenerateToken(user),
            User = _mapper.Map<UserDto>(user)
        };

        return Ok(response);
    }
}
