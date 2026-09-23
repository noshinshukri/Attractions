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

    public async Task<ResponseItemDto<IUser>> CreateUserAsync(UserCuDto itemDto)
    {
        // 1. Validate that UserId is null
        if (itemDto.UserId != null)
            throw new ArgumentException($"{nameof(itemDto.UserId)} must be null when creating a new object");

        // 2. Create new database entity from DTO
        var item = new DbUser(itemDto);

        // 3. Update navigation properties
        await navProp_UserCUdto_to_UserDbM(itemDto, item);

        // 4. Add to context and save
        _dbContext.User.Add(item);
        await _dbContext.SaveChangesAsync();

        // 5. Return fully populated item
        return await ReadUserAsync(item.UserId, false);
    }

    private async Task navProp_UserCUdto_to_UserDbM(UserCuDto itemDtoSrc, DbUser itemDst)
    {
        // Multiple relationships (Pets)
        if (itemDtoSrc.ReviewsId != null)
        {
            var reviews = new List<DbReview>();
            foreach (var id in itemDtoSrc.ReviewsId)
            {
                var p = await _dbContext.Review.FirstOrDefaultAsync(i => i.ReviewId == id);
                if (p == null) throw new ArgumentException($"Review id {id} not existing");
                reviews.Add(p);
            }
            itemDst.DbReviews = reviews;
        }

        // Multiple relationships (Quotes) - similar pattern
    }

    public async Task<ResponseItemDto<IUser>> UpdateUserAsync(UserCuDto itemDto)
    {
        //Find the instance with matching id and read the navigation properties.
        var query1 = _dbContext.User
            .Where(i => i.UserId == itemDto.UserId);
        var item = await query1
            .Include(i => i.DbReviews)
            .FirstOrDefaultAsync<DbUser>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {itemDto.UserId} is not existing");

        //transfer any changes from DTO to database objects
        //Update individual properties
        item.UpdateFromDTO(itemDto);

        //Update navigation properties
        await navProp_UserCUdto_to_UserDbM(itemDto, item);

        //write to database model
        _dbContext.User.Update(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadUserAsync(item.UserId, false);
    }
}
