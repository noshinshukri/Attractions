using Microsoft.Extensions.Logging;

using Models;
using DbRepos;
using Services;
using Models.DTO;

namespace Services;

public class UserDbService : IUserService
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

    public Task<ResponsePageDto<IUser>> ReadUsersAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadUsersAsync(seeded, flat, filter, pageNumber, pageSize);
    public Task<ResponseItemDto<IUser>> ReadUserAsync(Guid id, bool flat) => _repo.ReadUserAsync(id, flat);
    public Task<IUser> DeleteUserAsync(Guid id) => _repo.DeleteUserAsync(id);
}