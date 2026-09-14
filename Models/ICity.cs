namespace Models;

public interface ICity
{
    public Guid CityId { get; set; }
    public string CityName { get; set; }
    public List<IAttraction> Attractions { get; set; }
    public ICountry Country { get; set; }
}