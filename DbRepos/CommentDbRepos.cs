using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using DbModels;
using DbContext;

namespace DbRepos;

public class CommentDbRepos
{
    private ILogger<CommentDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public CommentDbRepos(ILogger<CommentDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
}
