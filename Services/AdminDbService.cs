using Microsoft.Extensions.Logging;

using DbRepos;
using Models.DTO;

namespace Services;

public class AdminDbService : IAdminService
{
    private readonly AdminDbRepos _repo = null;
    private readonly ILogger<AdminDbService> _logger = null;

    #region constructors
    public AdminDbService(AdminDbRepos repo)
    {
        _repo = repo;
    }
    public AdminDbService(AdminDbRepos repo, ILogger<AdminDbService> logger) : this(repo)
    {
        _logger = logger;
    }
    #endregion

    //Simple 1:1 calls in this case, but as Services expands, this will no longer need to be the case
    public Task<ResponseItemDto<GstUsrInfoAllDto>> GetDbInfoAsync() => _repo.DbInfo();
    public Task<ResponseItemDto<GstUsrInfoAllDto>> SeedAsync(int nrOfItems) => _repo.SeedAsync(nrOfItems);
    public Task<ResponseItemDto<GstUsrInfoAllDto>> RemoveSeedAsync(bool seeded) => _repo.RemoveSeedAsync(seeded);
}

