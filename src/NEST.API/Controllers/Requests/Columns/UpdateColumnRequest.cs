namespace NEST.API.Controllers.Requests.Columns;

public class UpdateColumnRequest
{
    public required string Name { get; set; }
    public int Position { get; set; }
}