using Microsoft.Extensions.Logging;

using Models;
using DbRepos;
using Services;

namespace Services;

public class AddressDbService : IAddressService
{
    private readonly AddressDbRepos _repo = null;
    private readonly ILogger<AddressDbService> _logger = null;

    public AddressDbService(AddressDbRepos repo)
    {
        _repo = repo;
    }
    public AddressDbService(AddressDbRepos repo, ILogger<AddressDbService> logger) : this(repo)
    {
        _logger = logger;
    }
}