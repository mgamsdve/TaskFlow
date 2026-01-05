using AutoMapper;
using TaskFlow.Application.Abstractions.Persistence;
using TaskFlow.Application.Abstractions.Services;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Queries.Boards;

namespace TaskFlow.Application.Services;

public sealed class BoardService : IBoardService
{
    private readonly IBoardRepository _boardRepository;
    private readonly IMapper _mapper;

    public BoardService(IBoardRepository boardRepository, IMapper mapper)
    {
        _boardRepository = boardRepository;
        _mapper = mapper;
    }

    public async Task<BoardDto?> GetBoardAsync(GetBoardQuery query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        if (query.BoardId == Guid.Empty)
        {
            throw new ArgumentException("Board id cannot be empty.", nameof(query));
        }

        var board = await _boardRepository.GetByIdAsync(query.BoardId, cancellationToken);
        return board is null ? null : _mapper.Map<BoardDto>(board);
    }
}
