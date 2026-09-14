using Microsoft.Extensions.Logging;

using Models;
using Models.DTO;
using DbRepos;
using Services;

namespace Services;

public class CityDbService : ICityService
{
    private readonly CityDbRepos _repo = null;
    private readonly ILogger<CityDbService> _logger = null;

    public CityDbService(CityDbRepos repo)
    {
        _repo = repo;
    }
    public CityDbService(CityDbRepos repo, ILogger<CityDbService> logger) : this(repo)
    {
        _logger = logger;
    }

    public Task<ResponsePageDto<ICity>> ReadCitiesAsync() => _repo.ReadCitiesAsync();

}