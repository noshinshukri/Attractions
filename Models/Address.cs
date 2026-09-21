using Seido.Utilities.SeedGenerator;

namespace Models;

public class Address : IAddress, ISeed<Address>
{
    public virtual Guid AddressId { get; set; }

    public virtual ICity City { get; set; }
    public virtual ICountry Country { get; set; }

    public virtual List<IAttraction> Attractions { get; set; } = null;

    public bool Seeded { get; set; } = false;
    public virtual Address Seed(SeedGenerator seedGenerator)
    {
        Seeded = true;
        AddressId = Guid.NewGuid();

        return this;
    }

}