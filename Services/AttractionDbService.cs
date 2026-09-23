using Microsoft.Extensions.Logging;

using Models;
using Models.DTO;
using DbRepos;
using Services;

namespace Services;

public class AttractionDbService : IAttractionService
{
    private readonly AttractionDbRepos _repo = null;
    private readonly ILogger<AttractionDbService> _logger = null;

    public AttractionDbService(AttractionDbRepos repo)
    {
        _repo = repo;
    }
    public AttractionDbService(AttractionDbRepos repo, ILogger<AttractionDbService> logger) : this(repo)
    {
        _logger = logger;
    }

    public Task<ResponsePageDto<IAttraction>> ReadAttractionsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadAttractionsAsync(seeded, flat, filter, pageNumber, pageSize);
    public Task<ResponseItemDto<IAttraction>> ReadAttractionAsync(Guid id, bool flat) => _repo.ReadAttractionAsync(id, flat);
    public Task<IAttraction> DeleteAttractionAsync(Guid id) => _repo.DeleteAttractionAsync(id);

    public Task<ResponseItemDto<IAttraction>> UpdateAttractionAsync(AttractionCuDto item) => _repo.UpdateAttractionAsync(item);
    public Task<ResponseItemDto<IAttraction>> CreateAttractionAsync(AttractionCuDto item) => _repo.CreateAttractionAsync(item);
}