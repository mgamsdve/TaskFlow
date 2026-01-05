using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Application.Commands.Projects;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Tests.Unit.Services;

public sealed class ProjectServiceTests
{
    [Fact]
    public async Task CreateProjectAsync_ShouldCreateProject_WhenCommandIsValid()
    {
        var projectRepositoryMock = new Mock<IProjectRepository>();
        var mapperMock = new Mock<IMapper>();
        var validatorMock = new Mock<IValidator<CreateProjectCommand>>();

        var command = new CreateProjectCommand(
            Guid.NewGuid(),
            "TaskFlow",
            "TaskFlow planning project");

        validatorMock
            .Setup(validator => validator.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        mapperMock
            .Setup(mapper => mapper.Map<ProjectDto>(It.IsAny<Project>()))
            .Returns((Project source) => new ProjectDto
            {
                Id = source.Id,
                OrganizationId = source.OrganizationId,
                Name = source.Name,
                Description = source.Description,
                CreatedAtUtc = source.CreatedAtUtc,
                UpdatedAtUtc = source.UpdatedAtUtc,
                Boards = []
            });

        var sut = new ProjectService(projectRepositoryMock.Object, mapperMock.Object, validatorMock.Object);

        var result = await sut.CreateProjectAsync(command);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(command.OrganizationId, result.OrganizationId);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.Description, result.Description);

        projectRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<Project>(project =>
                    project.OrganizationId == command.OrganizationId
                    && project.Name == command.Name
                    && project.Description == command.Description),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
