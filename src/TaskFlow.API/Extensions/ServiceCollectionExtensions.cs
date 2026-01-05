using AutoMapper;
using FluentValidation;
using TaskFlow.API.Services;
using TaskFlow.Application.Abstractions.Services;
using TaskFlow.Application.Commands.Projects;
using TaskFlow.Application.Commands.Tasks;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Mappings;
using TaskFlow.Application.Services;
using TaskFlow.Application.Validators;

namespace TaskFlow.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(DomainToDtoProfile).Assembly);

        services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserValidator>();
        services.AddScoped<IValidator<CreateProjectCommand>, CreateProjectValidator>();
        services.AddScoped<IValidator<CreateTaskCommand>, CreateTaskValidator>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<ITaskService, TaskService>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
