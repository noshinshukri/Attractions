using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using DbModels;
using Models.DTO;
using DbContext;

namespace DbRepos;

public class CategoryDbRepos
{
    private ILogger<CategoryDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public CategoryDbRepos(ILogger<CategoryDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponsePageDto<ICategory>> ReadCategoriesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        IQueryable<DbCategory> query = _dbContext.Category.AsNoTracking();

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.Category.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.Category.AsNoTracking()
                .Include(i => i.DbAttractions);
        }
        var ret = new ResponsePageDto<ICategory>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query.CountAsync(),
            PageItems = await query.ToListAsync<ICategory>(),
        };
        return ret;
    }

    public async Task<ResponseItemDto<ICategory>> ReadCategoryAsync(Guid id, bool flat)
    {
        IQueryable<DbCategory> query;

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.Category.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.Category.AsNoTracking()
                .Include(i => i.DbAttractions);
        }

        // Find the C by ID and return
        var C = await query.FirstOrDefaultAsync(f => f.CategoryId == id);

        var ret = new ResponseItemDto<ICategory>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = C
        };
        return ret;
    }

    public async Task<ICategory> DeleteCategoryAsync(Guid id)
    {
        //Find the instance with matching id
        var query1 = _dbContext.Category
            .Where(i => i.CategoryId == id);
        var item = await query1.FirstOrDefaultAsync<DbCategory>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.Category.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();
        return item;
    }

}
