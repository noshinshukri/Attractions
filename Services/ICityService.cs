using Models;
using Models.DTO;

namespace Services;
public interface ICityService
{
    public Task<ResponsePageDto<ICity>> ReadCitiesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
    public Task<ResponseItemDto<ICity>> ReadCityAsync(Guid id, bool flat);
    public Task<ICity> DeleteCityAsync(Guid id);
}