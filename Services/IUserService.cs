using Models;
using Models.DTO;

namespace Services;

public interface IUserService
{
    public Task<ResponsePageDto<IUser>> ReadUsersAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
    public Task<ResponseItemDto<IUser>> ReadUserAsync(Guid id, bool flat);
    public Task<IUser> DeleteUserAsync(Guid id);

    public Task<ResponseItemDto<IUser>> UpdateUserAsync(UserCuDto item);
    public Task<ResponseItemDto<IUser>> CreateUserAsync(UserCuDto item);
}