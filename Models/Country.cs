using Seido.Utilities.SeedGenerator;
namespace Models;

public class Country : ICountry, ISeed<Country>
{
    public virtual Guid CountryId { get; set; }
    public virtual string CountryName { get; set; }

    public virtual List<ICity> Cities { get; set; } = null;

    public bool Seeded {get; set;} = false;

    public virtual Country Seed(SeedGenerator seedGenerator)
    {
        Seeded = true;
        CountryId = Guid.NewGuid();

        CountryName = seedGenerator.Country;

        return this;
    }
}