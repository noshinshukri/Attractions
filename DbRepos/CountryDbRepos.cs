using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using DbModels;
using DbContext;
using Models.DTO;

namespace DbRepos;

public class CountryDbRepos
{
    private ILogger<CountryDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public CountryDbRepos(ILogger<CountryDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

            public async Task<ResponsePageDto<ICountry>> ReadCountriesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        IQueryable<DbCountry> query = _dbContext.Country.AsNoTracking();
            if (flat)
    {
        // Create query without navigation properties
        query = _dbContext.Country.AsNoTracking();
    }
    else
    {
        // Create query with all navigation properties included
        query = _dbContext.Country.AsNoTracking()
            .Include(i => i.DbCities)
;
    }
        var ret = new ResponsePageDto<ICountry>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query.CountAsync(),
            PageItems = await query.ToListAsync<ICountry>(),
        };
        return ret;
    }


        public async Task<ResponseItemDto<ICountry>> ReadCountryAsync(Guid id, bool flat)
{
    IQueryable<DbCountry> query;
    
    if (flat)
    {
        // Create query without navigation properties
        query = _dbContext.Country.AsNoTracking();
    }
    else
    {
        // Create query with all navigation properties included
        query = _dbContext.Country.AsNoTracking()
            .Include(i => i.DbCities)
;
    }

    // Find the C by ID and return
    var C = await query.FirstOrDefaultAsync(f => f.CountryId == id);

    var ret = new ResponseItemDto<ICountry>()
    {
#if DEBUG
        ConnectionString = _dbContext.dbConnection,
#endif
        Item = C
    };
    return ret;
}

    public async Task<ICountry> DeleteCountryAsync(Guid id)
    {
        //Find the instance with matching id
        var query1 = _dbContext.Country
            .Where(i => i.CountryId == id);
        var item = await query1.FirstOrDefaultAsync<DbCountry>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.Country.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();
        return item;
    }
}
