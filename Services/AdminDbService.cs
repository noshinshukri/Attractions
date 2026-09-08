using Microsoft.Extensions.Logging;

using DbRepos;

namespace Services;
    
public class AdminDbService : IAdminService
{
    private readonly AdminDbRepos _repo = null;
    private readonly ILogger<AdminDbService> _logger = null;

    public Task SeedAsync(int nrItems) => _repo.SeedAsync(nrItems);

    #region constructors
    public AdminDbService(AdminDbRepos repo)
    {
        _repo = repo;
    }
    public AdminDbService(AdminDbRepos repo, ILogger<AdminDbService> logger):this(repo)
    {
        _logger = logger;
    }
    #endregion
}

