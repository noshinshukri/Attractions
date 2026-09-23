using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class CityDbRepos
{
    private ILogger<CityDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public CityDbRepos(ILogger<CityDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponsePageDto<ICity>> ReadCitiesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<DbCity> query;

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.City.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.City.AsNoTracking()
                .Include(i => i.DbCountry);
        }
        var ret = new ResponsePageDto<ICity>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.CityName.ToLower().Contains(filter) ||
                            i.DbCountry.CountryName.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.CityName.ToLower().Contains(filter) ||
                            i.DbCountry.CountryName.ToLower().Contains(filter)))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<ICity>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }

    public async Task<ResponseItemDto<ICity>> ReadCityAsync(Guid id, bool flat)
    {
        IQueryable<DbCity> query;

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.City.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.City.AsNoTracking()
                .Include(i => i.DbCountry)
    ;
        }

        // Find the C by ID and return
        var C = await query.FirstOrDefaultAsync(f => f.CityId == id);

        var ret = new ResponseItemDto<ICity>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = C
        };
        return ret;
    }

    public async Task<ICity> DeleteCityAsync(Guid id)
    {
        //Find the instance with matching id
        var query1 = _dbContext.City
            .Where(i => i.CityId == id);
        var item = await query1.FirstOrDefaultAsync<DbCity>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.City.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();
        return item;
    }
}
