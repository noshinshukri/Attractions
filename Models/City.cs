namespace Models;

public class City : ICity
{
    public virtual Guid CityId { get; set; }
    public virtual string CityName { get; set; }
    public virtual List<IAttraction> Attractions { get; set; } = null;
}