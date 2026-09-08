namespace Models;

public interface ICategory
{
    public int CategoryId { get; set; }
    public string Name { get; set; }

    public List<IAttraction> Attractions { get; set; }
}