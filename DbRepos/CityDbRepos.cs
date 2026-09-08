using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
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
}
