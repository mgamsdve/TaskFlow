using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.API.Contracts.Boards;
using TaskFlow.Application.Abstractions.Services;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Queries.Boards;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("boards")]
public sealed class BoardsController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IBoardService _boardService;
    private readonly IMapper _mapper;

    public BoardsController(AppDbContext dbContext, IBoardService boardService, IMapper mapper)
    {
        _dbContext = dbContext;
        _boardService = boardService;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<ActionResult<BoardDto>> CreateBoard([FromBody] CreateBoardRequest request, CancellationToken cancellationToken)
    {
        var projectExists = await _dbContext.Projects
            .AsNoTracking()
            .AnyAsync(project => project.Id == request.ProjectId, cancellationToken);

        if (!projectExists)
        {
            return NotFound("Project not found.");
        }

        var board = new Board(Guid.NewGuid(), request.ProjectId, request.Name, request.Position);
        await _dbContext.Boards.AddAsync(board, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<BoardDto>(board);
        return CreatedAtAction(nameof(GetBoardById), new { id = dto.Id }, dto);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BoardDto>> GetBoardById(Guid id, CancellationToken cancellationToken)
    {
        var board = await _boardService.GetBoardAsync(new GetBoardQuery(id), cancellationToken);
        if (board is null)
        {
            return NotFound();
        }

        return Ok(board);
    }
}
