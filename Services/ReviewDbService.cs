using Microsoft.Extensions.Logging;

using Models;
using DbRepos;
using Services;
using Models.DTO;

namespace Services;

public class ReviewDbService : IReviewService
{
    private readonly ReviewDbRepos _repo = null;
    private readonly ILogger<ReviewDbService> _logger = null;

    public ReviewDbService(ReviewDbRepos repo)
    {
        _repo = repo;
    }
    public ReviewDbService(ReviewDbRepos repo, ILogger<ReviewDbService> logger) : this(repo)
    {
        _logger = logger;
    }

    public Task<ResponsePageDto<IReview>> ReadReviewsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _repo.ReadReviewsAsync(seeded, flat, filter, pageNumber, pageSize);
    public Task<ResponseItemDto<IReview>> ReadReviewAsync(Guid id, bool flat) => _repo.ReadReviewAsync(id, flat);
    public Task<IReview> DeleteReviewAsync(Guid id) => _repo.DeleteReviewAsync(id);

    public Task<ResponseItemDto<IReview>> UpdateReviewAsync(ReviewCuDto item) => _repo.UpdateReviewAsync(item);
    public Task<ResponseItemDto<IReview>> CreateReviewAsync(ReviewCuDto item) => _repo.CreateReviewAsync(item);
}