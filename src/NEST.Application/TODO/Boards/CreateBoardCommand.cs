using MediatR;

namespace NEST.Application.TODO.Boards;

public class CreateBoardCommand : IRequest<Guid>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
}