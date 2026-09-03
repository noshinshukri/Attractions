using Microsoft.Extensions.Logging;

using Models;
using DbRepos;
using Services;

namespace Services;

public class UserDbService : IUserDbService
{
    private readonly UserDbRepos _repo = null;
    private readonly ILogger<UserDbService> _logger = null;

    public UserDbService(UserDbRepos repo)
    {
        _repo = repo;
    }
    public UserDbService(UserDbRepos repo, ILogger<UserDbService> logger) : this(repo)
    {
        _logger = logger;
    }
}