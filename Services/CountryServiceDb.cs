using Microsoft.Extensions.Logging;

using Models;
using DbRepos;
using Models.DTO;
using DbContext;
using Services;
using Microsoft.EntityFrameworkCore;

namespace Services;

public class CountryDbService : ICountryService
{
    private readonly CountryDbRepos _repo = null;
    private readonly ILogger<CountryDbService> _logger = null;

    private readonly MainDbContext _dbContext;

    public CountryDbService(CountryDbRepos repo)
    {
        _repo = repo;
    }
    public CountryDbService(CountryDbRepos repo, ILogger<CountryDbService> logger) : this(repo)
    {
        _logger = logger;
    }

     public Task<ResponsePageDto<ICountry>> ReadCountriesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadCountriesAsync(seeded, flat, filter, pageNumber, pageSize);
     public Task<ResponseItemDto<ICountry>> ReadCountryAsync(Guid id, bool flat) => _repo.ReadCountryAsync(id, flat);
    public Task<ICountry> DeleteCountryAsync(Guid id) => _repo.DeleteCountryAsync(id);
}