using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Contracts.Lists;
using TaskFlow.Application.DTOs;
using TaskFlow.Infrastructure.Persistence.Context;
using TaskListEntity = TaskFlow.Domain.Entities.List;

namespace TaskFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("lists")]
public sealed class ListsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public ListsController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<ActionResult<ListDto>> CreateList([FromBody] CreateListRequest request, CancellationToken cancellationToken)
    {
        var boardExists = await _dbContext.Boards
            .AsNoTracking()
            .AnyAsync(board => board.Id == request.BoardId, cancellationToken);

        if (!boardExists)
        {
            return NotFound("Board not found.");
        }

        var list = new TaskListEntity(Guid.NewGuid(), request.BoardId, request.Name, request.Position);
        await _dbContext.Lists.AddAsync(list, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Ok(_mapper.Map<ListDto>(list));
    }
}
