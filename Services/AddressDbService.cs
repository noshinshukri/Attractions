using Microsoft.Extensions.Logging;

using Models;
using Models.DTO;
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

    public Task<ResponsePageDto<IAddress>> ReadAddressesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadAdressesAsync(seeded, flat, filter, pageNumber, pageSize);
    public Task<ResponseItemDto<IAddress>> ReadAddressAsync(Guid id, bool flat) => _repo.ReadAddressAsync(id, flat);
    public Task<IAddress> DeleteAddressAsync(Guid id) => _repo.DeleteAddressAsync(id);
}