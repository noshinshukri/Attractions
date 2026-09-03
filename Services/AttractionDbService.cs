using Microsoft.Extensions.Logging;

using Models;
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
}