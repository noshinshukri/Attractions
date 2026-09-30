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

    #region Read all items
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
                .Include(i => i.DbReviews)
                .ThenInclude(i => i.DbComment)
                .Include(i => i.DbReviews)
                .ThenInclude(i => i.DbAttraction);
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
    #endregion

    #region Read one item
    public async Task<ResponseItemDto<IUser>> ReadUserAsync(Guid id, bool flat)
    {
        IQueryable<DbUser> query;

        if (flat)
        {

            query = _dbContext.User.AsNoTracking();
        }
        else
        {

            query = _dbContext.User.AsNoTracking()
                .Include(i => i.DbReviews)
                .ThenInclude(i => i.DbComment)
                .Include(i => i.DbReviews)
                .ThenInclude(i => i.DbAttraction);
        }


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
    #endregion

    #region Delete item
    public async Task<ResponseItemDto<IUser>> DeleteUserAsync(Guid id)
    {
        var item = await _dbContext.User
            .Include(u => u.DbReviews)
                .ThenInclude(r => r.DbComment)
            .FirstOrDefaultAsync(u => u.UserId == id);

        if (item == null)
            throw new ArgumentException($"User id {id} not existing");

        var comments = item.DbReviews
            .Where(r => r.DbComment != null)
            .Select(r => r.DbComment)
            .ToList();

        _dbContext.Comment.RemoveRange(comments);
        _dbContext.Review.RemoveRange(item.DbReviews);
        _dbContext.User.Remove(item);

        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IUser> { Item = item };
    }
    #endregion

    #region Create item
    public async Task<ResponseItemDto<IUser>> CreateUserAsync(UserCuDto itemDto)
    {

        if (itemDto.UserId != null)
            throw new ArgumentException($"{nameof(itemDto.UserId)} must be null when creating a new object");


        var item = new DbUser(itemDto);


        _dbContext.User.Add(item);
        await _dbContext.SaveChangesAsync();


        return await ReadUserAsync(item.UserId, false);
    }
    #endregion

    #region Navigation Properties CuDto
    private async Task navProp_UserCUdto_to_UserDbM(UserCuDto itemDtoSrc, DbUser itemDst)
    {
        // Multiple relationships
    }
    #endregion

    #region Update item
    public async Task<ResponseItemDto<IUser>> UpdateUserAsync(UserCuDto itemDto)
    {

        var item = await _dbContext.User
            .Where(i => i.UserId == itemDto.UserId)
            .FirstOrDefaultAsync<DbUser>();


        if (item == null) throw new ArgumentException($"Item {itemDto.UserId} is not existing");


        item.UpdateFromDTO(itemDto);

        _dbContext.User.Update(item);

        await _dbContext.SaveChangesAsync();


        return await ReadUserAsync(item.UserId, false);
    }
    #endregion
}
