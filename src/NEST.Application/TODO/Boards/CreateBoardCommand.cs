namespace NEST.Application.TODO.Boards;

public class CreateBoardCommand
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid UserId { get; set; }
}