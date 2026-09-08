namespace Models;

public class Category : ICategory
{
    public virtual int CategoryId { get; set; }
    public virtual string Name { get; set; }

    public virtual List<IAttraction> Attractions { get; set; } = new();
}