using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Columns;

public class UpdateColumnCommandHandler : IRequestHandler<UpdateColumnCommand, bool>
{
    private readonly IApplicationDbContext _context;
    
    public UpdateColumnCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    // Обрабатываем команду обновления колонки
    public async Task<bool> Handle(
        UpdateColumnCommand request,
        CancellationToken cancellationToken)
    {
        var column = await _context.Columns
            .FirstOrDefaultAsync(c => c.Id == request.ColumnId,
                cancellationToken);
        
        if (column is null)
        {
            return false;
        }
        
        column.Name = request.Name;
        column.Position = request.Position;
        column.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}