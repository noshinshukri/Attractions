using Microsoft.Extensions.Logging;

using Models;
using DbRepos;
using Services;

namespace Services;

public class CategoryDbService : ICategoryService
{
    private readonly CategoryDbRepos _repo = null;
    private readonly ILogger<CategoryDbService> _logger = null;

    public CategoryDbService(CategoryDbRepos repo)
    {
        _repo = repo;
    }
    public CategoryDbService(CategoryDbRepos repo, ILogger<CategoryDbService> logger) : this(repo)
    {
        _logger = logger;
    }
}