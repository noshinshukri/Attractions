using Models;
using Models.DTO;

namespace Services;
public interface ICountryService
{
public Task<ResponsePageDto<ICountry>> ReadCountriesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
    public Task<ResponseItemDto<ICountry>> ReadCountryAsync(Guid id, bool flat);
    public Task<ICountry> DeleteCountryAsync(Guid id);
}