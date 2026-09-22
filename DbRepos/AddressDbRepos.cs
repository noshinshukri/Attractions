using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class AddressDbRepos
{
    private ILogger<AddressDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public AddressDbRepos(ILogger<AddressDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponsePageDto<IAddress>> ReadAdressesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<DbAddress> query;

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.Address.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.Address.AsNoTracking()
                .Include(i => i.DbCity)
                .Include(i => i.DbCountry);
        }
        var ret = new ResponsePageDto<IAddress>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.City.CityName.ToLower().Contains(filter) ||
                            i.Country.CountryName.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.City.CityName.ToLower().Contains(filter) ||
                            i.Country.CountryName.ToLower().Contains(filter)))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IAddress>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }

    public async Task<ResponseItemDto<IAddress>> ReadAddressAsync(Guid id, bool flat)
    {
        IQueryable<DbAddress> query;

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.Address.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.Address.AsNoTracking()
                .Include(i => i.DbCity)
                .Include(i => i.DbCountry);
        }

        // Find the C by ID and return
        var C = await query.FirstOrDefaultAsync(f => f.AddressId == id);

        var ret = new ResponseItemDto<IAddress>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = C
        };
        return ret;
    }

    public async Task<IAddress> DeleteAddressAsync(Guid id)
    {
        //Find the instance with matching id
        var query1 = _dbContext.Address
            .Where(i => i.AddressId == id);
        var item = await query1.FirstOrDefaultAsync<DbAddress>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.Address.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();
        return item;
    }
}