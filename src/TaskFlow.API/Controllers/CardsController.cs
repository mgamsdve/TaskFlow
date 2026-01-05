using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Contracts.Cards;
using TaskFlow.Application.Abstractions.Services;
using TaskFlow.Application.Commands.Tasks;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("cards")]
public sealed class CardsController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public CardsController(ITaskService taskService, AppDbContext dbContext, IMapper mapper)
    {
        _taskService = taskService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<CardDto>> CreateCard([FromBody] CreateCardRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTaskCommand(
            request.ListId,
            request.Title,
            request.Description,
            request.Priority,
            request.Position,
            request.DueDateUtc);

        var created = await _taskService.CreateTaskAsync(command, cancellationToken);
        return Created($"/cards/{created.Id}", created);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<CardDto>> UpdateCard(Guid id, [FromBody] UpdateCardRequest request, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid card id.");
        }

        var card = await _dbContext.Cards
            .Include(entity => entity.Comments)
                .ThenInclude(comment => comment.Author)
            .Include(entity => entity.Labels)
            .Include(entity => entity.AssignedUsers)
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);

        if (card is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            card.Rename(request.Title);
        }

        if (request.ClearDescription)
        {
            card.UpdateDescription(null);
        }
        else if (request.Description is not null)
        {
            card.UpdateDescription(request.Description);
        }

        if (request.Priority.HasValue)
        {
            card.ChangePriority(request.Priority.Value);
        }

        if (request.ClearDueDate)
        {
            card.ScheduleDueDate(null);
        }
        else if (request.DueDateUtc.HasValue)
        {
            card.ScheduleDueDate(request.DueDateUtc.Value);
        }

        if (request.ListId.HasValue)
        {
            var list = await _dbContext.Lists.FirstOrDefaultAsync(entity => entity.Id == request.ListId.Value, cancellationToken);
            if (list is null)
            {
                return NotFound("List not found.");
            }

            card.MoveToList(list, request.Position ?? card.Position);
        }
        else if (request.Position.HasValue)
        {
            card.Reposition(request.Position.Value);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var updated = await _dbContext.Cards
            .AsNoTracking()
            .Include(entity => entity.Comments)
                .ThenInclude(comment => comment.Author)
            .Include(entity => entity.Labels)
            .Include(entity => entity.AssignedUsers)
            .FirstAsync(entity => entity.Id == id, cancellationToken);

        return Ok(_mapper.Map<CardDto>(updated));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<IActionResult> DeleteCard(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid card id.");
        }

        var card = await _dbContext.Cards.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
        if (card is null)
        {
            return NotFound();
        }

        _dbContext.Cards.Remove(card);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/assign")]
    public async Task<IActionResult> AssignUser(Guid id, [FromBody] AssignUserRequest request, CancellationToken cancellationToken)
    {
        var command = new AssignUserToTaskCommand(id, request.UserId);
        await _taskService.AssignUserToTaskAsync(command, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/comment")]
    public async Task<ActionResult<CommentDto>> AddComment(Guid id, [FromBody] AddCommentRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest("Comment content is required.");
        }

        var card = await _dbContext.Cards
            .Include(entity => entity.Comments)
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);

        if (card is null)
        {
            return NotFound("Card not found.");
        }

        var author = await _dbContext.Users.FirstOrDefaultAsync(entity => entity.Id == request.AuthorId, cancellationToken);
        if (author is null)
        {
            return NotFound("User not found.");
        }

        var comment = new Comment(Guid.NewGuid(), card.Id, author, request.Content);
        card.AddComment(comment);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(_mapper.Map<CommentDto>(comment));
    }
}
