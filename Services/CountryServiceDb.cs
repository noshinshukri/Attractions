using Microsoft.Extensions.Logging;

using Models;
using DbRepos;
using Services;

namespace Services;

public class CountryDbService : ICountryService
{
    private readonly CountryDbRepos _repo = null;
    private readonly ILogger<CountryDbService> _logger = null;

    public CountryDbService(CountryDbRepos repo)
    {
        _repo = repo;
    }
    public CountryDbService(CountryDbRepos repo, ILogger<CountryDbService> logger) : this(repo)
    {
        _logger = logger;
    }
}