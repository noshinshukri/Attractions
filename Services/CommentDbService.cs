using Microsoft.Extensions.Logging;

using Models;
using DbRepos;
using Services;

namespace Services;

public class CommentDbService : ICommentService
{
    private readonly CommentDbRepos _repo = null;
    private readonly ILogger<CommentDbService> _logger = null;

    public CommentDbService(CommentDbRepos repo)
    {
        _repo = repo;
    }
    public CommentDbService(CommentDbRepos repo, ILogger<CommentDbService> logger) : this(repo)
    {
        _logger = logger;
    }
}