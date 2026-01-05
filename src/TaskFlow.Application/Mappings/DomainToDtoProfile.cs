using AutoMapper;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;
using TaskFlowList = TaskFlow.Domain.Entities.List;

namespace TaskFlow.Application.Mappings;

public sealed class DomainToDtoProfile : Profile
{
    public DomainToDtoProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<Comment, CommentDto>()
            .ForMember(
                destination => destination.AuthorName,
                config => config.MapFrom(source => source.Author == null ? string.Empty : source.Author.FullName));

        CreateMap<Card, CardDto>()
            .ForMember(
                destination => destination.Labels,
                config => config.MapFrom(source => source.Labels.Select(label => label.Name)));

        CreateMap<TaskFlowList, ListDto>();
        CreateMap<Board, BoardDto>();
        CreateMap<Project, ProjectDto>();
    }
}
