namespace Services;

using Models.DTO;

public interface IAdminService
{
    public Task<ResponseItemDto<GstUsrInfoAllDto>> SeedAsync(int nrOfItems);
    public Task<ResponseItemDto<GstUsrInfoAllDto>> RemoveSeedAsync(bool seeded);
    public Task<ResponseItemDto<GstUsrInfoAllDto>> GetDbInfoAsync();
}
