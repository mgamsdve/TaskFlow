using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Application.Commands.Tasks;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Queries.Tasks;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

namespace TaskFlow.Tests.Unit.Services;

public sealed class TaskServiceTests
{
    [Fact]
    public async Task CreateTaskAsync_ShouldCreateTask_WhenCommandIsValid()
    {
        var cardRepositoryMock = new Mock<ICardRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var mapperMock = new Mock<IMapper>();
        var validatorMock = new Mock<IValidator<CreateTaskCommand>>();

        var command = new CreateTaskCommand(
            Guid.NewGuid(),
            "Implement drag and drop",
            "Enable moving cards between columns",
            TaskPriority.High,
            2,
            DateTime.UtcNow.AddDays(2));

        validatorMock
            .Setup(validator => validator.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        mapperMock
            .Setup(mapper => mapper.Map<CardDto>(It.IsAny<Card>()))
            .Returns((Card source) => new CardDto
            {
                Id = source.Id,
                ListId = source.ListId,
                Title = source.Title,
                Description = source.Description,
                Priority = source.Priority,
                Position = source.Position,
                DueDateUtc = source.DueDateUtc,
                CreatedAtUtc = source.CreatedAtUtc,
                UpdatedAtUtc = source.UpdatedAtUtc,
                AssignedUsers = [],
                Comments = [],
                Labels = []
            });

        var sut = new TaskService(cardRepositoryMock.Object, userRepositoryMock.Object, mapperMock.Object, validatorMock.Object);

        var result = await sut.CreateTaskAsync(command);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(command.ListId, result.ListId);
        Assert.Equal(command.Title, result.Title);
        Assert.Equal(command.Priority, result.Priority);

        cardRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<Card>(card =>
                    card.ListId == command.ListId
                    && card.Title == command.Title
                    && card.Priority == command.Priority
                    && card.Position == command.Position),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AssignUserToTaskAsync_ShouldAssignUserAndPersist_WhenTaskAndUserExist()
    {
        var cardRepositoryMock = new Mock<ICardRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var mapperMock = new Mock<IMapper>();
        var validatorMock = new Mock<IValidator<CreateTaskCommand>>();

        var card = new Card(Guid.NewGuid(), Guid.NewGuid(), "Prepare test cases");
        var user = new User(Guid.NewGuid(), "Task Assignee", "assignee@example.com", UserRole.Member);

        cardRepositoryMock
            .Setup(repository => repository.GetByIdAsync(card.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(card);

        userRepositoryMock
            .Setup(repository => repository.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var sut = new TaskService(cardRepositoryMock.Object, userRepositoryMock.Object, mapperMock.Object, validatorMock.Object);

        await sut.AssignUserToTaskAsync(new AssignUserToTaskCommand(card.Id, user.Id));

        Assert.Contains(card.AssignedUsers, assigned => assigned.Id == user.Id);

        cardRepositoryMock.Verify(
            repository => repository.UpdateAsync(card, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTasksByListAsync_ShouldReturnMappedTasks_WhenListHasCards()
    {
        var cardRepositoryMock = new Mock<ICardRepository>();
        var userRepositoryMock = new Mock<IUserRepository>();
        var mapperMock = new Mock<IMapper>();
        var validatorMock = new Mock<IValidator<CreateTaskCommand>>();

        var listId = Guid.NewGuid();

        IReadOnlyCollection<Card> cards =
        [
            new Card(Guid.NewGuid(), listId, "Task A", priority: TaskPriority.Medium, position: 0),
            new Card(Guid.NewGuid(), listId, "Task B", priority: TaskPriority.High, position: 1)
        ];

        var expected = new List<CardDto>
        {
            new() { Id = Guid.NewGuid(), ListId = listId, Title = "Task A", Priority = TaskPriority.Medium, Position = 0 },
            new() { Id = Guid.NewGuid(), ListId = listId, Title = "Task B", Priority = TaskPriority.High, Position = 1 }
        };

        cardRepositoryMock
            .Setup(repository => repository.GetByListIdAsync(listId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cards);

        mapperMock
            .Setup(mapper => mapper.Map<List<CardDto>>(cards))
            .Returns(expected);

        var sut = new TaskService(cardRepositoryMock.Object, userRepositoryMock.Object, mapperMock.Object, validatorMock.Object);

        var result = await sut.GetTasksByListAsync(new GetTasksByListQuery(listId));

        Assert.Equal(2, result.Count);
        Assert.Collection(
            result,
            first => Assert.Equal("Task A", first.Title),
            second => Assert.Equal("Task B", second.Title));

        cardRepositoryMock.Verify(
            repository => repository.GetByListIdAsync(listId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
