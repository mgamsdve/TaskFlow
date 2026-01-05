using AutoMapper;
using FluentValidation;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Application.Abstractions.Services;
using TaskFlow.Application.Commands.Tasks;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Queries.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services;

public sealed class TaskService : ITaskService
{
    private readonly ICardRepository _cardRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateTaskCommand> _createTaskValidator;

    public TaskService(
        ICardRepository cardRepository,
        IUserRepository userRepository,
        IMapper mapper,
        IValidator<CreateTaskCommand> createTaskValidator)
    {
        _cardRepository = cardRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _createTaskValidator = createTaskValidator;
    }

    public async Task<CardDto> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        await _createTaskValidator.ValidateAndThrowAsync(command, cancellationToken);

        var card = new Card(
            Guid.NewGuid(),
            command.ListId,
            command.Title,
            command.Description,
            command.Priority,
            command.Position);

        card.ScheduleDueDate(command.DueDateUtc);

        await _cardRepository.AddAsync(card, cancellationToken);
        return _mapper.Map<CardDto>(card);
    }

    public async Task AssignUserToTaskAsync(AssignUserToTaskCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (command.CardId == Guid.Empty)
        {
            throw new ArgumentException("Card id cannot be empty.", nameof(command));
        }

        if (command.UserId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(command));
        }

        var card = await _cardRepository.GetByIdAsync(command.CardId, cancellationToken)
                   ?? throw new KeyNotFoundException($"Card '{command.CardId}' was not found.");

        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken)
                   ?? throw new KeyNotFoundException($"User '{command.UserId}' was not found.");

        card.AssignUser(user);
        await _cardRepository.UpdateAsync(card, cancellationToken);
    }

    public async Task<IReadOnlyCollection<CardDto>> GetTasksByListAsync(GetTasksByListQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (query.ListId == Guid.Empty)
        {
            throw new ArgumentException("List id cannot be empty.", nameof(query));
        }

        var cards = await _cardRepository.GetByListIdAsync(query.ListId, cancellationToken);
        return _mapper.Map<List<CardDto>>(cards);
    }
}
