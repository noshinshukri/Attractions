using Seido.Utilities.SeedGenerator;

namespace Models;

public interface IAddress
{
    public Guid AddressId { get; set; }

    public ICity City { get; set; }
    public ICountry Country { get; set; }

    public List<IAttraction> Attractions { get; set; }

}