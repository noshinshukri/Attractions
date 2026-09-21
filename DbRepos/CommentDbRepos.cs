using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class CommentDbRepos
{
    private ILogger<CommentDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public CommentDbRepos(ILogger<CommentDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponsePageDto<IComment>> ReadCommentsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        IQueryable<DbComment> query;

        if (flat)
        {
            query = _dbContext.Comment.AsNoTracking();
        }
        else
        {
            query = _dbContext.Comment.AsNoTracking()
                .Include(i => i.DbReview)
                .ThenInclude(r => r.DbAttraction);
        }

        var ret = new ResponsePageDto<IComment>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query.CountAsync(),
            PageItems = await query.ToListAsync<IComment>(),
        };
        return ret;
    }

                public async Task<ResponseItemDto<IComment>> ReadCommentAsync(Guid id, bool flat)
{
    IQueryable<DbComment> query;
    
    if (flat)
    {
        // Create query without navigation properties
        query = _dbContext.Comment.AsNoTracking();
    }
    else
    {
        // Create query with all navigation properties included
        query = _dbContext.Comment.AsNoTracking()
            .Include(i => i.DbReview)
            .ThenInclude(r => r.DbAttraction);
    }

    // Find the C by ID and return
    var C = await query.FirstOrDefaultAsync(f => f.CommentId == id);

    var ret = new ResponseItemDto<IComment>()
    {
#if DEBUG
        ConnectionString = _dbContext.dbConnection,
#endif
        Item = C
    };
    return ret;
}

    public async Task<IComment> DeleteCommentAsync(Guid id)
    {
        //Find the instance with matching id
        var query1 = _dbContext.Comment
            .Where(i => i.CommentId == id);
        var item = await query1.FirstOrDefaultAsync<DbComment>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.Comment.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();
        return item;
    }
}
