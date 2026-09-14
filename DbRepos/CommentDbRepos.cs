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

                public async Task<ResponsePageDto<IComment>> ReadCommentsAsync()
    {
        IQueryable<DbComment> query = _dbContext.Comment.AsNoTracking();
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
}
