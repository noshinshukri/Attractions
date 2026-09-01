using Microsoft.Extensions.Logging;
using DbModels;
using DbContext;
using Configuration;

namespace DbRepos;

public class AdminDbRepos
{
    private readonly ILogger<AdminDbRepos> _logger;
    private readonly Encryptions _encryptions;
    private readonly MainDbContext _dbContext;

    public async Task SeedAsync(int nrItems)
    {
        var safeCount = Math.Max(1, nrItems);

        _dbContext.Attractions.RemoveRange(_dbContext.Attractions);

        var attractions = Enumerable.Range(1, safeCount)
            .Select(i => new DbAttractions
            {
                AttractionsId = Guid.NewGuid(),
                Name = $"Attraction {i}"
            })
            .ToList();

        _dbContext.Attractions.AddRange(attractions);
        await _dbContext.SaveChangesAsync();
    }

    public AdminDbRepos(ILogger<AdminDbRepos> logger, Encryptions encryptions, MainDbContext context)
    {
        _logger = logger;
        _encryptions = encryptions;
        _dbContext = context;
    }
}
