using Models;
using Models.DTO;

namespace Services;

public interface IReviewService
{
    public Task<ResponsePageDto<IReview>> ReadReviewsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
    public Task<ResponseItemDto<IReview>> ReadReviewAsync(Guid id, bool flat);
    public Task<ResponseItemDto<IReview>> DeleteReviewAsync(Guid id);

    public Task<ResponseItemDto<IReview>> UpdateReviewAsync(ReviewCuDto item);
    public Task<ResponseItemDto<IReview>> CreateReviewAsync(ReviewCuDto item);
}