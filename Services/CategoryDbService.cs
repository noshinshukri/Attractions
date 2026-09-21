using Microsoft.Extensions.Logging;

using Models;
using Models.DTO;
using DbRepos;
using Services;

namespace Services;

public class CategoryDbService : ICategoryService
{
    private readonly CategoryDbRepos _repo = null;
    private readonly ILogger<CategoryDbService> _logger = null;

    public CategoryDbService(CategoryDbRepos repo)
    {
        _repo = repo;
    }
    public CategoryDbService(CategoryDbRepos repo, ILogger<CategoryDbService> logger) : this(repo)
    {
        _logger = logger;
    }

    public Task<ResponsePageDto<ICategory>> ReadCategoriesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadCategoriesAsync(seeded, flat, filter, pageNumber, pageSize);
    public Task<ResponseItemDto<ICategory>> ReadCategoryAsync(Guid id, bool flat) => _repo.ReadCategoryAsync(id, flat);
    public Task<ICategory> DeleteCategoryAsync(Guid id) => _repo.DeleteCategoryAsync(id);
}