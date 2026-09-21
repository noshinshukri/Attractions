using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using DbModels;
using DbContext;
using Models.DTO;

namespace DbRepos;

public class ReviewDbRepos
{
    private ILogger<ReviewDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public ReviewDbRepos(ILogger<ReviewDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }


    public async Task<ResponsePageDto<IReview>> ReadReviewsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        IQueryable<DbReview> query = _dbContext.Review.AsNoTracking();

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.Review.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.Review.AsNoTracking()
                .Include(i => i.Attraction)
                .Include(i => i.User)
                .Include(i => i.Comment)
                .Include(i => i.Rating);
        }
        var ret = new ResponsePageDto<IReview>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query.CountAsync(),
            PageItems = await query.ToListAsync<IReview>(),
        };
        return ret;
    }

    public async Task<ResponseItemDto<IReview>> ReadReviewAsync(Guid id, bool flat)
    {
        IQueryable<DbReview> query;

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.Review.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.Review.AsNoTracking()
                .Include(i => i.Attraction)
                .Include(i => i.User)
                .Include(i => i.Comment)
                .Include(i => i.Rating);
        }

        // Find the C by ID and return
        var C = await query.FirstOrDefaultAsync(f => f.ReviewId == id);

        var ret = new ResponseItemDto<IReview>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = C
        };
        return ret;
    }

    public async Task<IReview> DeleteReviewAsync(Guid id)
    {
        //Find the instance with matching id
        var query1 = _dbContext.Review
            .Where(i => i.ReviewId == id);
        var item = await query1.FirstOrDefaultAsync<DbReview>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.Review.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();
        return item;
    }
}
