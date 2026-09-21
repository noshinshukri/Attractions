using Models;
using Models.DTO;

namespace Services;
public interface ICommentService
{
    public Task<ResponsePageDto<IComment>> ReadCommentsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
        public Task<ResponseItemDto<IComment>> ReadCommentAsync(Guid id, bool flat);
    public Task<IComment> DeleteCommentAsync(Guid id);
}