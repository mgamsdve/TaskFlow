using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.DTOs;
using TaskFlow.Infrastructure.Persistence.Context;

namespace TaskFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("users")]
public sealed class UsersController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public UsersController(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<ActionResult<IReadOnlyCollection<UserDto>>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.FullName)
            .ToListAsync(cancellationToken);

        return Ok(_mapper.Map<List<UserDto>>(users));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid user id.");
        }

        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<UserDto>(user));
    }
}
