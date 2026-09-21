using Models;
using Models.DTO;

namespace Services;

public interface ICategoryService
{
    public Task<ResponsePageDto<ICategory>> ReadCategoriesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
    public Task<ResponseItemDto<ICategory>> ReadCategoryAsync(Guid id, bool flat);
    public Task<ICategory> DeleteCategoryAsync(Guid id);
}