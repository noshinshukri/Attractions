namespace Models;

public interface ICountry
{
    public Guid CountryId { get; set; }
    public string CountryName { get; set; }
    public List<ICity> Cities { get; set; }
}