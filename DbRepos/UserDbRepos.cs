using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using DbModels;
using DbContext;
using Models.DTO;

namespace DbRepos;

public class UserDbRepos
{
    private ILogger<UserDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public UserDbRepos(ILogger<UserDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }


    public async Task<ResponsePageDto<IUser>> ReadUsersAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<DbUser> query;

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.User.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.User.AsNoTracking()
                .Include(i => i.DbReviews);
        }
        var ret = new ResponsePageDto<IUser>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.UserName.ToLower().Contains(filter) ||
                            i.Email.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.UserName.ToLower().Contains(filter) ||
                            i.Email.ToLower().Contains(filter)))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IUser>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }

    public async Task<ResponseItemDto<IUser>> ReadUserAsync(Guid id, bool flat)
    {
        IQueryable<DbUser> query;

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.User.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.User.AsNoTracking()
                .Include(i => i.DbReviews);
        }

        // Find the C by ID and return
        var C = await query.FirstOrDefaultAsync(f => f.UserId == id);

        var ret = new ResponseItemDto<IUser>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = C
        };
        return ret;
    }

    public async Task<IUser> DeleteUserAsync(Guid id)
    {
        //Find the instance with matching id
        var query1 = _dbContext.User
            .Where(i => i.UserId == id);
        var item = await query1.FirstOrDefaultAsync<DbUser>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.User.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();
        return item;
    }
}
