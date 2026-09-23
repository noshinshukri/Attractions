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
        filter ??= "";
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
                .Include(i => i.DbReviews)
                    .ThenInclude(u => u.DbUser)
                .Include(i => i.DbCategories)
                .Include(i => i.DbAddress)
                    .ThenInclude(a => a.DbCity)
                .Include(i => i.DbAddress)
                    .ThenInclude(a => a.DbCountry);
                
        }
        var ret = new ResponsePageDto<IAttraction>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.Name.ToLower().Contains(filter) ||
                            i.DbAddress.DbCity.CityName.ToLower().Contains(filter) ||
                            i.DbAddress.DbCountry.CountryName.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.Name.ToLower().Contains(filter) ||
                            i.DbAddress.DbCity.CityName.ToLower().Contains(filter) ||
                            i.DbAddress.DbCountry.CountryName.ToLower().Contains(filter)))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IAttraction>(),

            PageNr = pageNumber,
            PageSize = pageSize
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
                .Include(i => i.DbReviews)
                    .ThenInclude(u => u.DbUser)
                .Include(i => i.DbCategories)
                .Include(i => i.DbAddress)
                    .ThenInclude(a => a.DbCity)
                .Include(i => i.DbAddress)
                    .ThenInclude(a => a.DbCountry);
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

    public async Task<ResponseItemDto<IAttraction>> CreateAttractionAsync(AttractionCuDto itemDto)
    {
        // 1. Validate that AttractionId is null
        if (itemDto.AttractionId != null)
            throw new ArgumentException($"{nameof(itemDto.AttractionId)} must be null when creating a new object");

        // 2. Create new database entity from DTO
        var item = new DbAttraction(itemDto);

        // 3. Update navigation properties
        await navProp_AttractionCUdto_to_AttractionDbM(itemDto, item);

        // 4. Add to context and save
        _dbContext.Attraction.Add(item);
        await _dbContext.SaveChangesAsync();

        // 5. Return fully populated item
        return await ReadAttractionAsync(item.AttractionId, false);
    }

    private async Task navProp_AttractionCUdto_to_AttractionDbM(AttractionCuDto itemDtoSrc, DbAttraction itemDst)
    {

        //Change to a excisting address
        if (itemDtoSrc.AddressId != null)
        {
            var address = await _dbContext.Address
                .FirstOrDefaultAsync(a => a.AddressId == itemDtoSrc.AddressId);

            if (address == null)
                throw new ArgumentException($"Address id {itemDtoSrc.AddressId} not existing");

            itemDst.DbAddress = address;
        }

        // Multiple relationships
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

        if (itemDtoSrc.CategoriesId != null)
        {
            var categories = new List<DbCategory>();
            foreach (var id in itemDtoSrc.CategoriesId)
            {
                var p = await _dbContext.Category.FirstOrDefaultAsync(i => i.CategoryId == id);
                if (p == null) throw new ArgumentException($"Category id {id} not existing");
                categories.Add(p);
            }
            itemDst.DbCategories = categories;
        }

        // Multiple relationships (Quotes) - similar pattern
    }

    public async Task<ResponseItemDto<IAttraction>> UpdateAttractionAsync(AttractionCuDto itemDto)
    {
        //Find the instance with matching id and read the navigation properties.
        var query1 = _dbContext.Attraction
            .Where(i => i.AttractionId == itemDto.AttractionId);
        var item = await query1
            .Include(i => i.DbReviews)
            .Include(i => i.DbCategories)
            .Include(i => i.DbAddress)
            .FirstOrDefaultAsync<DbAttraction>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {itemDto.AttractionId} is not existing");

        //transfer any changes from DTO to database objects
        //Update individual properties
        item.UpdateFromDTO(itemDto);

        //Update navigation properties
        await navProp_AttractionCUdto_to_AttractionDbM(itemDto, item);

        //write to database model
        _dbContext.Attraction.Update(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadAttractionAsync(item.AttractionId, false);
    }
}
