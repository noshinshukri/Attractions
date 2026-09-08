using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using DbModels;
using DbContext;

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
}
