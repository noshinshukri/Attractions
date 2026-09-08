using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using DbModels;
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
}
