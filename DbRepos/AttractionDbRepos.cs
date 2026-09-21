using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class AttractionDbRepos
{
    private ILogger<AttractionDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public AttractionDbRepos(ILogger<AttractionDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponsePageDto<IAttraction>> ReadAttractionsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        IQueryable<DbAttraction> query = _dbContext.Attraction.AsNoTracking();

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.Attraction.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.Attraction.AsNoTracking()
                .Include(i => i.Reviews)
                .Include(i => i.Address)
                .Include(i => i.Categories);
        }
        var ret = new ResponsePageDto<IAttraction>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query.CountAsync(),
            PageItems = await query.ToListAsync<IAttraction>(),
        };
        return ret;
    }

    public async Task<ResponseItemDto<IAttraction>> ReadAttractionAsync(Guid id, bool flat)
    {
        IQueryable<DbAttraction> query;

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.Attraction.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.Attraction.AsNoTracking()
                .Include(i => i.Reviews)
                .Include(i => i.Address)
                .Include(i => i.Categories);
        }

        // Find the C by ID and return
        var C = await query.FirstOrDefaultAsync(f => f.AttractionId == id);

        var ret = new ResponseItemDto<IAttraction>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = C
        };
        return ret;
    }

    public async Task<IAttraction> DeleteAttractionAsync(Guid id)
    {
        //Find the instance with matching id
        var query1 = _dbContext.Attraction
            .Where(i => i.AttractionId == id);
        var item = await query1.FirstOrDefaultAsync<DbAttraction>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.Attraction.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();
        return item;
    }
}
