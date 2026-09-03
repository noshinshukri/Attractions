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

        _dbContext.Attraction.RemoveRange(_dbContext.Attraction);

        var attractions = Enumerable.Range(1, safeCount)
            .Select(i => new DbAttraction
            {
                AttractionId = Guid.NewGuid(),
                Name = $"Attraction {i}"
            })
            .ToList();

        _dbContext.Attraction.AddRange(attractions);
        await _dbContext.SaveChangesAsync();
    }

    public AdminDbRepos(ILogger<AdminDbRepos> logger, Encryptions encryptions, MainDbContext context)
    {
        _logger = logger;
        _encryptions = encryptions;
        _dbContext = context;
    }
}
