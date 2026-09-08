namespace Models;

public class Country : ICountry
{
    public virtual Guid CountryId { get; set; }
    public virtual string CountryName { get; set; }

    public virtual List<ICity> Cities { get; set; } = null;
}