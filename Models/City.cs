namespace Models;
using Seido.Utilities.SeedGenerator;

public class City : ICity, ISeed<City>
{
    public virtual Guid CityId { get; set; }
    public virtual string CityName { get; set; }
    public virtual List<IAttraction> Attractions { get; set; } = null;
    public virtual ICountry Country { get; set; } = null;

    public bool Seeded {get; set;} = false;

        public virtual City Seed(SeedGenerator seedGenerator)
    {
        Seeded = true;
        CityId = Guid.NewGuid();

        CityName = seedGenerator.City();

        return this;
    }

}

